using Poker.TexasHoldem.Test._Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Poker.TexasHoldem.Test
{
    public class BaralhoTeste
    {
        [Fact]
        public void DeveGerarBaralhoCompleto()
        {
            var quantidadeDeCartasEsperada = 52;
            var baralhoGerado = new Baralho();
            var quantidadeDeCartasGeradas = baralhoGerado.Cartas.Where(carta => carta != null).Count();

            Assert.Equal(quantidadeDeCartasEsperada, quantidadeDeCartasGeradas);
        }

        [Fact]
        public void GerarArquivoLogDoBaralho()
        {
            var nomeArquivo = @"C:\temp\logBaralho.txt";
            var habilitarGeracao = false;
            LogBaralho.Gerar(100, nomeArquivo, habilitarGeracao);

            Assert.True(File.Exists(nomeArquivo));
        }


        [Fact]
        public void DeveGerarBaralhoSemCartasRepetidas()
        {
            var quantidadeDeCartasDuplicadasEsperada = 0;
            var cartasDuplicadas = new Baralho().Cartas
                .GroupBy(carta => carta.Id)
                .Where(grupo => grupo.Count() > 1);

            Assert.Equal(quantidadeDeCartasDuplicadasEsperada, cartasDuplicadas.Count());

        }
    }

    public class Baralho
    {
        public List<Carta> Cartas { get; private set; }
        public Baralho()
        {
            var arrCartas = new Carta[52];
            var listNumerosRandom = new List<int>();
            var random = new Random();

            foreach (var valor in Ressource.BaralhoIdValores.Split(";"))
            {
                foreach (var naipe in Ressource.BaralhoIdNaipes.Split(";"))
                {
                    var numRandom = random.Next(0, 51);

                    while (listNumerosRandom.Where(num => num == numRandom).Any())
                    {
                        numRandom++;
                        if (numRandom > 51)
                            numRandom = 0;
                    }

                    arrCartas[numRandom] = new Carta($"{valor};{naipe}");

                    listNumerosRandom.Add(numRandom);
                }
            }

            Cartas = arrCartas.ToList();
        }
    }

    internal static class LogBaralho
    {
        internal static void Gerar(int iteracoes, string nomeArquivo, bool habilitarGeracao)
        {
            if (!habilitarGeracao)
                return;

            if (File.Exists(nomeArquivo))
                File.Delete(nomeArquivo);

            using (StreamWriter sw = new StreamWriter(nomeArquivo))
            {
                sw.WriteLine($"Inicio: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:fff")}");
                sw.WriteLine($"");

                for (int i = 1; i < iteracoes; i++)
                {
                    sw.WriteLine($"###################");
                    sw.WriteLine($"Bralho #{i} - Inicio: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:fff")}");

                    var baralho = new Baralho();

                    foreach (var item in baralho.Cartas)
                    {
                        sw.WriteLine($"{item.Valor.Id};{item.Naipe.Id}");
                    }

                    sw.WriteLine($"Bralho #{i} - Fim: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:fff")}");
                    sw.WriteLine($"###################");
                    sw.WriteLine($"");
                }

                sw.WriteLine($"");
                sw.WriteLine($"Fim: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss:fff")}");
            }
        }
    }
}
