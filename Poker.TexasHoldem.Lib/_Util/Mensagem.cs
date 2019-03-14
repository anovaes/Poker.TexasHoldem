using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Lib._Util
{
    public static class Mensagem
    {
        public static string GerarMensagem(string mensagem, params string[] complemento)
        {
            for (int i = 0; i < complemento.Length; i++)
            {
                mensagem = mensagem.Replace($"#{i.ToString()}#", complemento[i].ToString());
            }

            return mensagem;
        }
    }
}
