using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cadastro.Models.Services.Application.IO.Files
{
    public static class DeleteFiles
    {

        public static Queue<FileToObject> listaParaDeletar { get; set; } = new Queue<FileToObject>();
        public static Queue<FileToObject> listaParaSalvar { get; set; } = new Queue<FileToObject>();
        public static bool Processing { get; set; }
        public static string pathFileList = Path.Combine(Environment.CurrentDirectory, "StaticFiles", "temp", "ListToDelete.txt");


        //########################################################################################################################
        private static void GravarLista()
        {
            if (Processing) return;
            Processing = true;
            StreamWriter fileW = new StreamWriter(pathFileList, false, Encoding.UTF8);

            while (listaParaSalvar.Any())
            {
                fileW.WriteLine(listaParaSalvar.Dequeue().PathFile);
            }
            fileW.Dispose();

            Processing = false;
        }

        //########################################################################################################################
        private static void LerLista(Queue<FileToObject> lista)
        {
            if (Processing) return;
            Processing = true;

            //Queue<FileToObject> res = new Queue<FileToObject>();

            StreamReader filerR = new StreamReader(pathFileList);
            while (!filerR.EndOfStream)
            {
                lista.Enqueue(new FileToObject(filerR.ReadLine()));
            }
            filerR.Dispose();
            Processing = false;
        }


        //########################################################################################################################
        public static void DeletarLista()
        {
            if (Processing) return;
            LerLista(listaParaDeletar);
            Processing = true;

            FileToObject item;
            while (listaParaDeletar.Any())
            {
                item = listaParaDeletar.Dequeue();
                Delete(item.PathFile, true);
            }

            Processing = false;
            GravarLista();
        }

        //########################################################################################################################
        public static void Delete(string path, bool listado = false)
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception("Erro: Path nullo para deletar");

            FileToObject file = new FileToObject(path);

            //Criar pasta caso não exista
            FileToObject listToDelete = new FileToObject(pathFileList);
            if (!Directory.Exists(listToDelete.GetFullPathDirectory()))
                Directory.CreateDirectory(listToDelete.GetFullPathDirectory());

            //Criar lista de deleção caso não exista
            if (!File.Exists(pathFileList))
                File.Create(pathFileList);

            if (string.IsNullOrEmpty(path)) return;
            try
            {
                if (file.Exists())
                {
                    File.Delete(path);
                }
                return;
            }
            catch
            {
                listaParaSalvar.Enqueue(new FileToObject(path));
            }
            finally
            {
                if (!File.Exists(pathFileList))
                    File.Create(pathFileList);

                DeletarLista();

            }

        }

    }
}
