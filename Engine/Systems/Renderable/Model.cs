using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    public struct Vertex
    {
        public Vertex(double X, double Y, double Z, double R, double G, double B, double A, double UVX, double UVY)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
            this.UVX = UVX;
            this.UVY = UVY;
        }
        double X;
        double Y;
        double Z;
        double R;
        double G;
        double B;
        double A;
        double UVX;
        double UVY;
    }


    public class Model
    {
        public Model()
        {
            this.Color = Color.RGB(1.0, 1.0, 1.0);
        }

        public static Model LoadFile(string filename)
        {
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            
            int[] precache;

            if (ext == ".obj")
                precache = OBJ.Load(filename);
            else
                throw new NotImplementedException("Model format not implented");

            Model mdl = new Model();

            mdl.VBOID = precache[0];
            Console.WriteLine("VBOID ONLINE...");
            mdl.Vertices = precache[1];

            return mdl;
        }

        public void Draw(ITransform Transform)
        {
            Vector Pos = Transform.Position;
            Matrix4d Ang = Transform.Orientation;
            Vector Scale = Transform.Scale;

            GL.PushMatrix();
            {
                GL.Color4(this.Color);
                GL.Translate(Pos);

                GL.Scale(Scale);

                Ang.Transpose();

                GL.MultMatrix(ref Ang);
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBOID);

                    GL.VertexPointer(3, VertexPointerType.Double, sizeof(double) * 9, 0);
                    GL.ColorPointer(4, ColorPointerType.Double, sizeof(double) * 9, sizeof(double) * 3);
                    GL.TexCoordPointer(2, TexCoordPointerType.Double, sizeof(double) * 9, sizeof(double) * 7);

                    GL.DrawArrays(BeginMode.Triangles, 0, this.Vertices);
                }
            }
            GL.PopMatrix();
        }
        public Color Color { get; set; }
        public int VBOID;
        public int Vertices { get; set; }
    }
}

