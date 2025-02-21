using Cadastro.Models.Services.Application.Settings.Geral;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.StaticPages.ActionResult
{
    public class WebContent
    {

        //Publics
        public string Reference { get; set; }
        public string[] CadeiaDiretorios { get; set; }
        public bool IsValid { get; set; }
        public string PathPages { get; set; }
        public string PathFile { get; set; }
        public string ContentType { get; set; }

        //Privates
        private char sep { get; set; }
        private string ContentFile { get; set; }
        private IUrlHelper Url { get; set; }
        private readonly Config conf;
        private StreamReader FileR { get; set; }
        private MemoryStream FileStream { get; set; }

        //****************************************************************************************************
        public WebContent(IUrlHelper url, string reference)
        {
            IsValid = false;
            if (string.IsNullOrWhiteSpace(reference)) return;

            Reference = reference;
            sep = Path.DirectorySeparatorChar;
            PathPages = /*Path.Combine(); */@$"{Environment.CurrentDirectory}{sep}\wwwroot\Paginas\";
            CadeiaDiretorios = GetCadeiaDiretorios();
            PathFile = @$"{PathPages}{string.Join('\\', CadeiaDiretorios)}";
            ContentType = GetContentType();
            Url = url;
            conf = Program.Config;

            if (File.Exists(PathFile) && CadeiaDiretorios.ToList().All(d => !string.IsNullOrWhiteSpace(d)))
                IsValid = true;
        }

        //****************************************************************************************************
        public WebContent(IUrlHelper url)
        {
            IsValid = false;

            Reference = "index.html";
            sep = Path.DirectorySeparatorChar;
            PathPages = @$"{Environment.CurrentDirectory}{sep}wwwroot{sep}Paginas{sep}";
            CadeiaDiretorios = GetCadeiaDiretorios();
            PathFile = @$"{PathPages}{string.Join(sep, CadeiaDiretorios)}";
            ContentType = GetContentType();
            Url = url;
            conf = Program.Config;

            if (File.Exists(PathFile))
                IsValid = true;
        }

        //####################################################################################################
        private string[] GetCadeiaDiretorios()
        {
            if (string.IsNullOrEmpty(Reference)) return null;
            string[] res = Reference.Split('|');

            return res;
        }

        //####################################################################################################
        private string GetContentType()
        {

            string extensao = Path.GetExtension(PathFile);//Ex: .html
            string contentType = string.Empty;

            switch (extensao.ToLower())
            {
                case ".html":
                case ".htm":
                    contentType = "text/html";
                    break;
                /*case ".css":
                case ".scss":
                  contentType = "text/css";
                  break;
                case ".js":
                  contentType = "application/javascript+module";
                  break;
                case ".map":
                  contentType = "application/octet-stream";
                  break;
                case ".xml":
                  contentType = "application/xml";
                  break;
                case ".csv":
                  contentType = "text/csv";
                  break;
                case ".xls":
                case ".xlsx":
                  contentType = "application/vnd.ms-excel";
                  break;
                case ".json":
                  contentType = "application/json";
                  break;
                case ".doc":
                  contentType = "application/msword";
                  break;
                case ".jpg":
                  contentType = "image/jpg";
                  break;
                case ".jpeg":
                  contentType = "image/jpeg";
                  break;
                case ".png":
                  contentType = "image/png";
                  break;
                case ".gif":
                  contentType = "image/gif";
                  break;
                case ".tif":
                case ".tiff":
                  contentType = "image/tiff";
                  break;
                case ".ico":
                  contentType = "image/x-icon";
                  break;
                case ".avi":
                  contentType = "video/x-msvideo";
                  break;
                case ".mpeg":
                  contentType = "video/mpeg";
                  break;
                case ".pdf":
                  contentType = "application/pdf";
                  break;
                case ".svg":
                  contentType = "image/svg+xml";
                  break;
                case ".webm":
                  contentType = "video/webm";
                  break;
                case ".webp":
                  contentType = "image/webp";
                  break;
                case ".zip":
                  contentType = "application/zip";
                  break;
                case ".7z":
                  contentType = "application/x-7z-compressed";
                  break;
                case ".ttf":
                  contentType = "font/ttf";
                  break;
                case ".eot":
                  contentType = "application/vnd.ms-fontobject";
                  break;
                case ".woff":
                  contentType = "font/woff";
                  break;
                case ".woff2":
                  contentType = "font/woff2";
                  break;*/
                default:
                    contentType = "";
                    break;
            }

            return contentType;
        }

        //####################################################################################################
        private async Task CollectFile()
        {
            if (!IsValid || ContentFile != null) return;
            ContentFile = string.Empty;

            /*byte[] bytes;

            switch (Path.GetExtension(PathFile)) {
              case ".jpg":
              case ".jpeg":
              case ".png":
              case ".gif":
              case ".pdf":
              case ".webp":
              case ".webm":
              case ".zip":
              case ".7z":
              case ".mpeg":
              case ".avi":
              case ".doc":
              case ".ico":
              case ".tif":
              case ".tiff":
              case ".ttf":
              case ".eot":
              case ".woff":
              case ".woff2":
                this.FileR = new StreamReader(PathFile);
                bytes = new byte[FileR.BaseStream.Length];
                //while () {}
                //this.FileStream = new MemoryStream(FileR.BaseStream.Read); ;
                break;
              default:
                break;
            }*/
            FileR = new StreamReader(PathFile, Encoding.UTF8);
        using (FileR)
            {
                ContentFile = await FileR.ReadToEndAsync();
            }

        }

        //####################################################################################################
        /// <summary>As páginas ficam à partir da pasta "wwwroot/Paginas", qualquer referência antes disso, será consideredo caminho estrapolado</summary>
        public bool StrappedPath(string conteudo)
        {

            bool res = false;
            StringBuilder strapped = new StringBuilder();
            for (int d = 0; d < CadeiaDiretorios.Length; d++)
            {
                strapped.Append("../");
            }

            if (conteudo.Contains(strapped.ToString()))
                res = false;

            return res;
        }

        //####################################################################################################
        private string ConvertReferences(string content)
        {
            if (StrappedPath(content)) throw new Exception("Caminho estrapolado!");
            string res = content;
            int qtdDiretorios = CadeiaDiretorios.Length - 1;
            StringBuilder returnDir;
            string caminhoReal = "";

            string[] cadeia = CadeiaDiretorios;
            int qtd = cadeia.Length - 1;

            for (int c = 0; c < cadeia.Length - 1; c++)
            {
                returnDir = new StringBuilder("|");
                caminhoReal = "";
                //Verifica a quantidade de retorno
                for (int d = qtd; d - c > 0; d--)
                    returnDir.Append("..|");

                //constrói caminho real
                for (int cr = 0; cr < c; cr++)
                    caminhoReal += $"{cadeia[cr]}|";

                res = res.Replace(returnDir.ToString(), $"|{caminhoReal}");
                res = res.Replace(returnDir.ToString().Substring(1), caminhoReal);
            }

            res = res.
            Replace("#url-home#", Url.ActionLink("", "")).
            Replace("#url-paginas#", $"{Url.ActionLink("", "")}Pagina/").
            Replace("#up#", $"{Url.ActionLink("", "")}Pagina/").
            Replace("#url-login#", Url.ActionLink("Index", "Login")).
            Replace("#url-principal#", conf.urlSite).
            Replace("#versao-sistema#", conf.Versao).
            Replace("#nome-sistema#", conf.nomeSistema).
            Replace("#logo-cabecalho#", $"{Url.ActionLink("", "")}Imagens/logo.png").
            Replace("#telefone#", conf.fone).
            Replace("#telefone-url-zap#", conf.foneUrl).
            Replace("#ano-atual-xx#", DateTime.Today.ToString("yy")).
            Replace("#ano-atual-xxxx#", DateTime.Today.ToString("yyyy"));


            return res;
        }

        //####################################################################################################
        public async Task<IActionResult> GetResult()
        {
            if (!IsValid) throw new Exception("Url inválida!");

            IActionResult res = null;

            //Gera a url direta do arquivo
            StringBuilder local = new StringBuilder($"{Url.ActionLink("", "")}Paginas/");
            for (int d = 0; d < CadeiaDiretorios.Length; d++)
            {
                local.Append(CadeiaDiretorios[d]);
                if (d != CadeiaDiretorios.Length - 1)
                    local.Append("/");
            }

            //Retorna arquivo que não seja HTML
            if (string.IsNullOrEmpty(ContentType))
            {
                return new RedirectResult(local.ToString());//new FileStreamResult(FileR.BaseStream, this.ContentType);
            }

            //Retorna Somente HTML
            switch (Path.GetExtension(PathFile))
            {
                case ".html":
                case ".htm":
                    await CollectFile();
                    res = new ContentResult()
                    {
                        Content = ConvertReferences(ContentFile),
                        ContentType = ContentType
                    };
                    break;
            }

            return res;
        }

        //####################################################################################################
        public void Dispose()
        {
            //await Task.Delay(2000);
            if (FileR == null) return;
            FileR.Dispose();
        }
    }
}
