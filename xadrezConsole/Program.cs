using System;
using tabuleiro;
using xadrez;

namespace xadrezConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Tabuleiro tab = new Tabuleiro(8, 8);

            tab.ColocarPeca(new Torre(tab, Cor.preta), new Posicao(0, 0));
            tab.ColocarPeca(new Torre(tab, Cor.preta), new Posicao(1, 3));
            tab.ColocarPeca(new Rei(tab, Cor.preta), new Posicao(2, 4));

            Tela.ImprimirTabuleiro(tab);

        }
    }
}