using Cadastro.Models.Account.Cadastro.Campos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadastro.Models.ViewModel.Account
{
    public static class CampoCadastroUtilitiesVM
    {


        //##################################################################################################################################
        public static IEnumerable<KeyValuePair<int, string>> GetColecaoModelosCampos()
        {

            IEnumerable<KeyValuePair<int, string>> res = (from EModeloCampo model in Enum.GetValues(typeof(EModeloCampo))
                                                          where model != EModeloCampo.Senha &&
                                                          model != EModeloCampo.Confirmar_Senha &&
                                                          model != EModeloCampo.MultiSelect &&
                                                          model != EModeloCampo.Vídeo_Youtube
                                                          select new KeyValuePair<int, string>((int)model, Enum.GetName(typeof(EModeloCampo), model).Replace('_', ' '))).OrderBy(m => m.Value);

            return res;
        }

        //##################################################################################################################################
        public static IEnumerable<KeyValuePair<int, string>> GetColecaoAutorizadoEditar()
        {

            IEnumerable<KeyValuePair<int, string>> res = from EAutorizadoEditar pe in Enum.GetValues(typeof(EAutorizadoEditar))
                                                         select new KeyValuePair<int, string>((int)pe, Enum.GetName(typeof(EAutorizadoEditar), pe).Replace('_', ' '));

            return res;
        }

        //##################################################################################################################################
        public static IEnumerable<KeyValuePair<int, string>> GetColecaoStartCriacaoCampos()
        {
            IEnumerable<KeyValuePair<int, string>> res = from EStartCriacaoCampo scp in Enum.GetValues(typeof(EStartCriacaoCampo))
                                                         select new KeyValuePair<int, string>((int)scp, Enum.GetName(typeof(EStartCriacaoCampo), scp));

            return res;
        }

        //##################################################################################################################################
        public static IEnumerable<KeyValuePair<int, string>> GetColecaoDirecaoItens()
        {

            IEnumerable<KeyValuePair<int, string>> res = from EDirecaoEixo dir in Enum.GetValues(typeof(EDirecaoEixo))
                                                         select new KeyValuePair<int, string>((int)dir, Enum.GetName(typeof(EDirecaoEixo), dir));

            return res;
        }

    }
}
