using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
    internal class Plataforma : Cubo
    {
        private bool exibeVetorNormal = false;
        public bool isFalso = false;
        public Plataforma(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
        {}

    }
}