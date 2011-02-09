using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Reflection;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    public static class OBJ
    {
        private static bool _Initalized = false;

        private delegate void TDele(string[] args);
        private static Dictionary<string, TDele> Subscribed;
        
        public static void Init()
        {
            if (_Initalized)
                return;
            Subscribed = new Dictionary<string,TDele>();

            Type info = typeof(OBJ);
            
            MethodInfo[] methods = info.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

            foreach (MethodInfo m in methods)
            {
                object[] attr = m.GetCustomAttributes(typeof(OBJHandelerAttribute), false);

                foreach (object obj in attr)
                {
                    OBJHandelerAttribute att = (OBJHandelerAttribute)obj;
                    Subscribed.Add(att.Name, (TDele)Delegate.CreateDelegate(typeof(TDele), m));
                }
            }
            
            _Initalized = true;
        }

        /// <summary>
        /// Maps a pre-loaded buffer to an ID
        /// </summary>
        private static Dictionary<string, int[]> _PreCached = new Dictionary<string, int[]>();

        /// <summary>
        /// Returns the VBO ID, Number of primatives
        /// </summary>
        public static int[] Load(string filename)
        {
            int[] outval;
            if (_PreCached.TryGetValue(filename, out outval))
                return outval;

            outval = _Load(filename);
            _PreCached.Add(filename, outval);

            return outval;
        }

        private static double _Scale = 1.0;

        private static List<Vector3d> _Points;
        private static List<Vector3d> _Normals;
        private static List<Vector2d> _TextureCords;
        private static List<Vertex[]> _Triangles;

        /// <summary>
        /// Loads the actual object if it has not been precached. (NOT MULTI THREADED!)
        /// </summary>
        private static int[] _Load(string filename)
        {
            if (!_Initalized)
                Init();

            StreamReader str = new StreamReader( filename );

            _Points = new List<Vector3d>();
            _Normals = new List<Vector3d>();
            _TextureCords = new List<Vector2d>();
            _Triangles = new List<Vertex[]>();

            while (!str.EndOfStream)
            {
                string[] split_line = str.ReadLine().Trim().Split(" ".ToCharArray());
                string opcode = split_line[0];

                TDele dele;
                if (Subscribed.TryGetValue(opcode, out dele))
                {
                    dele(split_line);
                }
            }

            int vertexcount = _Triangles.Count * 3;
            Vertex[] vertices = new Vertex[vertexcount];

            for (int i = 0; i < _Triangles.Count; i++)
            {
                vertices[i * 3] = _Triangles[i][0];
                vertices[i * 3 + 1] = _Triangles[i][1];
                vertices[i * 3 + 2] = _Triangles[i][2];
            }

            int id = 0;
			GL.GenBuffers( 1, out id );
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Vertex.Size * vertexcount), vertices, BufferUsageHint.StaticDraw);

            return new int[2]
            {
                id,
                vertexcount
            };
        }

        [OBJHandeler("v")]
        private static void DoVert(string[] args)
        {
            double x = double.Parse(args[1]);
            double y = double.Parse(args[2]);
            double z = double.Parse(args[3]);
            _Points.Add(new Vector3d(x * _Scale, y * _Scale, z * _Scale));
        }

        [OBJHandeler("vt")]
        private static void DoTextCords(string[] args)
        {
            double x = double.Parse(args[1]);
            double y = double.Parse(args[2]);
            _TextureCords.Add(new Vector2d(x * _Scale, y * _Scale));
        }

        [OBJHandeler("vn")]
        private static void DoNormal(string[] args)
        {
            double x = double.Parse(args[1]);
            double y = double.Parse(args[2]);
            double z = double.Parse(args[3]);
            _Normals.Add(new Vector3d(x, y, z));
        }

        [OBJHandeler("f")]
        private static void DoFace(string[] args)
        {
            Vector3d point;
            Vector2d uv = new Vector2d();
            Vector3d normal = new Vector3d();
            Vertex[] tr_all = new Vertex[4];

            int facedefs = (args.Length - 1);

            for (int face = 0; face < facedefs; face++)
            {
                string[] tmp = args[face + 1].Split("/".ToCharArray());
                int j = 0;
                int v = int.Parse(tmp[j++]);
                int n = 0;
                point = _Points[v - 1];

                int t = 0;

                if (tmp.Length > 1)
                {
                    string s = tmp[j++];
                    if (_TextureCords.Count > 0)
                    {
                        t = int.Parse(s);
                        uv = _TextureCords[t - 1];
                    }
                }
                if (_Normals.Count > 0)
                {
                    string s = tmp[j++];
                    if (s.Length > 0)
                    {
                        n = int.Parse(s);
                        normal = _Normals[n - 1];
                    }
                }

                tr_all[face] = new Vertex(point.X, point.Y, point.Z, 1, 1, 1, 1, uv.X, uv.Y);
            }

            Vertex[] tr = new Vertex[3];
            for (int i = 0; i < 3; i++)
                tr[i] = tr_all[i];
            _Triangles.Add(tr);

            if (facedefs == 4)
            {
                // x z a || 0 2 3
                Vertex[] tr_quadbit = new Vertex[3];
                tr_quadbit[0] = tr_all[0];
                tr_quadbit[1] = tr_all[2];
                tr_quadbit[2] = tr_all[3];
                _Triangles.Add(tr_quadbit);
            }
        }

        [OBJHandeler("scale")]
        private static void DoScale(string[] args)
        {
            _Scale = double.Parse(args[1]);
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OBJHandelerAttribute : Attribute
    {
        private string _Name;
        public string Name
        {
            get { return this._Name; }
        }

        public OBJHandelerAttribute(string name)
        {
            this._Name = name;
        }
    }
}