using System;
using tabuleiro;
using xadrez;

namespace xadrezConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            PosicaoXadrez pos = new PosicaoXadrez('a', 1);

            Console.WriteLine(pos);

            Console.WriteLine(pos.ToPosicao());

        }
    }
}