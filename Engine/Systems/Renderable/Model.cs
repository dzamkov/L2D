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
        public const int Size = sizeof(float) * (3 + 4 + 2 + 3);
        public Vertex(Vector3d Position, Color4 Color, Vector2d UV, Vector3d Normal)
        {
            this.Position = new Vector3((float)Position.X, 
                (float)Position.Y, 
                (float)Position.Z);
            
            this.Color = Color;

            this.UV = new Vector2((float)UV.X,
                (float)UV.Y);

            this.Normal = new Vector3((float)Normal.X, 
                (float)Normal.Y, 
                (float)Normal.Z);
        }
        public Vector3 Position;
        public Color4 Color;
        public Vector2 UV;
        public Vector3 Normal;
    }


    public class Model
    {
        public Model()
        {
            this.Color = Color.RGB(1.0, 1.0, 1.0);
        }

        public static Model LoadFile(Path path, string filename)
        {
            string ext = System.IO.Path.GetExtension(filename).ToLower();
            
            int[] precache;

            if (ext == ".obj")
                precache = OBJ.Load(path["Models"][filename]);
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
                //GL.Color4(this.Color);
                
				GL.Translate(Pos);
				GL.MultMatrix(ref Ang);
				GL.Scale(Scale);
                {
                    GL.BindBuffer(BufferTarget.ArrayBuffer, this.VBOID);
                    GL.InterleavedArrays(InterleavedArrayFormat.T2fC4fN3fV3f, 0, IntPtr.Zero);
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

