using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace L2D.Engine
{
    public static class OBJ
    {
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

        private static int[] _Load(string filename)
        {
            StreamReader str = new StreamReader( filename );
            
            List<Vector3d> points = new List<Vector3d>();
            List<Vector3d> normals = new List<Vector3d>();
            List<Vector2d> texcoords = new List<Vector2d>();
            List<Vertex[]> triangles = new List<Vertex[]>();

            double x, y, z;
            int v, t, n;

            while (!str.EndOfStream)
            {
                string line = str.ReadLine().Trim();
                string[] split_line = line.Split(" ".ToCharArray());
                if (line.StartsWith("v"))
                {
                    x = double.Parse(split_line[1]);
                    y = double.Parse(split_line[2]);
                    z = double.Parse(split_line[3]);
                    points.Add(new Vector3d(x, z, y));
                }
                else if (line.StartsWith("vt"))
                {
                    x = double.Parse(split_line[1]);
                    y = double.Parse(split_line[2]);
                    texcoords.Add(new Vector2d(x, y));
                }
                else if (line.StartsWith("vn"))
                {
                    x = double.Parse(split_line[1]);
                    y = double.Parse(split_line[2]);
                    z = double.Parse(split_line[3]);
                    normals.Add(new Vector3d(x, y, z));
                }
                else if (line.StartsWith("f"))
                {

                    Vector3d point;
                    Vector2d uv = new Vector2d();
                    Vector3d normal = new Vector3d();
                    Vertex[] tr_all = new Vertex[4];

                    int facedefs = (split_line.Length - 1);

                    for (int face = 0; face < facedefs; face++)
                    {
                        string[] tmp = split_line[face + 1].Split("/".ToCharArray());
                        int j = 0;
                        v = int.Parse(tmp[j++]);
                        point = points[v - 1];

                        if (tmp.Length > 1)
                        {
                            string s = tmp[j++];
                            if (texcoords.Count > 0)
                            {
                                t = int.Parse(s);
                                uv = texcoords[t - 1];
                            }
                        }
                        if (normals.Count > 0)
                        {
                            string s = tmp[j++];
                            if (s.Length > 0)
                            {
                                n = int.Parse(s);
                                normal = normals[n - 1];
                            }
                        }

                        tr_all[face] = new Vertex(point.X, point.Y, point.Z, 1, 1, 1, 1, uv.X, uv.Y);
                    }

                    Vertex[] tr = new Vertex[3];
                    for (int i = 0; i < 3; i++)
                        tr[i] = tr_all[i];
                    triangles.Add(tr);

                    if (facedefs == 4)
                    {
                        // x z a || 0 2 3
                        Vertex[] tr_quadbit = new Vertex[3];
                        tr_quadbit[0] = tr_all[0];
                        tr_quadbit[1] = tr_all[2];
                        tr_quadbit[2] = tr_all[3];
                        triangles.Add(tr_quadbit);
                    }
                }
            }

            int vertexcount = triangles.Count * 3;

            Vertex[] vertices = new Vertex[vertexcount];

            for (int i = 0; i < triangles.Count; i++)
            {

                vertices[i * 3] = triangles[i][0];
                vertices[i * 3 + 1] = triangles[i][1];
                vertices[i * 3 + 2] = triangles[i][2];
            }

            //--------------  Send to the card

            int id = 0;
			GL.GenBuffers( 1, out id );
            GL.BindBuffer(BufferTarget.ArrayBuffer, id);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(double) * 9 * vertexcount), vertices, BufferUsageHint.StaticDraw);

            return new int[2]
            {
                id,
                vertexcount
            };
        }
    }
}