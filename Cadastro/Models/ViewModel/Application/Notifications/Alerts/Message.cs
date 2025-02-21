using System;

namespace Cadastro.Models.ViewModel.Application.Notifications.Alerts
{
    public class Message
    {

        public string key { get; set; }
        public string Texto { get; set; }
        public DateTime Criado { get; set; }

        public Message(string Texto, string Key)
        {
            this.Texto = Texto;
            key = Key;
            Criado = DateTime.Now;
        }


    }
}