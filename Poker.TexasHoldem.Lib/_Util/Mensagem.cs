using System;
using System.Collections.Generic;
using System.Text;

namespace Poker.TexasHoldem.Lib._Util
{
    public static class Mensagem
    {
        /// <summary>
        /// Customiza a mensagem com base nos parâmetros que são passados. Os templates de mensagem devem sempre começar por #0#
        /// </summary>
        /// <param name="mensagem">Template de mensagem</param>
        /// <param name="complemento">Array de complementos que substituirão o template da mensagem</param>
        /// <returns>Retorna a mensagem customizada</returns>
        public static string Gerar(string mensagem, params string[] complemento)
        {
            for (int i = 0; i < complemento.Length; i++)
            {
                mensagem = mensagem.Replace($"#{i.ToString()}#", complemento[i].ToString());
            }

            return mensagem;
        }
    }
}
