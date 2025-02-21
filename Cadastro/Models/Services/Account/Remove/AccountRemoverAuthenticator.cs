using Cadastro.Data;
using Cadastro.Models.Entities.DB.Account;
using Cadastro.Models.Entities.DB.Account.Remove;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Account.Remove
{
    /// <summary>Responsável por gerenciar as autenticação de solicitação de remoção de conta de um usuário</summary>
    public class AccountRemoverAuthenticator
    {

        public AccountRemoverAuthenticator(Usuario usuario, AppDbContext context)
        {
            _usuario = usuario;
            _context = context;
        }

        private AppDbContext _context { get; set; }
        private Usuario _usuario { get; set; }

        /// <summary>
        /// Gera uma solicitação de remoção de conta no banco de dados para que o usuário seja enviado para a próxima página
        /// no próximo passo esta requisição gerada será usada para identificar de qual seção o usuário veio
        /// </summary>
        /// <returns>Retorna a autenticação gerada</returns>
        public async Task<AuthToRemove> GenerateAuthAsync()
        {
            AuthToRemove auth = new AuthToRemove(_usuario.UsuarioId);
            auth.GenerateCode();

            IEnumerable<AuthToRemove> currentAuths = await CollectAllAuthsFromUserAsync();
            if (currentAuths.Any()) _context.RemoveRange(currentAuths);
            await _context.AuthsToRemove.AddAsync(auth);
            await _context.SaveChangesAsync();

            return auth;
        }

        /// <summary>
        /// Coleta a autenticação que foi gerada em "GenerateAuth()"
        /// </summary>
        /// <param name="code">este é o código que foi gerado na hora da geração da solicitação</param>
        /// <returns>Retorna a autenticação</returns>
        /// <exception cref="Exception">Caso não tenha sido informado nenhuma autenticação uma exceção será gerada</exception>
        public async Task<AuthToRemove> CollectAuthAsync(string code)
        {
            AuthToRemove auth = await _context.AuthsToRemove.FirstOrDefaultAsync(a => a.Code == code);
            if (auth == null) throw new Exception("Autenticação não encontrada!");

            return auth;
        }

        /// <summary>
        /// Aprova a autenticação que tinha sido gerada em outro contexto usando o método "GenerateAuth()"
        /// </summary>
        /// <param name="resuqest">Código da autenticação informada</param>
        public async Task ApproveAuthAsync(string code)
        {
            AuthToRemove auth = await CollectAuthAsync(code);

            auth.Approved = true;
            _context.AuthsToRemove.Update(auth);
            await _context.SaveChangesAsync();
        }

        /// <summary>Remove todas autenticações do usuário em questão</summary>
        public async Task RemoveAllAuthsFromUser()
        {
            IEnumerable<AuthToRemove> currentList = await CollectAllAuthsFromUserAsync();
            _context.RemoveRange(currentList);
            await _context.SaveChangesAsync();
        }



        //    EmailSender sender = new EmailSender(subject, body.ToString(), to);
        //}

        /// <summary>Coleta todas autenticações do usuário em questão</summary>
        /// <returns>Retorna todas autenticações do usuário em questão</returns>
        private async Task<IEnumerable<AuthToRemove>> CollectAllAuthsFromUserAsync()
        {
            IEnumerable<AuthToRemove> currentAuths = await _context.AuthsToRemove.Where(a => a.UsuarioId == _usuario.UsuarioId).ToListAsync();
            return currentAuths;
        }
    }
}
