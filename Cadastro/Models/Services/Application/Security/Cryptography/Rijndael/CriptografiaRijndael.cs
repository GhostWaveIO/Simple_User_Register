using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Cadastro.Models.Services.Application.Security.Cryptography.Rijndael {

    public class CriptografiaRijndael {


        #region Variáveis e Métodos Privados

        private int keyCifra = 7;
        public string ChaveAgoritimo { get; private set; }
        public readonly Dictionary<int, string> prefixos = new Dictionary<int, string>() {
        { 0, "lkmndc)" }
    };
        public bool ChaveObrigatoria { get; set; }

        #endregion

        #region Métodos Públicos

        public string Encrypt(string texto) {
            if(VerificarPrefixo(texto))
                return texto;

            string textoCifrado = Convert.ToBase64String(Encoding.UTF8.GetBytes(texto));
            return InserirPrefixo(textoCifrado);
        }

        public string Decrypt(string textoCriptografado) {
            if(!VerificarPrefixo(textoCriptografado))
                return textoCriptografado;

            string textoBase64 = RemoverPrefixo(textoCriptografado);

            try {
                byte[] data = Convert.FromBase64String(textoBase64);
                return Encoding.UTF8.GetString(data);
            } catch {
                return null;
            }
        }

        private string InserirPrefixo(string texto) {
            string prefixo = prefixos.Last().Value;
            if(!prefixos.Any(p => texto.StartsWith(p.Value)))
                texto = $"{prefixo}{texto}";
            return texto;
        }

        private string RemoverPrefixo(string texto) {
            foreach(var prefixo in prefixos.Values) {
                if(texto.StartsWith(prefixo)) {
                    return texto.Substring(prefixo.Length);
                }
            }
            return texto;
        }

        public bool VerificarPrefixo(string texto) {
            return !string.IsNullOrWhiteSpace(texto) && prefixos.Any(p => texto.StartsWith(p.Value));
        }

        #endregion
    }
}
