using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SamSeifert.Utilities.FileParsing;
using OpenTK;

namespace SamSeifert.GLE.CAD
{
    public static partial class CadObjectGenerator
    {
        static void consolidateMatrices(CadObject co, List<CadObject> all_objects)
        {
            all_objects.Add(co);
            var m4 = Matrix4.Identity;
            if (co.BoolUseTranslationAndRotation) m4 = co._Matrix;
            co.BoolUseTranslationAndRotation = false;
            co._Matrix = Matrix4.Identity;
            foreach (var child in co.Children)
            {
                var new_mat = m4 * child._Matrix;
                child._Matrix = new_mat;
                child.BoolUseTranslationAndRotation = true;
                CadObjectGenerator.consolidateMatrices(child, all_objects);
            }
            co.transformPoints(ref m4);
        }


        public static CadObject CreateFromAnonymousFunction(Action a, Vector3[] vertices)
        {
            var co = new CadObject();
            co.Vertices = vertices;
            co.AnonymousDraw = a;
            return co;
        }























        













































        public static CadObject CreateSphere(
            float radius,
            int resolution = 0,
            String name = "Sphere"
            )
        {
            double r = 0.5 / Math.Sin(MathHelper.DegreesToRadians(36));
            double r_sq = r * r;
            double vx = Math.Sqrt(1 - r_sq);
            double vy = (2 * r_sq - 1) / vx;

            Vector3[] middle_ring = new Vector3[10];
            Vector3 top = new Vector3(0.0f, 1, 0.0f);
            Vector3 bot = new Vector3(0.0f, -1, 0.0f);

            for (int i = 0; i < 10; i++)
            {
                float ang = i * 36;
                middle_ring[i].X = (float)(r * Math.Sin(MathHelper.DegreesToRadians(ang)));
                middle_ring[i].Z = (float)(r * Math.Cos(MathHelper.DegreesToRadians(ang)));
                middle_ring[i].Y = (float)((vy / 2) * ((i % 2) * 2 - 1));
                middle_ring[i].Normalize();
            }

            var all = new List<Vector3>();

            // Middle Ring
            all.Add(middle_ring[0]); all.Add(middle_ring[2]); all.Add(middle_ring[1]);
            all.Add(middle_ring[1]); all.Add(middle_ring[9]); all.Add(middle_ring[0]);
            all.Add(middle_ring[2]); all.Add(middle_ring[4]); all.Add(middle_ring[3]);
            all.Add(middle_ring[3]); all.Add(middle_ring[1]); all.Add(middle_ring[2]);
            all.Add(middle_ring[4]); all.Add(middle_ring[6]); all.Add(middle_ring[5]);
            all.Add(middle_ring[5]); all.Add(middle_ring[3]); all.Add(middle_ring[4]);
            all.Add(middle_ring[6]); all.Add(middle_ring[8]); all.Add(middle_ring[7]);
            all.Add(middle_ring[7]); all.Add(middle_ring[5]); all.Add(middle_ring[6]);
            all.Add(middle_ring[8]); all.Add(middle_ring[0]); all.Add(middle_ring[9]);
            all.Add(middle_ring[9]); all.Add(middle_ring[7]); all.Add(middle_ring[8]);

            // Top 5
            all.Add(middle_ring[1]); all.Add(middle_ring[3]); all.Add(top);
            all.Add(middle_ring[3]); all.Add(middle_ring[5]); all.Add(top);
            all.Add(middle_ring[5]); all.Add(middle_ring[7]); all.Add(top);
            all.Add(middle_ring[7]); all.Add(middle_ring[9]); all.Add(top);
            all.Add(middle_ring[9]); all.Add(middle_ring[1]); all.Add(top);

            // Bottom 5
            all.Add(bot); all.Add(middle_ring[2]); all.Add(middle_ring[0]);
            all.Add(bot); all.Add(middle_ring[4]); all.Add(middle_ring[2]);
            all.Add(bot); all.Add(middle_ring[6]); all.Add(middle_ring[4]);
            all.Add(bot); all.Add(middle_ring[8]); all.Add(middle_ring[6]);
            all.Add(bot); all.Add(middle_ring[0]); all.Add(middle_ring[8]);

            for (int dex = 0; dex < resolution; dex++)
            {
                Vector3 p1, p2, p3, p4, p5, p6;
                var n = new List<Vector3>();
                for (int i = 0; i < all.Count; i += 3)
                {
                    p1 = all[i];
                    p2 = all[i+1];
                    p3 = all[i+2];
                    p4 = p3 + p1;
                    p5 = p2 + p1;
                    p6 = p2 + p3;
                    p4.Normalize();
                    p5.Normalize();
                    p6.Normalize();
                    n.Add(p1); n.Add(p5); n.Add(p4);
                    n.Add(p5); n.Add(p2); n.Add(p6);
                    n.Add(p4); n.Add(p5); n.Add(p6);
                    n.Add(p4); n.Add(p6); n.Add(p3);
                }
                all = n;
            }

            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            foreach (var v in all)
            {
                ns.Add(v);
                vs.Add(v * radius);                    
            }

            CadObject co = new CadObject(vs.ToArray(), ns.ToArray(), name);

            co.setBoundingSphere(Vector3.Zero, radius);

            return co;
        }




        public static CadObject CreateFace(
            Vector3 v1,
            Vector3 v2,
            Vector3 v3,
            Vector3 v4,
            Vector3 n,
            String name = "Plane")
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            vs.Add(v1); ns.Add(n);
            vs.Add(v2); ns.Add(n);
            vs.Add(v3); ns.Add(n);

            vs.Add(v1); ns.Add(n);
            vs.Add(v3); ns.Add(n);
            vs.Add(v4); ns.Add(n);

            return new CadObject(vs.ToArray(), ns.ToArray(), name);
        }
    }
}
