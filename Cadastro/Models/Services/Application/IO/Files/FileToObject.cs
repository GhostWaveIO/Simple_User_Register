using System;
using System.IO;

namespace Cadastro.Models.Services.Application.IO.Files
{
    public class FileToObject
    {

        public string PathFile { get; set; }

        public FileToObject(string PathFile)
        {
            this.PathFile = PathFile;
        }


        //##############################################################################################################################
        public void Delete()
        {
            if (string.IsNullOrEmpty(PathFile))
                return;
            DeleteFiles.Delete(PathFile);
        }

        //##############################################################################################################################
        public string GetFullPathDirectory()
        {
            string pathDirectory = null;
            string[] divPath;

            if (!string.IsNullOrEmpty(PathFile))
                pathDirectory = string.Empty;
            divPath = PathFile.Split(Path.DirectorySeparatorChar);
            pathDirectory = Path.Combine(divPath);
            //for (int c = 0; c + 1 < divPath.Length && divPath.Length > 0; c++) {
            //     += $"{divPath[c]}{Path.DirectorySeparatorChar}";
            //}


            return pathDirectory;
        }

        //##############################################################################################################################
        public string GetFileName()
        {

            string fileName = null;

            if (!string.IsNullOrEmpty(PathFile))
                if (File.Exists(PathFile))
                {
                    fileName = Path.GetFileName(PathFile);
                }

            return fileName;
        }

        //##############################################################################################################################
        public string GetExtension()
        {
            return Path.GetExtension(PathFile);
        }

        //##############################################################################################################################
        public bool Exists()
        {
            return File.Exists(PathFile);
        }

        //##############################################################################################################################
        public void CopyFile(string newPath)
        {
            try
            {
                if (Exists())
                    File.Copy(PathFile, newPath);
                else
                    throw new Exception("Este arquivo não existe.");
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }

        }

        //##############################################################################################################################
        public void CreateFile()
        {
            File.Create(PathFile);
        }

        //##############################################################################################################################
        public void CreateDirectory()
        {
            Directory.CreateDirectory(GetFullPathDirectory());
        }
    }
}
