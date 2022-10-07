using CG_Biblioteca;
using gcgcg;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_N2
{
    internal class Ponto : ObjetoGeometria
    {
        public Ponto4D ponto { get; set; }
        public Color cor { private get; set; }


        public Ponto(char rotulo, Objeto paiRef, Ponto4D ponto, int tamanho = 20) : base(rotulo, paiRef)
        {
            PrimitivaTamanho = tamanho;
            base.PrimitivaTipo = PrimitiveType.Points;
            base.PontosAdicionar(ponto);
            this.ponto = ponto;
            cor = Color.Black;
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitiveType.Points);
            GL.Color3(cor);
            GL.Vertex2(pontosLista[0].X, pontosLista[0].Y);
            GL.End();
        }
    }
}
