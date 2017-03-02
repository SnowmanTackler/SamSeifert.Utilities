using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace SamSeifert.CSCV
{
    public static partial class SingleImage
    {
        /// <summary>
        /// Specials Errors: Couldn't Initialize different clusters!
        /// </summary>
        /// <param name="inpt"></param>
        /// <param name="K"></param>
        /// <param name="outp"></param>
        /// <returns></returns>
        public static ToolboxReturn KMeans(
            Sect inpt, 
            ref Sect outp,
            int K,
            int max_iterations = 100
            )
        {
            if (inpt == null)
            {
                outp = null;
                return ToolboxReturn.NullInput;
            }
            else
            {
                MatchOutputToInput(inpt, ref outp);

                var sz = outp.getPrefferedSize();
                int w = sz.Width;
                int h = sz.Height;

                var in_sects = new List<Sect>();
                var out_sects = new List<Sect>();

                if (inpt._Type == SectType.Holder)
                {
                    var sh = inpt as SectHolder;
                    foreach (var inp in sh.Sects.Values)
                    {
                        in_sects.Add(inp);
                        out_sects.Add((outp as SectHolder).getSect(inp._Type));
                    }
                }
                else
                {
                    in_sects.Add(inpt);
                    out_sects.Add(outp);
                }

                int pixels = w * h;
                var pixels_vectors = new Vector<float>[pixels];
                var pixels_cluster = new int[pixels];

                for (int p = 0; p < pixels; p++)
                    pixels_vectors[p] = Vector<float>.Build.Dense(in_sects.Count);

                for (int s = 0; s < in_sects.Count; s++)
                {
                    int p = 0;
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            pixels_vectors[p++][s] = in_sects[s][y, x];
                }


                var new_cluster_centers = new Vector<float>[K];
                var new_cluster_counts = new int[K];
                var cluster_centers = new Vector<float>[K];


                // Initialize Clusters
                {
                    // Try to initialize K clusters! (give us 10 tries)
                    bool found = false;
                    Random r = new Random(Environment.TickCount);
                    for (int tries = 0; (tries < 10) && (!found); tries++)
                    {
                        found = true;
                        for (int k = 0; (k < K) && found; k++)
                        {
                            cluster_centers[k] = pixels_vectors[r.Next(pixels)];

                            for (int checker = 0; (checker < k) && found; checker++)
                                if (cluster_centers[k].Equals(cluster_centers[checker]))
                                    found = false;
                        }
                    }

                    if (!found) return ToolboxReturn.SpecialError;
                }

                // Run K Means.  Max 100 iterations
                for (int iterations = 0; iterations < max_iterations; iterations++)
                {
                    // Assign clusters
                    for (int k = 0; k < K; k++)
                    {
                        new_cluster_centers[k] = Vector<float>.Build.Dense(in_sects.Count, 0);
                        new_cluster_counts[k] = 0;
                    }

                    bool no_changes = true;

                    for (int p = 0; p < pixels; p++)
                    {
                        double best_dist = double.MaxValue;
                        int best_cluster = 0;

                        for (int k = 0; k < K; k++)
                        {
                            double dist = (pixels_vectors[p] - cluster_centers[k]).L2Norm();
                            if (dist < best_dist)
                            {
                                best_dist = dist;
                                best_cluster = k;
                            }
                        }

                        new_cluster_centers[best_cluster] += pixels_vectors[p];
                        new_cluster_counts[best_cluster]++;

                        if (pixels_cluster[p] != best_cluster)
                        {
                            no_changes = false;
                            pixels_cluster[p] = best_cluster;
                        }
                    }

                    if (no_changes) // We're done here!
                        break;

                    for (int k = 0; k < K; k++)
                    {
                        if (new_cluster_counts[k] == 0) return ToolboxReturn.SpecialError; // Nothing in cluster!
                        cluster_centers[k] = new_cluster_centers[k] / new_cluster_counts[k];
                    }
                }

                for (int s = 0; s < out_sects.Count; s++)
                {
                    int p = 0;
                    for (int y = 0; y < h; y++)
                        for (int x = 0; x < w; x++)
                            out_sects[s][y, x] = cluster_centers[pixels_cluster[p++]][s];
                }


                return ToolboxReturn.Good;
            }
        }
    }
}
