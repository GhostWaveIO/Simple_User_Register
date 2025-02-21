using Cadastro.Models.Services.Application.Text.Generators.Keys;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.IO.Files.Upload
{
    public class UploadDocument
    {
        public UploadDocument(IFormFile documento, string folder, List<string> extensoes, long tamMax)
        {
            Documento = documento;
            Folder = folder;
            Extensoes = extensoes;
            TamMax = tamMax;
        }

        private Dictionary<string, string> NovoNome { get; set; }
        private IFormFile Documento { get; set; }
        private long TamMax { get; set; }
        private string Folder { get; set; }
        private List<string> Extensoes { get; set; }
        private string PathWwwroot { get; set; } = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "wwwroot");
        private string documentToDelete { get; set; }

        //Getters e Setters
        public string SetDocumentToDele { set { documentToDelete = value; } }

        //####################################################################################################
        public async Task Upload()
        {
            if (NovoNome == null)
                throw new Exception("Não foi gerado um nome para o arquivo");

            if (!Extensoes.Any()) throw new Exception("Nenhum formato de arquivo foi informado!");

            //Gera a mensagem de extensões aceitas para avisar de extensões aceitas
            StringBuilder msgExtensoesAceitas = new StringBuilder();
            msgExtensoesAceitas.Append($"Formatos aceitos ");
            msgExtensoesAceitas.Append($"({string.Join(", ", Extensoes)}) [{NovoNome["extensao"]}]");

            //Verificar se os dados foram informados
            string erroDocumento = "Erro ao tentar enviar Documento";
            if (Documento == null) throw new Exception(erroDocumento, new Exception("O IFormFile está null"));
            if (Documento.Length == 0) throw new Exception(erroDocumento, new Exception("O IFormFile.Length é igual a 0"));
            if (!Path.HasExtension(Documento.FileName)) throw new Exception("O arquivo enviado não possui uma extensão");
            if (string.IsNullOrWhiteSpace(Folder)) throw new Exception(erroDocumento, new Exception("O caminho da nova Documento não foi informado"));//Precisa definir onde o Documento será salva à partir do wwwroot
            if (!Extensoes.Any()) throw new Exception(erroDocumento, new Exception("Nenhum formato foi informado"));//é obrigatório informar ao menos 1 formato
            if (Documento.Length > TamMax) throw new Exception($"Tamanho máximo permitido do arquivo {TamMax / 1024f:N2} Kb");//Verifica se tamanho máximo do arquivo foi atingido
            if (!Extensoes.Any(f => f == NovoNome["extensao"])) throw new Exception(msgExtensoesAceitas.ToString());//Verifica se a extensão do arquivo é aceito

            try
            {
                //Se o local para salvar não existir, ele tentará criar
                if (!Directory.Exists(Path.Combine(PathWwwroot, Folder))) Directory.CreateDirectory(Path.Combine(PathWwwroot, Folder));

                //Salvar novo Documento
                FileStream fileS = new FileStream(Path.Combine(PathWwwroot, Folder, $"{NovoNome["nome"]}{NovoNome["extensao"]}"), FileMode.Create);
                await Documento.CopyToAsync(fileS);
                await fileS.DisposeAsync();
            }
            catch (Exception err)
            {
                throw new Exception(erroDocumento, err);
            }

            try
            {
                //Apagar antigo arquivo se solicitado
                if (!string.IsNullOrWhiteSpace(documentToDelete))
                {
                    if (File.Exists(Path.Combine(PathWwwroot, Folder, documentToDelete)))
                        File.Delete(Path.Combine(PathWwwroot, Folder, documentToDelete));
                }
            }
            catch { }
        }

        //####################################################################################################
        public string GerarNome()
        {

            if (Documento == null) return null;

            //Gera novo nome do Documento
            NovoNome = new Dictionary<string, string>(){
                {"nome", GeradorKey.Gerar(60, true, true, true)},
                {"extensao", Path.GetExtension(Documento.FileName).ToLower()}
            };

            return $"{NovoNome["nome"]}{NovoNome["extensao"]}";
        }

        //####################################################################################################
        /// <summary> Remove o Documento possivelmente salva por tentativa, use este método caso tenha ocorrido algum erro no salvamento </summary>
        public void DeleteFileError()
        {
            if (NovoNome == null)
                throw new Exception("Não foi gerado um nome para o arquivo");

            try
            {
                if (File.Exists(Path.Combine(PathWwwroot, Folder, $"{NovoNome["nome"]}{NovoNome["extensao"]}")))
                    File.Delete(Path.Combine(PathWwwroot, Folder, $"{NovoNome["nome"]}{NovoNome["extensao"]}"));
            }
            catch { }
        }

    }
}
