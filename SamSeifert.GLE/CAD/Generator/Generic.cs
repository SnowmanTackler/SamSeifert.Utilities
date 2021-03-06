﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using SamSeifert.Utilities.Files.Json;
using OpenTK;
using SamSeifert.Utilities.Extensions;
using SamSeifert.GLE.Extensions;
using SamSeifert.Utilities;

namespace SamSeifert.GLE.CAD.Generator
{
    public static class Generic
    {



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

            co.SetBoundingSphere(Vector3.Zero, radius);

            return co;
        }


        public static CadObject CreateCylinder(Geometry3D.Cylinder c, int section = 36)
        {
            return CreateCylinder(c._Point1, c._Point2, c._Radius, section);
        }

        public static CadObject CreateCylinder(
            Vector3 bot,
            Vector3 top,
            float radius,
            int section = 36
            )
        {
            var norm = (bot - top).Normalized();

            Vector3 perp1, perp2;

            {
                perp1 = norm.Perpindicular() * radius;
                perp2 = Vector3.Cross(perp1, norm).Normalized() * radius;
            }


            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            for (int i = 0; i < section; i++)
            {
                var angle1 = (i + 0) * Math.PI * 2 / section;
                var angle2 = (i + 1) * Math.PI * 2 / section;

                var radial1 = perp1 * (float)Math.Cos(angle1) + perp2 * (float)Math.Sin(angle1);
                var radial2 = perp1 * (float)Math.Cos(angle2) + perp2 * (float)Math.Sin(angle2);

                vs.AddRange(new Vector3[] { bot, bot + radial2, bot + radial1 });
                ns.AddRange(new Vector3[] { norm, norm, norm });
                vs.AddRange(new Vector3[] { top, top + radial1, top + radial2 });
                ns.AddRange(new Vector3[] { -norm, -norm, -norm });

                vs.AddRange(new Vector3[] { bot + radial2, top + radial1, bot + radial1 });
                ns.AddRange(new Vector3[] { radial2.Normalized(), radial1.Normalized(), radial1.Normalized() });

                vs.AddRange(new Vector3[] { bot + radial2, top + radial2, top + radial1 });
                ns.AddRange(new Vector3[] { radial2.Normalized(), radial2.Normalized(), radial1.Normalized() });
            }

            var co = new CadObject(vs.ToArray(), ns.ToArray(), "Cylinder");
            co._CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode.Back;
            return co;
        }

        public static CadObject CreateCylinderHollow(
            Vector3 bot,
            Vector3 top,
            float outerRadius,
            float innerRadius,
            int section = 36
            )
        {
            var norm = (bot - top).Normalized();

            Vector3 perp1, perp2;

            {
                perp1 = norm.Perpindicular();
                perp2 = Vector3.Cross(perp1, norm).Normalized();
            }

            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            for (int i = 0; i < section; i++)
            {
                var angle1 = (i + 0) * Math.PI * 2 / section;
                var angle2 = (i + 1) * Math.PI * 2 / section;

                var radial1 = perp1 * outerRadius * (float)Math.Cos(angle1) + perp2 * outerRadius * (float)Math.Sin(angle1);
                var radial2 = perp1 * outerRadius * (float)Math.Cos(angle2) + perp2 * outerRadius * (float)Math.Sin(angle2);
                var radial3 = perp1 * innerRadius * (float)Math.Cos(angle2) + perp2 * innerRadius * (float)Math.Sin(angle2);
                var radial4 = perp1 * innerRadius * (float)Math.Cos(angle1) + perp2 * innerRadius * (float)Math.Sin(angle1);

                Face(top + radial1, top + radial2, top + radial3, top + radial4, -norm, vs, ns);
                Face(bot + radial4, bot + radial3, bot + radial2, bot + radial1, norm, vs, ns);

                vs.AddRange(new Vector3[] { bot + radial2, top + radial1, bot + radial1 });
                ns.AddRange(new Vector3[] { radial2.Normalized(), radial1.Normalized(), radial1.Normalized() });

                vs.AddRange(new Vector3[] { bot + radial2, top + radial2, top + radial1 });
                ns.AddRange(new Vector3[] { radial2.Normalized(), radial2.Normalized(), radial1.Normalized() });

                vs.AddRange(new Vector3[] { bot + radial4, top + radial3, bot + radial3 });
                ns.AddRange(new Vector3[] { -radial4.Normalized(), -radial3.Normalized(), -radial3.Normalized() });

                vs.AddRange(new Vector3[] { bot + radial4, top + radial4, top + radial3 });
                ns.AddRange(new Vector3[] { -radial4.Normalized(), -radial4.Normalized(), -radial3.Normalized() });
            }

            var co = new CadObject(vs.ToArray(), ns.ToArray(), "Cylinder");
            co._CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode.Back;
            return co;
        }


        public static CadObject CreateCylinders(Vector3[] fiberPath, float radius, int section = 36)
        {
            Assert.IsTrue(fiberPath.Length > 1);

            var norm = (fiberPath[1] - fiberPath[0]).Normalized();

            Vector3 perp_up, perp_right;

            {
                perp_up = norm.Perpindicular() * radius;
                perp_right = Vector3.Cross(norm, perp_up).Normalized() * radius;
            }

            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();


            for (int i = 0; i < section; i++)
            {
                var angle1 = (i + 0) * Math.PI * 2 / section;
                var angle2 = (i + 1) * Math.PI * 2 / section;

                var radial1 = perp_up * (float)Math.Cos(angle1) + perp_right * (float)Math.Sin(angle1);
                var radial2 = perp_up * (float)Math.Cos(angle2) + perp_right * (float)Math.Sin(angle2);

                var pt = fiberPath.First();
                vs.AddRange(new Vector3[] { pt, pt + radial2, pt + radial1 });
                ns.AddRange(new Vector3[] { -norm, -norm, -norm });
            }

            for (int j = 0; j < fiberPath.Length - 1; j++)
            {
                var new_norm = (fiberPath[j+1] - fiberPath[j]).Normalized();
                var new_perp_up = Vector3.Cross(perp_right, new_norm).Normalized() * radius;
                var new_perp_right = Vector3.Cross(new_norm, new_perp_up).Normalized() * radius;

                for (int i = 0; i < section; i++)
                {
                    var angle1 = (i + 0) * Math.PI * 2 / section;
                    var angle2 = (i + 1) * Math.PI * 2 / section;

                    var radial1_at_j0 = perp_up * (float)Math.Cos(angle1) + perp_right * (float)Math.Sin(angle1);
                    var radial2_at_j0 = perp_up * (float)Math.Cos(angle2) + perp_right * (float)Math.Sin(angle2);
                    var radial1_at_j1 = new_perp_up * (float)Math.Cos(angle1) + new_perp_right * (float)Math.Sin(angle1);
                    var radial2_at_j1 = new_perp_up * (float)Math.Cos(angle2) + new_perp_right * (float)Math.Sin(angle2);

                    var j0 = fiberPath[j + 0];
                    var j1 = fiberPath[j + 1];

                    vs.AddRange(new Vector3[] { j0 + radial2_at_j0, j1 + radial1_at_j1, j0 + radial1_at_j0 });
                    ns.AddRange(new Vector3[] { radial2_at_j0.Normalized(), radial1_at_j1.Normalized(), radial1_at_j0.Normalized() });

                    vs.AddRange(new Vector3[] { j0 + radial2_at_j0, j1 + radial2_at_j1, j1 + radial1_at_j1 });
                    ns.AddRange(new Vector3[] { radial2_at_j0.Normalized(), radial2_at_j1.Normalized(), radial1_at_j1.Normalized() });
                }

                norm = new_norm;
                perp_up = new_perp_up;
                perp_right = new_perp_right;           
            }

            for (int i = 0; i < section; i++)
            {
                var angle1 = (i + 0) * Math.PI * 2 / section;
                var angle2 = (i + 1) * Math.PI * 2 / section;

                var radial1 = perp_up * (float)Math.Cos(angle1) + perp_right * (float)Math.Sin(angle1);
                var radial2 = perp_up * (float)Math.Cos(angle2) + perp_right * (float)Math.Sin(angle2);

                var pt = fiberPath.Last();
                vs.AddRange(new Vector3[] { pt, pt + radial1, pt + radial2 });
                ns.AddRange(new Vector3[] { norm, norm, norm });
            }


            var co = new CadObject(vs.ToArray(), ns.ToArray(), "Cylinder");
            co._CullFaceMode = OpenTK.Graphics.OpenGL.CullFaceMode.Back;
            return co;
        }


        private static void Face(
            Vector3 v1,
            Vector3 v2,
            Vector3 v3,
            Vector3 v4,
            Vector3 n,
            List<Vector3> vs,
            List<Vector3> ns
        )
        {
            vs.Add(v1); ns.Add(n);
            vs.Add(v2); ns.Add(n);
            vs.Add(v3); ns.Add(n);

            vs.Add(v1); ns.Add(n);
            vs.Add(v3); ns.Add(n);
            vs.Add(v4); ns.Add(n);
        }

        public static CadObject CreateFace(
            Vector3 v1,
            Vector3 v2,
            Vector3 v3,
            Vector3 v4,
            Vector3 n,
            String name = "Plane",
            bool reverse = false)
        {
            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            Face(v1, v2, v3, v4, n, vs, ns);

            if (reverse)
                Face(v4, v3, v2, v1, n, vs, ns);

            return new CadObject(vs.ToArray(), ns.ToArray(), name);
        }

        public static CadObject CreateRectangularPrism(float dim_x, float dim_y, float dim_z, String name = "Rectangle Prism")
        {
            dim_x /= 2;
            dim_y /= 2;
            dim_z /= 2;

            Vector3 n;

            List<Vector3> vs = new List<Vector3>();
            List<Vector3> ns = new List<Vector3>();

            n = Vector3.UnitZ;
            Face(
                new Vector3(-dim_x, dim_y, dim_z),
                new Vector3(-dim_x, -dim_y, dim_z),
                new Vector3(dim_x, -dim_y, dim_z),
                new Vector3(dim_x, dim_y, dim_z),
                n, vs, ns);

            n = -Vector3.UnitZ;
            Face(
                new Vector3(dim_x, dim_y, -dim_z),
                new Vector3(dim_x, -dim_y, -dim_z),
                new Vector3(-dim_x, -dim_y, -dim_z),
                new Vector3(-dim_x, dim_y, -dim_z),
                n, vs, ns);

            n = Vector3.UnitX;
            Face(
                new Vector3(dim_x, dim_y, dim_z),
                new Vector3(dim_x, -dim_y, dim_z),
                new Vector3(dim_x, -dim_y, -dim_z),
                new Vector3(dim_x, dim_y, -dim_z),
                n, vs, ns);

            n = -Vector3.UnitX;
            Face(
                new Vector3(-dim_x, dim_y, -dim_z),
                new Vector3(-dim_x, -dim_y, -dim_z),
                new Vector3(-dim_x, -dim_y, dim_z),
                new Vector3(-dim_x, dim_y, dim_z),
                n, vs, ns);

            n = Vector3.UnitY;
            Face(
                new Vector3(dim_x, dim_y, dim_z),
                new Vector3(dim_x, dim_y, -dim_z),
                new Vector3(-dim_x, dim_y, -dim_z),
                new Vector3(-dim_x, dim_y, dim_z),
                n, vs, ns);

            n = -Vector3.UnitY;
            Face(
                new Vector3(-dim_x, -dim_y, dim_z),
                new Vector3(-dim_x, -dim_y, -dim_z),
                new Vector3(dim_x, -dim_y, -dim_z),
                new Vector3(dim_x, -dim_y, dim_z),
                n, vs, ns);


            return new CadObject(vs.ToArray(), ns.ToArray(), name);

        }
    }
}
