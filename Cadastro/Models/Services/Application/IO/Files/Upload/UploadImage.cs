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
    public class UploadImage
    {
        public UploadImage(IFormFile imagem, string folder, List<string> extensoes, long tamMax)
        {
            Imagem = imagem;
            Folder = folder;
            Extensoes = extensoes;
            TamMax = tamMax;
        }

        private Dictionary<string, string> NovoNome { get; set; }
        private IFormFile Imagem { get; set; }
        private long TamMax { get; set; }
        private string Folder { get; set; }
        private List<string> Extensoes { get; set; }
        private string PathWwwroot { get; set; } = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "wwwroot");
        private string ImageToDelete { get; set; }

        //Getters e Setters
        public string SetImageToDele { set { ImageToDelete = value; } }




        //####################################################################################################
        public async Task Upload()
        {
            if (NovoNome == null)
                throw new Exception("Não foi gerado um nome para o arquivo");

            if (!Extensoes.Any()) throw new Exception("Nenhum formato de arquivo foi informado!");

            //Gera a mensagem de extensões aceitas para avisar de extensões aceitas
            StringBuilder msgExtensoesAceitas = new StringBuilder();
            msgExtensoesAceitas.Append($"Formatos aceitos ");
            msgExtensoesAceitas.Append($"({string.Join(", ", Extensoes)})");

            //Verificar se os dados foram informados
            string erroImagem = "Erro ao tentar enviar imagem";
            if (Imagem == null) throw new Exception(erroImagem, new Exception("O IFormFile está null"));
            if (Imagem.Length == 0) throw new Exception(erroImagem, new Exception("O IFormFile.Length é igual a 0"));
            if (!Path.HasExtension(Imagem.FileName)) throw new Exception("O arquivo enviado não possui uma extensão");
            if (string.IsNullOrWhiteSpace(Folder)) throw new Exception(erroImagem, new Exception("O caminho da nova imagem não foi informado"));//Precisa definir onde a imagem será salva à partir do wwwroot
            if (!Extensoes.Any()) throw new Exception(erroImagem, new Exception("Nenhum formato foi informado"));//é obrigatório informar ao menos 1 formato
            if (Imagem.Length > TamMax) throw new Exception($"Tamanho máximo permitido do arquivo {TamMax / 1024f:N2} Kb");//Verifica se tamanho máximo do arquivo foi atingido
            if (!Extensoes.Any(f => f == NovoNome["extensao"])) throw new Exception(msgExtensoesAceitas.ToString());//Verifica se a extensão do arquivo é aceito

            try
            {
                //Se o local para salvar não existir, ele tentará criar
                if (!Directory.Exists(Path.Combine(PathWwwroot, Folder))) Directory.CreateDirectory(Path.Combine(PathWwwroot, Folder));

                //Salvar nova imagem
                FileStream fileS = new FileStream(Path.Combine(PathWwwroot, Folder, $"{NovoNome["nome"]}{NovoNome["extensao"]}"), FileMode.Create);
                await Imagem.CopyToAsync(fileS);
                await fileS.DisposeAsync();
            }
            catch (Exception err)
            {
                throw new Exception(erroImagem, err);
            }

            try
            {
                //Apagar antigo arquivo se solicitado
                if (!string.IsNullOrWhiteSpace(ImageToDelete))
                {
                    if (File.Exists(Path.Combine(PathWwwroot, Folder, ImageToDelete)))
                        File.Delete(Path.Combine(PathWwwroot, Folder, ImageToDelete));
                }
            }
            catch { }
        }

        //####################################################################################################
        public string GerarNome()
        {

            if (Imagem == null) return null;

            //Gera novo nome da imagem
            NovoNome = new Dictionary<string, string>(){
                {"nome", GeradorKey.Gerar(60, true, true, true)},
                {"extensao", Path.GetExtension(Imagem.FileName).ToLower()}
            };

            return $"{NovoNome["nome"]}{NovoNome["extensao"]}";
        }

        /// <summary> Remove a imagem possivelmente salva por tentativa, use este método caso tenha ocorrido algum erro no salvamento </summary>
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
