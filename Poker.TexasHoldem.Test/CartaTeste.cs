using ExpectedObjects;
using System;
using System.Collections.Generic;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class CartaTeste
    {
        [Fact]
        public void DeveCriarUmaCarta()
        {
            var idCarta = "A;C";
            var arrCarta = idCarta.Split(";");

            var cartaEsperada = new
            {
                Id = idCarta,
                Valor = new Valor(arrCarta[0]),
                Naipe = new Naipe(arrCarta[1])
            };

            var cartaGerada = new Carta(idCarta);

            cartaEsperada.ToExpectedObject().ShouldMatch(cartaGerada);
        }

        [Theory(DisplayName = "DeveVerificarDescricaoDasCartas")]
        [InlineData("10;P", "DEZ", "PAUS")]
        [InlineData("A;C", "�S", "COPAS")]
        [InlineData("J;O", "VALETE", "OUROS")]
        [InlineData("8;E", "OITO", "ESPADAS")]
        public void DeveVerificarDescricaoDasCartas(string idCarta, string NomeValorEsperado, string NomeNaipeEsperado)
        {
            var cartaGerada = new Carta(idCarta);

            Assert.Equal(NomeValorEsperado, cartaGerada.Valor.Nome);
            Assert.Equal(NomeNaipeEsperado, cartaGerada.Naipe.Nome);
        }

        [Theory(DisplayName ="NaoDevePermitirCartaComFormatoInvalido")]
        [InlineData("10")]
        [InlineData(";P")]
        [InlineData("10;")]
        [InlineData(";;")]
        [InlineData(";10;E;")]
        [InlineData("8C;")]
        [InlineData(";8C")]
        [InlineData("   ;   ")]
        [InlineData("      ")]
        [InlineData(null)]
        public void NaoDevePermitirCartaComFormatoInvalido(string cartaComFormatoInvalido)
        {
            var mensagem = Assert.Throws<Exception>(() => new Carta(cartaComFormatoInvalido)).Message;
            Assert.Equal("O formato da carta n�o � v�lido.", mensagem);
        }
    }

    public class Carta
    {
        public string Id { get; private set; }
        public Valor Valor { get; private set; }
        public Naipe Naipe { get; private set; }

        /// <summary>
        /// Inicia uma inst�ncia de carta.
        /// </summary>
        /// <param name="idCarta">Id da carta.</param>
        public Carta(string idCarta)
        {
            var arrayCarta = idCarta?.Split(';');

            if (!ValidaFormato(arrayCarta))
                throw new Exception("O formato da carta n�o � v�lido.");

            Id = idCarta;
            Valor = new Valor(arrayCarta[0].ToUpper());
            Naipe = new Naipe(arrayCarta[1].ToUpper());

        }

        /// <summary>
        /// Verifica se o formato da carta � v�lido
        /// </summary>
        /// <param name="arrayCarta">Array contendo o id do valor e o id do naipe</param>
        /// <returns>Se o formato da carta for v�lido retorna true, caso contr�rio falso</returns>
        private static bool ValidaFormato(string[] arrayCarta)
        {
            return arrayCarta?.Length == 2 && !string.IsNullOrWhiteSpace(arrayCarta[0]) && !string.IsNullOrWhiteSpace(arrayCarta[1]);
        }

    }
}