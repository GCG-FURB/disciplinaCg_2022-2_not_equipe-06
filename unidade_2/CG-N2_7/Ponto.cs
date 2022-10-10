using CG_Biblioteca;
using gcgcg;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_N2
{
    internal class Ponto : ObjetoGeometria
    {
        public Ponto4D ponto { get; set; }
        public Cor cor { private get; set; }


        public Ponto(char rotulo, Objeto paiRef, Ponto4D ponto, int tamanho = 20) : base(rotulo, paiRef)
        {
            PrimitivaTamanho = tamanho;
            base.PrimitivaTipo = PrimitiveType.Points;
            base.PontosAdicionar(ponto);
            this.ponto = ponto;
            cor = new Cor(0, 0, 0, 255);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.Points);
            GL.Color3(cor.CorR, cor.CorG, cor.CorB);
            GL.Vertex2(pontosLista[0].X, pontosLista[0].Y);
            GL.End();
        }
    }
}
