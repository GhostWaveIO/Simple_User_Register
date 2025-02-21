namespace Cadastro.Models.Services.Application.Security.Cryptography.Rijndael {
    public class Criptografia {

        private int keyCifra = 7;
        private string prefixo = "_#f@";

        //########################################################################################################################
        public string Cifrar(string texto) {
            string textoCifrado = string.Empty;
            if(VerificarPrefixo(texto) || string.IsNullOrEmpty(texto))
                return texto;

            foreach(char c in texto) {
                textoCifrado += char.ConvertFromUtf32(c + keyCifra);
            }

            return InserirPrefixo(textoCifrado);
        }

        //########################################################################################################################
        public string Descifrar(string texto) {
            string textoCifrado = string.Empty;

            if(!VerificarPrefixo(texto) || string.IsNullOrEmpty(texto))
                return texto;

            foreach(char c in RemoverPrefixo(texto)) {
                textoCifrado += char.ConvertFromUtf32(c - keyCifra);
            }

            return textoCifrado;
        }

        //########################################################################################################################
        private string InserirPrefixo(string texto) {
            if(!texto.StartsWith(prefixo))
                texto = $"{prefixo}{texto}";

            return texto;
        }

        //########################################################################################################################
        private string RemoverPrefixo(string texto) {
            if(texto.StartsWith(prefixo))
                texto = texto.Substring(prefixo.Length);

            return texto;
        }

        //########################################################################################################################
        public bool VerificarPrefixo(string texto) {
            bool res = false;

            if(string.IsNullOrEmpty(texto))
                return false;
            if(texto.StartsWith(prefixo))
                res = true;

            return res;
        }

    }
}
