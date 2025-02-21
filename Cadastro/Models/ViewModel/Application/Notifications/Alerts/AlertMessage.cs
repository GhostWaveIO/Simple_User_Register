using Cadastro.Models.Services.Application.Text.Generators.Keys;
using System;
using System.Collections.Generic;
using System.Linq;

//#fim no final de um alert, é uma técnica usado para quando colher erros, filtrar os com "#fim" para não serem colhidos,
//é usado para não ser enviados erros para o coletor de erros sem necessidade

namespace Cadastro.Models.ViewModel.Application.Notifications.Alerts
{
    public class AlertMessage
    {

        //public AlertMessage(string code) {
        //    _code = code;
        //}

        //public string _code { get; set; }

        #region Static
        private static List<Message> listaMensagens { get; set; } = new List<Message>();

        public static string Danger(string msg, bool returnKey = true)
        {
            string texto = $"<p class=\"alert alert-danger container text-center my-3\">{msg}</p>";
            if (returnKey)
                return SetMessage(texto);
            else
                return texto.Replace("#fim", "");
        }

        public static string Success(string msg, bool returnKey = true)
        {
            string texto = $"<p class=\"alert alert-success container text-center my-3\">{msg}</p>";
            if (returnKey)
                return SetMessage(texto);
            else
                return texto.Replace("#fim", "");
        }

        public static string Info(string msg, bool returnKey = true)
        {
            string texto = $"<p class=\"alert alert-info container text-center my-3\">{msg}</p>";
            if (returnKey)
                return SetMessage(texto);
            else
                return texto.Replace("#fim", "");
        }

        public static string Warning(string msg, bool returnKey = true)
        {
            string texto = $"<p class=\"alert alert-warning container text-center my-3\">{msg}</p>";
            if (returnKey)
                return SetMessage(texto);
            else
                return texto.Replace("#fim", "");
        }

        public static string CorrigeUrl(string texto)
        {
            return texto?.Replace("%2F", "/");
        }

        public static string SetMessage(string texto)
        {
            ReciclarMesagens();
            if (string.IsNullOrEmpty(texto)) return null;

            string Key;

            //Gera a chave da mensagem
            do
            {
                Key = GeradorKey.Gerar(10);//Config.GeraKey(10);
            } while (listaMensagens.Any(m => m.key == Key));

            Message msg = new Message(texto, Key);
            listaMensagens.Add(new Message(texto, Key));
            return Key + "-";
        }

        private static string GetDivMessage(string key)
        {
            if (key == null) { return string.Empty; }

            Message msg = listaMensagens.FirstOrDefault(lm => lm.key == key) ?? null;
            string res = msg?.Texto ?? "";

            //Remover a mensagem consultada
            listaMensagens.Remove(msg);

            return res;
        }

        public static string GetMessage(string key)
        {
            string res = string.Empty;
            string[] divMessage = null;

            if (!string.IsNullOrEmpty(key))
            {
                divMessage = key.Split('-');

                foreach (string m in divMessage)
                {
                    res += GetDivMessage(m);
                }
            }
            res = res.Replace("#fim", "");

            return res;
        }

        //Recicla mensagens por mais de um tempo determinado
        private static void ReciclarMesagens()
        {
            if (listaMensagens.Count() <= 15) return;

            DateTime dataMin = DateTime.Now.AddMinutes(-30);


            foreach (Message m in listaMensagens)
            {
                if (m.Criado <= dataMin)
                {
                    listaMensagens.Remove(m);
                }
            }
        }
        #endregion FIM | Static

        //#region Non Static
        //public string DecodeMessage() {
        //    return AlertMessage.GetMessage(_code);
        //}
        //#endregion FIM | Non Static
    }
}
