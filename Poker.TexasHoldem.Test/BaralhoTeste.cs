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

        [Theory(DisplayName = "NaoDevePermitirQuantidadeDeValoresDeCartasInvalida")]
        [InlineData("2;3;4", "P;C;E;O")]
        [InlineData("2;3;4;5;6;7;8;9;10;11;12;13;14;15", "P;C;E;O")]
        [InlineData("", "P;C;E;O")]
        [InlineData("     ", "P;C;E;O")]
        [InlineData(null, "P;C;E;O")]
        public void NaoDevePermitirQuantidadeDeValoresDeCartasInvalida(string valoresInvalidos, string naipesValidos)
        {
            var mensasgemDeErro = Assert.Throws<Exception>(() => new Baralho(valoresInvalidos, naipesValidos)).Message;
            Assert.Equal(Ressource.BaralhoQuantidadeDeValoresInvalido, mensasgemDeErro);
        }

        [Theory(DisplayName = "NaoDevePermitirQuantidadeDeNaipesDeCartasInvalida")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "P;C")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "P;C;E;O;X")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "    ")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", null)]
        public void NaoDevePermitirQuantidadeDeNaipesDeCartasInvalida(string valoresValidos, string naipesInvalidos)
        {
            var mensasgemDeErro = Assert.Throws<Exception>(() => new Baralho(valoresValidos, naipesInvalidos)).Message;
            Assert.Equal(Ressource.BaralhoQuantidadeDeNaipesInvalido, mensasgemDeErro);
        }

        [Theory(DisplayName = "NaoDevePermitirIdDeValoresDuplicados")]
        [InlineData("2;3;4;4;6;7;8;9;10;Q;J;K;A", "P;C;E;O")]
        [InlineData("2;2;2;4;6;7;8;9;10;Q;J;K;A", "P;C;E;O")]
        public void NaoDevePermitirIdDeValoresDuplicados(string valoresContendoIdsDuplicados, string naipesValidos)
        {
            var mensasgemDeErro = Assert.Throws<Exception>(() => new Baralho(valoresContendoIdsDuplicados, naipesValidos)).Message;
            Assert.Equal(Ressource.BaralhoContemIdsDeValoresDuplicados, mensasgemDeErro);
        }

        [Theory(DisplayName = "NaoDevePermitirIdDeNaipesDuplicados")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "P;C;C;O")]
        [InlineData("2;3;4;5;6;7;8;9;10;Q;J;K;A", "P;C;C;C")]
        public void NaoDevePermitirIdDeNaipesDuplicados(string valoresValidos, string naipesContendoIdsDuplicados)
        {
            var mensasgemDeErro = Assert.Throws<Exception>(() => new Baralho(valoresValidos, naipesContendoIdsDuplicados)).Message;
            Assert.Equal(Ressource.BaralhoContemIdsDeNaipesDuplicados, mensasgemDeErro);
        }

        [Fact]
        public void DeveDistribuirApenasUmaCarta()
        {
            var quantidadeDeCartasEsperada = 51;
            var baralhoGerado = new Baralho();
            var cartaEntregue = baralhoGerado.DistribuirCarta();

            Assert.True(cartaEntregue != null);
            Assert.Equal(quantidadeDeCartasEsperada, baralhoGerado.Cartas.Where(carta => carta != null).Count());
        }

        [Fact]
        public void DeveRetirarDuasCartasDiferentesDoBaralho()
        {
            var quantidadeDeCartasEsperada = 50;
            var baralhoGerado = new Baralho();
            var PrimeiraCarta = baralhoGerado.DistribuirCarta();
            var SegundaCarta = baralhoGerado.DistribuirCarta();

            Assert.True(PrimeiraCarta.Id != SegundaCarta.Id);
            Assert.Equal(quantidadeDeCartasEsperada, baralhoGerado.Cartas.Where(carta => carta != null).Count());
        }
    }

    public class Baralho
    {
        public List<Carta> Cartas { get; private set; }
        public Baralho()
        {
            Embaralhar(Ressource.BaralhoIdValores, Ressource.BaralhoIdNaipes);
        }

        internal Baralho(string valores, string naipes)
        {
            Embaralhar(valores, naipes);
        }

        private void Embaralhar(string valores, string naipes)
        {
            var arrCartas = new Carta[52];
            var listNumerosRandom = new List<int>();
            var random = new Random();
            var arrValores = (valores ?? "").Split(";");
            var arrNaipes = (naipes ?? "").Split(";");

            if (arrValores.Length != 13)
                throw new Exception(Ressource.BaralhoQuantidadeDeValoresInvalido);

            if (arrNaipes.Length != 4)
                throw new Exception(Ressource.BaralhoQuantidadeDeNaipesInvalido);

            if (arrValores.GroupBy(valor => valor).Where(grupo => grupo.Count() > 1).Count() > 0)
                throw new Exception(Ressource.BaralhoContemIdsDeValoresDuplicados);

            if (arrNaipes.GroupBy(valor => valor).Where(grupo => grupo.Count() > 1).Count() > 0)
                throw new Exception(Ressource.BaralhoContemIdsDeNaipesDuplicados);

            foreach (var valor in arrValores)
            {
                foreach (var naipe in arrNaipes)
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

        public Carta DistribuirCarta()
        {
            var carta = Cartas.First();
            Cartas.Remove(carta);
            return carta;
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
