using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;


namespace gcgcg
{
    internal class SegReta : ObjetoGeometria
    {

        public SegReta(char rotulo, Objeto paiRef, Ponto4D pontoInicio, Ponto4D pontoFim, PrimitiveType primitivo = PrimitiveType.Lines) : base(rotulo, paiRef)
        {
            base.PrimitivaTipo = primitivo;
            base.PontosAdicionar(pontoInicio);
            base.PontosAdicionar(pontoFim);
        }

        protected override void DesenharObjeto()
        {
            GL.Begin(base.PrimitivaTipo);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }

    }
}
