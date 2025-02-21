using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Cadastro.Models.Services.Application.Logs
{

    public static class ErroFileService
    {

        public static Queue<ErroFile> filaErros = new Queue<ErroFile>();
        public static bool Saving = false;
        private static readonly string path = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "logs", "error");
        private static readonly string filePath = path + "erro.txt";

        public static void ColherErros(string Titulo, string Conteudo)
        {
            Titulo = Titulo.Replace("\n", "<br/>").Replace("\r\n", "<br/>").Replace(Environment.NewLine, "<br/>");
            Conteudo = Conteudo.Replace("\n", "<br/>").Replace("\r\n", "<br/>").Replace(Environment.NewLine, "<br/>");
            filaErros.Enqueue(new ErroFile(Titulo, Conteudo));
            SalvarErros();
        }

        private static void SalvarErros()
        {
            if (Saving)
                return;
            else
                Saving = true;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            StreamWriter fileW = new StreamWriter(filePath, true);
            ErroFile erro;

            for (int c = 0; c < filaErros.Count(); c++)
            {
                erro = filaErros.Dequeue();
                erro.Conteudo = erro.Conteudo.Replace("\n", "<br/>").Replace("\r\n", "<br/>").Replace(@"\n", "<br/>").Replace(@"\r\n", "<br/>");//tira quebra de linha para não bugar
                fileW.Write(erro.Titulo + Environment.NewLine);
                fileW.Write(erro.Conteudo + Environment.NewLine);
                fileW.Write(DateTime.Now.ToString() + Environment.NewLine);
            }
            fileW.Dispose();

            Saving = false;
        }

        public static List<ErroFile> GetListaErros()
        {
            List<ErroFile> res = new List<ErroFile>();
            ErroFile erro;
            string criado;
            if (!File.Exists(filePath)) return res;
            StreamReader fileR = new StreamReader(filePath);

            while (!fileR.EndOfStream)
            {
                erro = new ErroFile();
                erro.Titulo = fileR.ReadLine();
                erro.Conteudo = fileR.ReadLine();
                criado = fileR.ReadLine();
                while (criado.Length != 19)
                {
                    erro.Conteudo += "\r\n" + criado;
                    criado = fileR.ReadLine();
                }
                erro.Criado = Convert.ToDateTime(criado); ;
                res.Add(erro);
            }

            fileR.Dispose();
            return res;
        }
    }

}
