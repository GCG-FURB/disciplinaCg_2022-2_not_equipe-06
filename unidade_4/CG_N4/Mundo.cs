#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

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

        private CameraPerspective camera = new CameraPerspective();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private String menuSelecao = "";
        private char menuEixoSelecao = 'z';
        private float deslocamento = 0;
        private bool bBoxDesenhar = false;

        private BBox bBox;

        private bool firstPerson = true;

        private Vector3 cameraPositionAt;

        private Vector3 cameraPositionEye;

        private Character character;

        //  Controle de movimentação
        private bool front = false;

        private bool back = true;

        private bool left = false;

        private bool right = false;

        private Cubo start;

        private Cubo map;

        private Cubo end;

        private int row;

        private int col;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            // Mapa do jogo
            GL.Color3(1.0f, 0.0f, 0.0f);
            objetoId = Utilitario.charProximo(objetoId);
            start = new Cubo(objetoId, null);
            start.ObjetoCor.CorR = 0; start.ObjetoCor.CorG = 0; start.ObjetoCor.CorB = 0;
            objetosLista.Add(start);
            objetoSelecionado = start;
            objetoSelecionado.EscalaXYZBBox(10, 1, 1);
            objetoSelecionado.Translacao(45, 'x');
            objetoSelecionado.Translacao(-10, 'z');

            objetoSelecionado.ObjetoCor.CorR = 27;
            objetoSelecionado.ObjetoCor.CorG = 36;
            objetoSelecionado.ObjetoCor.CorB = 158;

            for (col = 0; col < 100; col += 10)
            {
                for (row = 0; row < 100; row += 10)
                {
                    objetoId = Utilitario.charProximo(objetoId);
                    map = new Cubo(objetoId, null);
                    objetosLista.Add(map);
                    objetoSelecionado = map;
                    objetoSelecionado.Translacao(row, 'x');
                    objetoSelecionado.Translacao(col, 'z');

                    objetoSelecionado.ObjetoCor.CorR = 70;
                    objetoSelecionado.ObjetoCor.CorG = 25;
                    objetoSelecionado.ObjetoCor.CorB = 110;
                }
            }

            objetoId = Utilitario.charProximo(objetoId);
            end = new Cubo(objetoId, null);
            objetosLista.Add(end);
            objetoSelecionado = end;
            objetoSelecionado.EscalaXYZBBox(10, 1, 1);
            objetoSelecionado.Translacao(45, 'x');
            objetoSelecionado.Translacao(100, 'z');

            objetoSelecionado.ObjetoCor.CorR = 44;
            objetoSelecionado.ObjetoCor.CorG = 163;
            objetoSelecionado.ObjetoCor.CorB = 31;
            // Mapa do jogo

            // Personagem
            objetoId = Utilitario.charProximo(objetoId);
            character = new Character(objetoId, null);
            objetosLista.Add(character);
            objetoSelecionado = character;

            objetoSelecionado.Translacao(-5, 'z');
            objetoSelecionado.Translacao(33.66f, 'x');
            objetoSelecionado.Escala(1.5f);
            objetoSelecionado.Rotacao(180, 'y');
            // Personagem

            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        public void resetCharacter()
        {
            objetoSelecionado.Matriz.AtribuirIdentidade();
            objetoSelecionado.Translacao(-5, 'z');
            objetoSelecionado.Translacao(33.66f, 'x');
            objetoSelecionado.Escala(1.5f);
            objetoSelecionado.Rotacao(180, 'y');

            front = false;
            back = true;
            left = false;
            right = false;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, Width / (float)Height, camera.Near, 200.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (firstPerson)
            {
                cameraPositionEye = new Vector3((float)objetoSelecionado.Matriz.ObterDados()[12], (float)objetoSelecionado.Matriz.ObterDados()[13] + 20, (float)objetoSelecionado.Matriz.ObterDados()[14] - 40);
                cameraPositionAt = new Vector3((float)objetoSelecionado.Matriz.ObterDados()[12], (float)objetoSelecionado.Matriz.ObterDados()[13], (float)objetoSelecionado.Matriz.ObterDados()[14] + 20);
            }
            else
            {
                cameraPositionEye = new Vector3(50.5f, 30.0f, -65.0f);
                cameraPositionAt = new Vector3(50.5f, 0.0f, 20.0f);
            }

            Matrix4 modelview = Matrix4.LookAt(cameraPositionEye, cameraPositionAt, camera.Up);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
            {
                bBox = new BBox(
                    (float)objetoSelecionado.Matriz.ObterDados()[12] - 2.5f,
                    (float)objetoSelecionado.Matriz.ObterDados()[13],
                    (float)objetoSelecionado.Matriz.ObterDados()[14] - 2.5f,

                    (float)objetoSelecionado.Matriz.ObterDados()[12] + 2.5f,
                    (float)objetoSelecionado.Matriz.ObterDados()[13] + 6.0f,
                    (float)objetoSelecionado.Matriz.ObterDados()[14] + 2.5f);
                bBox.Desenhar();
            }
            this.SwapBuffers();
        }

        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H) Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape) Exit();
            //--------------------------------------------------------------
            else if (e.Key == Key.E)
            {
                Console.WriteLine("--- Objetos / Pontos: ");
                for (var i = 0; i < objetosLista.Count; i++)
                {
                    Console.WriteLine(objetosLista[i]);
                }
            }
            //--------------------------------------------------------------
            else if (e.Key == Key.X) menuEixoSelecao = 'x';
            else if (e.Key == Key.Y) menuEixoSelecao = 'y';
            else if (e.Key == Key.Z) menuEixoSelecao = 'z';
            else if (e.Key == Key.Minus) deslocamento--;
            else if (e.Key == Key.Plus) deslocamento++;

            // Movimentação do Personagem 
            else if (e.Key == Key.W) // Para frente
            {
                front = true;
                if (left)
                {
                    character.Rotacao(-90, 'y');
                    left = false;
                }
                else if (right)
                {
                    character.Rotacao(90, 'y');
                    right = false;
                }
                else if (front && back) objetoSelecionado.Rotacao(180, 'y');
                objetoSelecionado.Translacao(1, 'z');
                back = false;
                left = false;
                right = false;
            }
            else if (e.Key == Key.S) // Para trás
            {
                back = true;
                if (left)
                {
                    character.Rotacao(90, 'y');
                    left = false;
                }
                else if (right)
                {
                    character.Rotacao(-90, 'y');
                    right = false;
                }
                if (back && front) objetoSelecionado.Rotacao(180, 'y');
                objetoSelecionado.Translacao(-1, 'z');
                front = false;
                left = false;
                right = false;
            }
            else if (e.Key == Key.A) // Para esquerda
            {
                left = true;
                if (left && right) objetoSelecionado.Rotacao(180, 'y');
                else if (left && front)
                {
                    objetoSelecionado.Rotacao(90, 'y');
                    front = false;
                }
                else if (left && back)
                {
                    objetoSelecionado.Rotacao(-90, 'y');
                    back = false;
                }
                objetoSelecionado.Translacao(1, 'x');
                right = false;
                front = false;
                back = false;
            }
            else if (e.Key == Key.D) // Para direita
            {
                right = true;
                if (right && left) objetoSelecionado.Rotacao(-180, 'y');
                else if (right && front)
                {
                    objetoSelecionado.Rotacao(-90, 'y');
                    front = false;
                }
                else if (right && back)
                {
                    objetoSelecionado.Rotacao(90, 'y');
                    back = false;
                }
                objetoSelecionado.Translacao(-1, 'x');
                left = false;
                front = false;
                back = false;
            }
            // Movimentação do Personagem 

            else if (e.Key == Key.P)
            {
                Console.WriteLine("Location: " + new Vector3(
                                                            (float)objetoSelecionado.Matriz.ObterDados()[12],
                                                            (float)objetoSelecionado.Matriz.ObterDados()[13],
                                                            (float)objetoSelecionado.Matriz.ObterDados()[14]));
            }

            // Altera modo de câmera
            else if (e.Key == Key.C)
            {
                firstPerson = !firstPerson;

            }
            // Altera modo de câmera

            else if (e.Key == Key.R)
            {
                resetCharacter();
            }

            else if (e.Key == Key.O)
            {
                bBoxDesenhar = !bBoxDesenhar;
            }

            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
        }

#if CG_Gizmo
        private void Sru3D()
        {
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
        }
#endif
    }
    class Program
    {
        static void Main(string[] args)
        {
            ToolkitOptions.Default.EnableHighResolution = false;
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N4: Fall Guy";
            window.Run(1.0 / 60.0);
        }
    }
}