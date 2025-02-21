using Cadastro.Data;
using Cadastro.Models.Account.Cadastro.Campos;
using Cadastro.Models.Account.Cadastro.Dados;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Form;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cadastro.Models.ViewModel.Account.Cadastro
{
    public class CadastroUsuarioVM
    {

        public AppDbContext _context { get; set; }

        public CadastroUsuarioVM() { }
        public CadastroUsuarioVM(AppDbContext context)
        {
            _context = context;
        }


        public Usuario usuario { get; set; }
        public List<Grupo_Cadastro> grupos { get; set; }

        public string Indicacao { get; set; }


        public List<Dado_Cadastro> GetListaDados()
        {
            List<Dado_Cadastro> res = new List<Dado_Cadastro>();


            if (grupos == null) return new List<Dado_Cadastro>();
            Dado_Cadastro dadoEmailCpf = grupos[0].Linhas[0]?.Campos[0]?.Dados[0];//Coleta Email e Cpf genéricos do primeiro dado
            foreach (Grupo_Cadastro grupo in grupos)
            {
                foreach (Linha_Cadastro linha in grupo.Linhas)
                {
                    if (linha.Campos == null) continue;
                    foreach (Campo_Cadastro campo in linha.Campos)
                    {
                        if (campo.Dados == null) continue;
                        foreach (Dado_Cadastro dado in campo.Dados)
                        {
                            dado.Campo = campo;

                            //Desloca Email e Cpf genéricos para o dado original
                            if (dadoEmailCpf != null && (!string.IsNullOrEmpty(dadoEmailCpf?.Email_Novo) || !string.IsNullOrEmpty(dadoEmailCpf?.Email_Generico)) && dado.Campo.CampoGenerico == ECampoGenerico.Email)
                            {
                                dado.Email_Generico = dadoEmailCpf.Email_Novo ?? dadoEmailCpf.Email_Generico;
                            }
                            else if (dadoEmailCpf != null && !string.IsNullOrEmpty(dadoEmailCpf?.Cpf_Generico) && dado.Campo.CampoGenerico == ECampoGenerico.CPF)
                                dado.Cpf_Generico = dadoEmailCpf.Cpf_Generico;

                            res.Add(dado);
                        }
                    }
                }
            }

            return res;
        }


        //#### Antigo ####

        public string msg { get; set; }//Colocar msgResult
        public string msgResult { get; set; }//Colocar msgResult
        public string key { get; set; }
        public string UrlConfirmacao { get; set; }


        //############################################################################################################
        public async Task<Usuario> GetUsuarioByEmail(string email)
        {
            Dado_Cadastro dadoEmail = await _context.DadosCadastro.
              Include(d => d.Usuario).ThenInclude(u => u.EmailsValidados).
              Include(d => d.Usuario).ThenInclude(u => u.Dados).ThenInclude(d => d.Campo).
              Include(d => d.Campo).
              FirstOrDefaultAsync(d => d.Campo.CampoGenerico == ECampoGenerico.Email && d.Email == email);

            return dadoEmail.Usuario;
        }

        //######################################################################################
        public async Task<bool> verificarEmailExists(string emailNovo, string email, long dadoId = 0)
        {

            string supostoEmail = emailNovo ?? email;

            if (supostoEmail?.Trim() == Program.Config.emailPrincipal) return true;

            bool res = await _context.DadosCadastro.
              Include(d => d.Campo).
              Include(d => d.Usuario).
              AnyAsync(d => d.Campo.CampoGenerico == ECampoGenerico.Email && d.Email == supostoEmail && (dadoId == 0 || d.Dado_CadastroId != dadoId));


            return res || email == Program.Config.emailPrincipal ? true : false;
        }

        //######################################################################################
        public async Task<bool> verificarCpfExists(string cpf, long dadoId = 0)
        {

            bool res = await _context.DadosCadastro.
              Include(d => d.Campo).
              Include(d => d.Usuario).
              AnyAsync(d => d.Campo.CampoGenerico == ECampoGenerico.CPF && d.Texto250.Replace(".", "").Replace(",", "").Replace("-", "").Replace("_", "") == cpf.Replace(".", "").Replace(",", "").Replace("-", "").Replace("_", "") && (dadoId == 0 || d.Dado_CadastroId != dadoId));


            return res;
        }
    }


}
