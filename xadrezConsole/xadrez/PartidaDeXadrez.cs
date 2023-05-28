using System.Collections.Generic;
using tabuleiro;
using Xadrez;
using xadrezConsole.Xadrez;

namespace xadrez
{
    class PartidaDeXadrez
    {
        public Tabuleiro tab { get; private set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas;
        private HashSet<Peca> Capturadas;
        public bool Xeque { get; private set; }

        public PartidaDeXadrez()
        {
            tab = new Tabuleiro(8, 8);
            Turno = 1;
            JogadorAtual = Cor.branca;
            Terminada = false;
            Xeque = false;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            ColocarPecas();
        }

        public Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = tab.RetirarPeca(origem);
            p.IncrementarQtdMovimentos();
            Peca pecaCapturada = tab.RetirarPeca(destino);
            tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }
            return pecaCapturada;
        }
        public void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = tab.RetirarPeca(destino);
            p.DecrementarQtdMovimentos();
            if (pecaCapturada != null)
            {
                tab.ColocarPeca(pecaCapturada, destino);
                Capturadas.Remove(pecaCapturada);
            }
            tab.ColocarPeca(p, origem);
        }
        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);

            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            if (EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            }
            else
            {
                Xeque = false;
            }
            if (TesteXequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                Turno++;
                MudaJogador();
            }



        }
        public void ValidarPosicaoDeOrigem(Posicao pos)
        {
            if (tab.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na origem");
            }
            if (JogadorAtual != tab.Peca(pos).Cor)
            {
                throw new TabuleiroException("a peça de origem não corresponde a cor de quem está jogando!");
            }
            if (!tab.Peca(pos).ExisteMovimentosPossiveis())
            {
                throw new TabuleiroException("Não há movimentos possiveis para a peça de origem escolhida!");
            }
        }
        public void ValidarPosicaoDeDestino(Posicao origem, Posicao destino)
        {
            if (!tab.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida");
            }
        }
        private void MudaJogador()
        {
            if (JogadorAtual == Cor.branca)
            {
                JogadorAtual = Cor.preta;
            }
            else
            {
                JogadorAtual = Cor.branca;
            }
        }
        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in Capturadas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            return aux;
        }
        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca x in Pecas)
            {
                if (x.Cor == cor)
                {
                    aux.Add(x);
                }
            }
            aux.ExceptWith(PecasCapturadas(cor));
            return aux;
        }
        private Cor Adversaria(Cor cor)
        {
            if (cor == Cor.branca)
            {
                return Cor.preta;
            }
            else
            {
                return Cor.branca;
            }
        }

        private Peca Rei(Cor cor)
        {
            foreach (Peca x in PecasEmJogo(cor))
            {
                if (x is Rei)
                {
                    return x;
                }
            }
            return null;
        }
        public bool EstaEmXeque(Cor cor)
        {
            Peca R = Rei(cor);
            if (R == null)
            {
                throw new TabuleiroException("Não tem rei da cor no tabuleiro");
            }
            foreach (Peca x in PecasEmJogo(Adversaria(cor)))
            {
                bool[,] mat = x.MovimentosPossiveis();
                if (mat[R.Posicao.Linha, R.Posicao.Coluna])
                {
                    return true;
                }
            }
            return false;
        }

        public bool TesteXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }
            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < tab.Linhas; i++)
                {
                    for (int j = 0; j < tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
        }
        private void ColocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(tab, Cor.branca));
            ColocarNovaPeca('b', 1, new Cavalo(tab, Cor.branca));
            ColocarNovaPeca('c', 1, new Bispo(tab, Cor.branca));
            ColocarNovaPeca('d', 1, new Dama(tab, Cor.branca));
            ColocarNovaPeca('e', 1, new Rei(tab, Cor.branca));
            ColocarNovaPeca('f', 1, new Bispo(tab, Cor.branca));
            ColocarNovaPeca('g', 1, new Cavalo(tab, Cor.branca));
            ColocarNovaPeca('h', 1, new Torre(tab, Cor.branca));
            ColocarNovaPeca('a', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('b', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('c', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('d', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('e', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('f', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('g', 2, new Peao(tab, Cor.branca));
            ColocarNovaPeca('h', 2, new Peao(tab, Cor.branca));

            ColocarNovaPeca('a', 8, new Torre(tab, Cor.preta));
            ColocarNovaPeca('b', 8, new Cavalo(tab, Cor.preta));
            ColocarNovaPeca('c', 8, new Bispo(tab, Cor.preta));
            ColocarNovaPeca('d', 8, new Dama(tab, Cor.preta));
            ColocarNovaPeca('e', 8, new Rei(tab, Cor.preta));
            ColocarNovaPeca('f', 8, new Bispo(tab, Cor.preta));
            ColocarNovaPeca('g', 8, new Cavalo(tab, Cor.preta));
            ColocarNovaPeca('h', 8, new Torre(tab, Cor.preta));
            ColocarNovaPeca('a', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('b', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('c', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('d', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('e', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('f', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('g', 7, new Peao(tab, Cor.preta));
            ColocarNovaPeca('h', 7, new Peao(tab, Cor.preta));

        }
    }
}
