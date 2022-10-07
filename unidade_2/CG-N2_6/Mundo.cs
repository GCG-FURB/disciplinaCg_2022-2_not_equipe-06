/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using CG_N2;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
        private bool mouseMoverPto = false;
        private Retangulo obj_Retangulo;
        private Ponto pontoSpline1;
        private Ponto pontoSpline2;
        private Ponto pontoSpline3;
        private Ponto pontoSpline4;
        private Ponto pontoSplineSelecionado;
        private Spline spline;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        private void DesenharCirculo(Ponto4D pontoCentral, int raio, Cor cor, int tamanho, int pontos = 72, PrimitiveType primitivo = PrimitiveType.Points)
        {
            Circulo circulo = new Circulo(Convert.ToChar("C"), null, pontoCentral, raio, pontos, primitivo);
            circulo.ObjetoCor.CorR = cor.CorR; circulo.ObjetoCor.CorG = cor.CorG; circulo.ObjetoCor.CorB = cor.CorB;
            circulo.PrimitivaTamanho = tamanho;
            objetosLista.Add(circulo);
        }

        private void DesenharReta(Ponto4D pontoInicio, Ponto4D pontoFim, Cor cor, int tamanho)
        {
            SegReta reta = new SegReta(Convert.ToChar("R"), null, pontoInicio, pontoFim);
            reta.ObjetoCor.CorR = cor.CorR; reta.ObjetoCor.CorG = cor.CorG; reta.ObjetoCor.CorB = cor.CorB;
            reta.PrimitivaTamanho = tamanho;
            objetosLista.Add(reta);
        }

        private Ponto4D pontoA = new Ponto4D(0, 0);
        private Ponto4D pontoB = new Ponto4D(100, 0);
        private int raio = 100;
        private int angulo = 45;

        private void MoverSrPalitoParaEsquerda()
        {
            pontoA.X -= 2;
            pontoB.X -= 2;
        }
        private void MoverSrPalitoParaDireita()
        {
            pontoA.X += 2;
            pontoB.X += 2;
        }

        private void AumentarRaioPalito()
        {
            raio += 2;
            CalcularPontoBPalito();
        }

        private void DiminuirRaioPalito()
        {
            raio -= 2;
            CalcularPontoBPalito();
        }

        private void AumentarAnguloPalito()
        {
            angulo += 2;
            CalcularPontoBPalito();
        }

        private void DiminuirAnguloPalito()
        {
            angulo -= 2;
            CalcularPontoBPalito();
        }

        private void CalcularPontoBPalito()
        {
            pontoB.X = Math.Cos(angulo * Math.PI / 180) * raio;
            pontoB.Y = Math.Sin(angulo * Math.PI / 180) * raio;
        }

        private void DesenharSrPalito()
        {
            CalcularPontoBPalito();
            SegReta reta = new SegReta(Convert.ToChar("P"), null, pontoA, pontoB);
            Cor cor = new Cor(0, 0, 0, 0);
            reta.ObjetoCor.CorR = cor.CorR; reta.ObjetoCor.CorG = cor.CorG; reta.ObjetoCor.CorB = cor.CorB;
            reta.PrimitivaTamanho = 5;

            objetosLista.Add(reta);
        }
        private void DesenharSpline()
        {
            Ponto4D ponto1 = new Ponto4D(-200, -200);
            Ponto4D ponto2 = new Ponto4D(-200, 200);
            Ponto4D ponto3 = new Ponto4D(200, 200);
            Ponto4D ponto4 = new Ponto4D(200, -200);

            pontoSpline1 = new Ponto(Convert.ToChar("A"), null, ponto1);
            pontoSpline2 = new Ponto(Convert.ToChar("B"), null, ponto2);
            pontoSpline3 = new Ponto(Convert.ToChar("C"), null, ponto3);
            pontoSpline4 = new Ponto(Convert.ToChar("D"), null, ponto4);

            objetosLista.Add(pontoSpline1);
            objetosLista.Add(pontoSpline2);
            objetosLista.Add(pontoSpline3);
            objetosLista.Add(pontoSpline4);

            SegReta segReta1 = new SegReta(Convert.ToChar("E"), null, ponto1, ponto2);
            segReta1.PrimitivaTamanho = 4;
            segReta1.ObjetoCor.CorR = 0; segReta1.ObjetoCor.CorG = 255; segReta1.ObjetoCor.CorB = 255;
            objetosLista.Add(segReta1);

            SegReta segReta2 = new SegReta(Convert.ToChar("F"), null, ponto2, ponto3);
            segReta2.PrimitivaTamanho = 4;
            segReta2.ObjetoCor.CorR = 0; segReta2.ObjetoCor.CorG = 255; segReta2.ObjetoCor.CorB = 255;
            objetosLista.Add(segReta2);

            SegReta segReta3 = new SegReta(Convert.ToChar("G"), null, ponto3, ponto4);
            segReta3.PrimitivaTamanho = 4;
            segReta3.ObjetoCor.CorR = 0; segReta3.ObjetoCor.CorG = 255; segReta3.ObjetoCor.CorB = 255;
            objetosLista.Add(segReta3);

            spline = new Spline(Convert.ToChar("H"), null, ponto1, ponto2, ponto3, ponto4, 10);
            spline.PrimitivaTamanho = 4;
            spline.ObjetoCor.CorR = 255; spline.ObjetoCor.CorG = 255; spline.ObjetoCor.CorB = 0;
            objetosLista.Add(spline);

        }
        private void SelecionarPonto(Ponto ponto)
        {
            if (spline == null)
            {
                return;
            }

            pontoSpline1.cor = Color.Black;
            pontoSpline2.cor = Color.Black;
            pontoSpline3.cor = Color.Black;
            pontoSpline4.cor = Color.Black;
            ponto.cor = Color.Red;
            pontoSplineSelecionado = ponto;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -300; camera.xmax = 300; camera.ymin = -300; camera.ymax = 300;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            DesenharSpline();

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;

#endif
#if CG_OpenGL
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
#endif
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
#if CG_OpenGL
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
#if CG_OpenGL
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#endif
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
#if CG_Gizmo
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();
#endif
            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.E)
                camera.PanEsquerda();
            else if (e.Key == Key.D)
                camera.PanDireita();
            else if (e.Key == Key.C)
                camera.PanCima();
            else if (e.Key == Key.B)
                camera.PanBaixo();
            else if (e.Key == Key.I)
                camera.ZoomIn();
            else if (e.Key == Key.O)
                camera.ZoomOut();
            else if (e.Key == Key.Q)
                MoverSrPalitoParaEsquerda();
            else if (e.Key == Key.W)
                MoverSrPalitoParaDireita();
            else if (e.Key == Key.A)
                DiminuirRaioPalito();
            else if (e.Key == Key.S)
                AumentarRaioPalito();
            else if (e.Key == Key.Z)
                DiminuirAnguloPalito();
            else if (e.Key == Key.X)
                AumentarAnguloPalito();
            else if (e.Key == Key.Number1 || e.Key == Key.Keypad1)
                SelecionarPonto(pontoSpline1);
            else if (e.Key == Key.Number2 || e.Key == Key.Keypad2)
                SelecionarPonto(pontoSpline2);
            else if (e.Key == Key.Number3 || e.Key == Key.Keypad3)
                SelecionarPonto(pontoSpline3);
            else if (e.Key == Key.Number4 || e.Key == Key.Keypad4)
                SelecionarPonto(pontoSpline4);
            else if (e.Key == Key.Plus || e.Key == Key.KeypadAdd)
            {
                if (spline.quantidadePontos <= 100)
                    spline.quantidadePontos++;
            }
            else if (e.Key == Key.Minus || e.Key == Key.KeypadSubtract)
            {
                if (spline.quantidadePontos > 1)
                    spline.quantidadePontos--;
            }
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
#if CG_Gizmo
            else if (e.Key == Key.O)
                bBoxDesenhar = !bBoxDesenhar;
#endif
            else if (e.Key == Key.V)
                mouseMoverPto = !mouseMoverPto;   //TODO: falta atualizar a BBox do objeto
            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (mouseMoverPto && (objetoSelecionado != null))
            {
                objetoSelecionado.PontosUltimo().X = mouseX;
                objetoSelecionado.PontosUltimo().Y = mouseY;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
#if CG_OpenGL
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
#endif
        }
#endif
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N2_6";
            window.Run(1.0 / 60.0);
        }
    }
}
