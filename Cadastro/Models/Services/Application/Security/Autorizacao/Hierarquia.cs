using Cadastro.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cadastro.Models.Services.Application.Security.Autorizacao
{
    public class Hierarquia
    {
        public int HierarquiaId { get; set; }
        public AppDbContext context { get; set; }

        public Hierarquia() { }
        public Hierarquia(AppDbContext context) { this.context = context; }


        //Título
        [Required(ErrorMessage = "Este campo é obrigatório")]
        [StringLength(150, ErrorMessage = "Máximo {1} caracteres")]
        public string Titulo { get; set; }
        public int Ordem { get; set; }

        public PermissaoHierarquia PermissaoHierarquia { get; set; }

        public IEnumerable<HierarquiaUsuario> HierarquiasUsuario { get; set; }
        public IEnumerable<FuncaoUsuario> FuncoesUsuario { get; set; }

        //#############################################################################################################################
        public void CopyTo(Hierarquia hierarquia)
        {
            hierarquia.Titulo = Titulo;
        }

        //##################################################################################################################################
        public async Task SubirOrdem(int id)
        {
            if (!await context.Hierarquias.AnyAsync()) return;
            if (await context.Hierarquias.AnyAsync(h => h.Ordem == 0))
            {
                await ReposicionarOrdem();
                return;
            }

            List<Hierarquia> fila = new List<Hierarquia>(await context.Hierarquias?.OrderBy(h => h.Ordem).ToListAsync() ?? new List<Hierarquia>());
            for (int c = 0; c < fila.Count(); c++)
            {
                if (c + 1 >= fila.Count())
                {//Se não ouver mais abaixo
                    fila[c].Ordem = c + 1;
                }
                else
                {
                    if (fila[c + 1].HierarquiaId == id)
                    {
                        fila[c].Ordem = c + 2;
                        fila[c + 1].Ordem = ++c;
                    }
                    else
                    {
                        fila[c].Ordem = c + 1;
                    }
                }
            }

            context.Hierarquias.UpdateRange(context.Hierarquias);
            await context.SaveChangesAsync();
        }

        //##################################################################################################################################
        public async Task DescerOrdem(int id)
        {
            if (!await context.Hierarquias.AnyAsync()) return;
            if (await context.Hierarquias.AnyAsync(h => h.Ordem == 0))
            {
                await ReposicionarOrdem();
                return;
            }

            List<Hierarquia> fila = new List<Hierarquia>(await context.Hierarquias?.OrderBy(h => h.Ordem).ToListAsync() ?? new List<Hierarquia>());
            for (int c = 0; c < fila.Count(); c++)
            {
                if (fila[c].HierarquiaId == id)
                {
                    if (c + 1 >= fila.Count())
                    {
                        fila[c].Ordem = c + 1;
                    }
                    else
                    {
                        fila[c].Ordem = c + 2;
                        fila[c + 1].Ordem = ++c;
                    }
                }
                else
                {
                    fila[c].Ordem = c + 1;
                }
            }

            context.Hierarquias.UpdateRange(context.Hierarquias);
            context.SaveChanges();
        }

        //##################################################################################################################################
        public async Task ReposicionarOrdem()
        {
            if (!await context.Hierarquias?.AnyAsync()) return;

            int c = 1;
            foreach (Hierarquia h in await context.Hierarquias?.OrderBy(h => h.Ordem).ToListAsync() ?? new List<Hierarquia>())
                h.Ordem = c++;

            context.Hierarquias.UpdateRange(await Task.FromResult(context.Hierarquias));
            await context.SaveChangesAsync();
        }
    }
}
