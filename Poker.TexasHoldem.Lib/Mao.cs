using Poker.TexasHoldem.Lib._Base;
using Poker.TexasHoldem.Lib._Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poker.TexasHoldem.Lib
{
    public class Mao
    {
        public List<Carta> Cartas { get; private set; }
        public string Classificacao { get; private set; }
        public long Pontuacao { get; private set; }

        private delegate bool MetodoVerificacao();
        private readonly List<MetodoVerificacao> _listVerificacao;
        private List<Carta> _cartasShowdown;

        /// <summary>
        /// Inicia uma instância de mão
        /// </summary>
        /// <param name="primeiraCarta">Primeira Carta</param>
        /// <param name="segundaCarta">Segunda Carta</param>
        public Mao(Carta primeiraCarta, Carta segundaCarta)
        {
            if (primeiraCarta == null || segundaCarta == null)
                throw new Exception(Ressource.MaoMsgCartaInvalida);

            if (primeiraCarta.Id == segundaCarta.Id)
                throw new Exception(Ressource.MaoMsgCartasJogadorDuplicadas);

            Classificacao = "";
            Cartas = new List<Carta>
            {
                primeiraCarta,
                segundaCarta
            };

            _listVerificacao = new List<MetodoVerificacao>
            {
                VerificarRoyalFlush,
                VerificarStraightFlush,
                VerificarQuadra,
                VerificarFullHouse,
                VerificarFlush,
                VerificarSequencia,
                VerificarTrinca,
                VerificarDoisPares,
                VerificarPar,
                VerificarCartaAlta
            };
        }

        /// <summary>
        /// Classifica e gera pontuação a jogada com base nas cartas do jogador e as cartas da mesa
        /// </summary>
        /// <param name="cartasMesa">Cartas da mesa. É esperado receber uma lista contendo cinco cartas</param>
        public void Classificar(List<Carta> cartasMesa)
        {
            _cartasShowdown = MontarShowdown(cartasMesa).OrderByDescending(carta => carta.Valor.Peso).ToList();

            var cartasDuplicadas = _cartasShowdown
                    .GroupBy(c => c.Id)
                    .Where(g => g.Count() > 1)
                    .Select(c => new { Id = c.Key });

            if (cartasDuplicadas.Any())
            {
                string idsDuplicados = null;

                foreach (var carta in cartasDuplicadas)
                {
                    idsDuplicados += idsDuplicados == null ? carta.Id : $"|{carta.Id}";
                }

                throw new Exception($"{Ressource.MaoMsgCartasClassificacaoDuplicadas} {idsDuplicados}");
            }

            foreach (var verificar in _listVerificacao)
            {
                if (verificar())
                    break;
            }
        }

        /// <summary>
        /// Monta uma lista de cartas juntando as cartas do jogador e da mesa
        /// </summary>
        /// <param name="cartasMesa">Cartas da Mesa</param>
        /// <returns>Retorna a lista de cartas do showdown</returns>
        private List<Carta> MontarShowdown(List<Carta> cartasMesa)
        {
            List<Carta> cartasDaJogada = new List<Carta>();

            foreach (var carta in Cartas)
            {
                cartasDaJogada.Add(carta);
            }

            foreach (var carta in cartasMesa)
            {
                cartasDaJogada.Add(carta);
            }

            return cartasDaJogada;
        }

        /// <summary>
        /// Verifica se a mão possui um RoyalFlush
        /// </summary>
        /// <returns>Retorna true caso houver Royal Flush na jogada, caso contrário false</returns>
        private bool VerificarRoyalFlush()
        {
            try
            {
                var grupo = _cartasShowdown.Take(5).GroupBy(carta => carta.Naipe.Nome);

                if (_cartasShowdown[0].Valor.Peso == 14 &&
                    _cartasShowdown[1].Valor.Peso == 13 &&
                    _cartasShowdown[2].Valor.Peso == 12 &&
                    _cartasShowdown[3].Valor.Peso == 11 &&
                    _cartasShowdown[4].Valor.Peso == 10 &&
                    grupo.Count() == 1)
                {
                    Classificacao = $"Royal Flush de {grupo.First().Key}";
                    Pontuacao = 91413121110;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Royal Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Straight Flush
        /// </summary>
        /// <returns>Retorna true caso houver Straight Flush na jogada, caso contrário false</returns>
        private bool VerificarStraightFlush()
        {
            try
            {
                (string Id, string Nome) flush = AgruparNaipe(_cartasShowdown);

                if (flush.Id == null)
                    return false;

                return IsSequencia(_cartasShowdown.Where(c => c.Naipe.Id == flush.Id).ToList(), $"Straight Flush de {flush.Nome},", "8");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Straight Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Quadra
        /// </summary>
        /// <returns>Retorna true caso houver Quadra na jogada, caso contrário false</returns>
        private bool VerificarQuadra()
        {
            try
            {
                (string Id, string Plural, string Peso) quadra = AgruparValor(_cartasShowdown, 4, null);

                if (quadra.Id != null)
                {
                    (string Nome, string Peso) kicker = ObterKickers(_cartasShowdown, quadra.Id, 1).First();
                    Classificacao = $"Quadra de {quadra.Plural}, kicker {kicker.Nome}";
                    Pontuacao = long.Parse($"7{Texto.Repeat(quadra.Peso, 4)}{kicker.Peso}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Quadra - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um FullHouse
        /// </summary>
        /// <returns>Retorna true caso houver Full House na jogada, caso contrário false</returns>
        private bool VerificarFullHouse()
        {
            try
            {
                (string Id, string Plural, string Peso) trinca = AgruparValor(_cartasShowdown, 3, null);

                if (trinca.Id == null)
                    return false;

                (string Id, string Plural, string Peso) par = AgruparValor(_cartasShowdown, 2, trinca.Id);

                if (par.Id != null)
                {
                    Classificacao = $"Full House de {trinca.Plural} e {par.Plural}";
                    Pontuacao = long.Parse($"6{Texto.Repeat(trinca.Peso, 3)}{Texto.Repeat(par.Peso, 2)}");
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Full House - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Flush
        /// </summary>
        /// <returns>Retorna true caso houver Flush na jogada, caso contrário false</returns>
        private bool VerificarFlush()
        {
            try
            {
                (string Id, string Nome) flush = AgruparNaipe(_cartasShowdown);

                if (flush.Id == null)
                    return false;

                int contador = 1;
                string pontuacao = "";

                foreach (var carta in _cartasShowdown.Where(c => c.Naipe.Id == flush.Id).Take(5))
                {
                    pontuacao += carta.Valor.PesoTexto;

                    if (contador == 1)
                        Classificacao = $"Flush de {flush.Nome}. {carta.Valor.Nome}";
                    else if (contador < 5)
                        Classificacao += $", {carta.Valor.Nome}";
                    else
                        Classificacao += $" e {carta.Valor.Nome}";

                    contador++;
                }

                Pontuacao = long.Parse($"5{pontuacao}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Flush - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Sequência
        /// </summary>
        /// <returns>Retorna true caso houver Sequência na jogada, caso contrário false</returns>
        private bool VerificarSequencia()
        {
            try
            {
                return IsSequencia(_cartasShowdown.Take(7).ToList(), "Sequência de", "4");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Sequência - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui uma Trinca
        /// </summary>
        /// <returns>Retorna true caso houver Trinca na jogada, caso contrário false</returns>
        private bool VerificarTrinca()
        {
            try
            {
                (string Id, string Plural, string Peso) trinca = AgruparValor(_cartasShowdown, 3, null);

                if (trinca.Id == null)
                    return false;

                (string Nome, string Peso)[] kickers = ObterKickers(_cartasShowdown, trinca.Id, 2);

                Classificacao = $"Trinca de {trinca.Plural}, kickers {kickers[0].Nome} e {kickers[1].Nome}";
                Pontuacao = long.Parse($"3{Texto.Repeat(trinca.Peso, 3)}{string.Concat(kickers[0].Peso, kickers[1].Peso)}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Trinca - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui dois Pares
        /// </summary>
        /// <returns>Retorna true caso houver Dois Pares na jogada, caso contrário false</returns>
        private bool VerificarDoisPares()
        {
            try
            {
                (string Id, string Plural, string Peso) par1 = AgruparValor(_cartasShowdown, 2, null);

                if (par1.Id == null)
                    return false;

                (string Id, string Plural, string Peso) par2 = AgruparValor(_cartasShowdown, 2, par1.Id);

                if (par2.Id == null)
                    return false;

                var kicker = _cartasShowdown
                    .Where(c => c.Valor.Id != par1.Id && c.Valor.Id != par2.Id)
                    .First();

                Classificacao = $"Dois pares de {par1.Plural} e {par2.Plural}, kicker {kicker.Valor.Nome}";
                Pontuacao = long.Parse($"2{Texto.Repeat(par1.Peso, 2)}{Texto.Repeat(par2.Peso, 2)}{kicker.Valor.PesoTexto}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Dois Pares - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui um Par
        /// </summary>
        /// <returns>Retorna true caso houver Par na jogada, caso contrário false</returns>
        private bool VerificarPar()
        {
            try
            {
                (string Id, string Plural, string Peso) par = AgruparValor(_cartasShowdown, 2, null);

                if (par.Id == null)
                    return false;

                (string Nome, string Peso)[] kickers = ObterKickers(_cartasShowdown, par.Id, 3);

                Classificacao = $"Par de {par.Plural}, kickers {kickers[0].Nome}, {kickers[1].Nome} e {kickers[2].Nome}";
                Pontuacao = long.Parse($"1{Texto.Repeat(par.Peso, 2)}{string.Concat(kickers[0].Peso, kickers[1].Peso, kickers[2].Peso)}");
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Par - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se a mão possui Carta Alta
        /// </summary>
        /// <returns>Retorna true caso houver Carta Alta na jogada, caso contrário false</returns>
        private bool VerificarCartaAlta()
        {
            try
            {
                var pontuacao = "";
                var contador = 1;

                foreach (var carta in _cartasShowdown.Take(5).ToList())
                {
                    pontuacao += carta.Valor.PesoTexto;

                    if (contador == 1)
                        Classificacao = $"Carta Alta de {carta.Valor.Nome}, kickers ";
                    else if (contador == 2)
                        Classificacao += $"{carta.Valor.Nome}";
                    else if (contador < 5)
                        Classificacao += $", {carta.Valor.Nome}";
                    else
                        Classificacao += $" e {carta.Valor.Nome}";

                    contador++;
                }

                Pontuacao = long.Parse(pontuacao);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao Verificar Carta Alta - {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Realiza o agrupamento da quantidade de valores iguais informada
        /// </summary>
        /// <param name="cartas">Cartas disponíveis na jogada</param>
        /// <param name="quantidadeMinima">Quantidade de cartas mínima para gerar um grupo</param>
        /// <param name="idIgnorado">Id da carta que será ignorado no agrupamento</param>
        /// <returns>Tupla contendo Id, Nome no Plural e o Peso do valor que foi agrupado. Caso não seja formado um grupo com a quantidade mínima de cartas, 
        /// será retornado nulo</returns>
        private static (string Id, string Plural, string Peso) AgruparValor(List<Carta> cartas, int quantidadeMinima, string idIgnorado)
        {
            return cartas
                .GroupBy(c => new { c.Valor.Id, c.Valor.Plural, c.Valor.PesoTexto })
                .Where(g => (idIgnorado == null || g.Key.Id != idIgnorado) && g.Count() >= quantidadeMinima)
                .Select(c =>
                    (
                        Id: c.Key.Id,
                        Plural: c.Key.Plural,
                        Peso: c.Key.PesoTexto
                    )
                )
                .FirstOrDefault();
        }

        /// <summary>
        /// Realiza o agrupamento de cinco cartas com naipes iguais
        /// </summary>
        /// <param name="cartas">Cartas disponíveis na jogada</param>
        /// <returns>Tupla contendo Id e Nome do naipe que foi agrupado. Caso não seja formado um grupo com a quantidade de cinco cartas, 
        /// será retornado nulo</returns>
        private static (string Id, string Nome) AgruparNaipe(List<Carta> cartas)
        {
            return cartas
                .GroupBy(c => new { c.Naipe.Id, c.Naipe.Nome })
                .Where(g => g.Count() >= 5)
                .Select(c =>
                    (
                        Id: c.Key.Id,
                        Nome: c.Key.Nome
                    )
                )
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtêm as cartas kickers 
        /// </summary>
        /// <param name="cartas"> Cartas disponíveis na jogada</param>
        /// <param name="idIgnorado">Id da carta que será ignorado no agrupamento</param>
        /// <param name="quantidadeDeCartas">Quantidade de cartas utilizadas como kicker</param>
        /// <returns>Retorna uma lista de tupla contendo o Nome e o Peso das cartas kickers</returns>
        private (string Nome, string Peso)[] ObterKickers(List<Carta> cartas, string idIgnorado, int quantidadeDeCartas)
        {
            return _cartasShowdown
                .Where(c => c.Valor.Id != idIgnorado)
                .Take(quantidadeDeCartas)
                .Select(c => (Nome: c.Valor.Nome, Peso: c.Valor.PesoTexto))
                .ToArray();
        }

        /// <summary>
        /// Realiza a verificação de sequência. Utilizada nos casos de Straight Flush e Sequêcia
        /// </summary>
        /// <param name="cartas">Cartas válidas</param>
        /// <param name="raizClassificacao">Raíz da mensagem de classificação</param>
        /// <param name="raizPontuacao">Raiz da pontuação</param>
        /// <returns>Se for uma sequência, retorna true. Caso contrário, false </returns>
        private bool IsSequencia(List<Carta> cartas, string raizClassificacao, string raizPontuacao)
        {
            string menorValor = null;
            string maiorValor = null;
            Carta cartaAnterior = null;
            string pontuacao = "";
            int intervalos = 0;
            int quantidadeDeCartas = 0;

            // Caso houver um Ás entre as cartas será adicionada uma nova carta, mas com o peso = 1
            if (cartas.Where(c => c.Valor.Id == "A").Any())
            {
                string naipe = "";
                cartas.ForEach(c =>
                {
                    if (c.Valor.Id == "A")
                        naipe = c.Naipe.Id;
                });

                Carta carta = new Carta($"A;{naipe}");
                carta.Valor.TrocarPeso();
                cartas.Add(carta);
                intervalos--;
            }

            foreach (var cartaAtual in cartas)
            {
                var diferencaDeValor = cartaAnterior?.Valor?.Peso - cartaAtual.Valor.Peso;
                if (diferencaDeValor > 1)
                {
                    pontuacao = "";
                    maiorValor = null;
                    quantidadeDeCartas = 0;

                    //Será incrementado um intervalo sermpre que houver uma diferença maior que 1 do peso entre as cartas.
                    //Caso exista Ás já foi retirado um do intervalo, pois ainda será analisado como Ás baixo
                    intervalos++;

                    //Com 3 intervalos maiores que 1 já não é mais possível realizar uma sequência
                    if (intervalos == 3)
                        break;
                }
                else if (diferencaDeValor == 0)
                    continue;

                pontuacao += cartaAtual.Valor.Peso.ToString().PadLeft(2, '0');
                maiorValor = maiorValor ?? cartaAtual.Valor.Nome;
                menorValor = cartaAtual.Valor.Nome;
                cartaAnterior = cartaAtual;

                //Necessário ter cinco cartas na sequência para ocorrer a sequência
                quantidadeDeCartas++;
                if (quantidadeDeCartas == 5)
                    break;
            }

            if (quantidadeDeCartas == 5)
            {
                Classificacao = $"{raizClassificacao} {menorValor} a {maiorValor}";
                Pontuacao = long.Parse($"{raizPontuacao}{pontuacao}");
                return true;
            }
            else
                return false;
        }
    }
}
