using Poker.TexasHoldem.Lib._Enum;
using System.Collections.Generic;

namespace Poker.TexasHoldem.Lib._Base
{
    public static class Ressource
    {
        #region Baralho
        public static string BaralhoIdValores = "2;3;4;5;6;7;8;9;10;Q;J;K;A";
        public static string BaralhoIdNaipes = "P;C;E;O";
        #endregion

        #region DePara Valor
        public static int ValorDoisPeso = 2;
        public static string ValorDoisNome = "DOIS";
        public static int ValorTresPeso = 3;
        public static string ValorTresNome = "TRÊS";
        public static int ValorQuatroPeso = 4;
        public static string ValorQuatroNome = "QUATRO";
        public static string ValorQuatroPlural = "QUATROS";
        public static int ValorCincoPeso = 5;
        public static string ValorCincoNome = "CINCO";
        public static string ValorCincoPlural = "CINCOS";
        public static int ValorSeisPeso = 6;
        public static string ValorSeisNome = "SEIS";
        public static int ValorSetePeso = 7;
        public static string ValorSeteNome = "SETE";
        public static string ValorSetePlural = "SETES";
        public static int ValorOitoPeso = 8;
        public static string ValorOitoNome = "OITO";
        public static string ValorOitoPlural = "OITOS";
        public static int ValorNovePeso = 9;
        public static string ValorNoveNome = "NOVE";
        public static string ValorNovePlural = "NOVES";
        public static int ValorDezPeso = 10;
        public static string ValorDezNome = "DEZ";
        public static int ValorValetePeso = 11;
        public static string ValorValeteNome = "VALETE";
        public static string ValorValetePlural = "VALETES";
        public static int ValorDamaPeso = 12;
        public static string ValorDamaNome = "DAMA";
        public static string ValorDamaPlural = "DAMAS";
        public static int ValorReiPeso = 13;
        public static string ValorReiNome = "REI";
        public static string ValorReiPlural = "REIS";
        public static int ValorAsAltoPeso = 14;
        public static int ValorAsBaixoPeso = 1;
        public static string ValorAsNome = "ÁS";
        public static string ValorAsPlural = "ASES";
        #endregion

        #region DePara Naipe
        public static string NaipePausNome = "PAUS";
        public static string NaipePausSimbolo = "♣";
        public static string NaipeCopasNome = "COPAS";
        public static string NaipeCopasSimbolo = "♥";
        public static string NaipeEspadasNome = "ESPADAS";
        public static string NaipeEspadasSimbolo = "♠";
        public static string NaipeOurosNome = "OUROS";
        public static string NaipeOurosSimbolo = "♦";
        #endregion

        #region Jogador
        public static int JogadorFichasInicial = 1000;
        public static StatusJogador JogadorStatusInicial = StatusJogador.Esperando;
        #endregion

        #region Mao
        public static string MaoClassificacaoRoyalFlush = "Royal Flush de #0#";
        public static string MaoClassificacaoStraightFlush = "Straight Flush de #0#, #1# a #2#";
        public static string MaoClassificacaoQuadra = "Quadra de #0#, kicker #1#";
        public static string MaoClassificacaoFullHouse = "Full House de #0# e #1#";
        public static string MaoClassificacaoFlush = "Flush de #0#. #1#, #2#, #3#, #4# e #5#";
        public static string MaoClassificacaoSequencia = "Sequência de #0# a #1#";
        public static string MaoClassificacaoTrinca = "Trinca de #0#, kickers #1# e #2#";
        public static string MaoClassificacaoDoisPares = "Dois pares de #0# e #1#, kicker #2#";
        public static string MaoClassificacaoPar = "Par de #0#, kickers #1#, #2# e #3#";
        public static string MaoClassificacaoCartaAlta = "Carta Alta de #0#, kickers #1#, #2#, #3# e #4#";
        #endregion

        #region Mesa
        public static int MesaQuantidadeMaximaDeJogadores = 9;
        #endregion

        #region Mensagem de Erro Cartas
        public static string CartaMsgFormatoInvalido = "O formato da carta não é válido.";
        public static string CartaMsgValorInvalido = "O valor da carta não é válido.";
        public static string CartaMsgNaipeInvalido = "O naipe da carta não é válido.";
        #endregion

        #region Mensagem de Erro Baralho
        public static string BaralhoMsgQuantidadeDeValoresInvalido = "A quantidade de valores não é válida";
        public static string BaralhoMsgQuantidadeDeNaipesInvalido = "A quantidade de naipes não é válida";
        public static string BaralhoMsgContemIdsDeValoresDuplicados = "Foram informado valores de cartas duplicados";
        public static string BaralhoMsgContemIdsDeNaipesDuplicados = "Foram informado naipes de cartas duplicados";
        #endregion

        #region Mensagem de Erro Mao
        public static string MaoMsgCartaInvalida = "Carta inválida";
        public static string MaoMsgCartasJogadorDuplicadas = "As cartas recebidas pelo jogador estão duplicadas";
        public static string MaoMsgCartasClassificacaoDuplicadas = "As cartas utilizadas para realizar a classificação da jogada contém cartas duplicadas - Cartas duplicadas:";
        #endregion

        #region Mensagem de Erro Jogador
        public static string JogadorMsgIdInvalido = "Não é permitido jogadores com id inferior a 1";
        public static string JogadorMsgNomeObrigatorio = "É obrigatório informar o nome do jogador";
        public static string JogadorMsgNomeSuperior20Caracteres = "O nome do jogador não pode ultrapassar os 20 caracteres";
        public static string JogadorMsgValorFichasGanhasInvalido = "Não é permitido receber quantidade de fichas negativas";
        #endregion

        #region Mesa
        public static int MesaValorInicialBlind = 2;
        #endregion

        #region Mensagem de Erro Mesa
        public static string MesaMsgIdInvalido = "Não é permitido mesas com id inferior a 1";
        public static string MesaMsgNaoPermitidoMudarStatusDeAtivaParaAguardando = "Não é permitido alterar o status da mesa para Aguardando quando a mesma já estiver ativa.";
        public static string MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAguardando = "Não é permitido alterar o status da mesa para Aguardando quando a mesma já estiver finalizada.";
        public static string MesaMsgNaoPermitidoMudarStatusDeFinalizadaParaAtiva = "Não é permitido alterar o status da mesa para Ativa quando a mesma já estiver finalizada.";
        public static string MesaMsgNaoPermitidoIniciarPartidaSemQuantidadeMinimaDeJogadores = "É necessário pelo menos dois jogadores para iniciar a partida.";
        public static string MesaMsgNaoPermitidoIniciarRodadaSemQuantidadeMinimaDeJogadores = "É necessário pelo menos dois jogadores para iniciar a rodada.";
        #endregion

        #region Mensagem de Sucesso Mesa
        public static string MesaMsgJogadorIncluidoComSucesso = "Jogador #0# sentou-se a mesa #1#";
        #endregion
    }
}
