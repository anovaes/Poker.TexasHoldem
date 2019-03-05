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

        #region Mensagem de Erro Cartas
        public static string CartaFormatoInvalido = "O formato da carta não é válido.";
        public static string CartaValorInvalido = "O valor da carta não é válido.";
        public static string CartaNaipeInvalido = "O naipe da carta não é válido.";
        #endregion

        #region Mensagem de Erro Baralho
        public static string BaralhoQuantidadeDeValoresInvalido = "A quantidade de valores não é válida";
        public static string BaralhoQuantidadeDeNaipesInvalido = "A quantidade de naipes não é válida";
        public static string BaralhoContemIdsDeValoresDuplicados = "Foram informado valores de cartas duplicados";
        public static string BaralhoContemIdsDeNaipesDuplicados = "Foram informado naipes de cartas duplicados";
        #endregion

        #region Mensagem de Erro Mao
        public static string MaoCartaInvalida = "Carta inválida";
        public static string MaoCartasJogadorDuplicadas = "As cartas recebidas pelo jogador estão duplicadas";
        public static string MaoCartasClassificacaoDuplicadas = "As cartas utilizadas para realizar a classificação da jogada contém cartas duplicadas - Cartas duplicadas:";
        #endregion
    }
}
