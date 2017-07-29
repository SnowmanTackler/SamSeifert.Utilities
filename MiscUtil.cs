using SamSeifert.Utilities.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class MiscUtil
    {
        public static bool InRange(this int i, int low, int high)
        {
            return (i >= low) && (i < high);
        }

        public static void Swap<T>(ref T i, ref T o)
        {
            T temp = i;
            i = o;
            o = temp;
        }

        public static String GetExecutablePath()
        {
            return System.Reflection.Assembly.GetEntryAssembly().Location;
        }

        public struct Merge
        {
            /// <summary>
            /// Always Lower or Equal
            /// </summary>
            public int _Index1;

            /// <summary>
            /// Always Greater or Equal
            /// </summary>
            public int _Index2;

            public Merge(int index1, int index2)
            {
                this._Index1 = index1;
                this._Index2 = index2;

                if (index1 > index2)
                    MiscUtil.Swap(ref this._Index1, ref this._Index2);
            }
        }


        /// <summary>
        /// Given a bunch of inital groups, and some merges between groups, remap to new groups!
        /// </summary>
        public static DefaultDict<int, int> FindMapFromMerges( IEnumerable<Merge> merges)
        { 
            var temp_mapping = new Dictionary<int, int>();

            var additional_merges = new List<System.Drawing.Point>();

            foreach (var merge in merges)
            {
                int cmap;

                if (temp_mapping.TryGetValue(merge._Index2, out cmap))
                {
                    // 4 maps to 3, but also 4 maps 0
                    // 3 maps to 2
                    // 4 3 and 2 should map to zero;
                    if (cmap != merge._Index1)
                        additional_merges.Add(new System.Drawing.Point(cmap, merge._Index1));
                    if (cmap >= merge._Index1)
                        continue; // Pick Lowest To Set To
                }

                temp_mapping[merge._Index2] = merge._Index1;
            }

            var mapping = new DefaultDict<int, int>((int key) => key);

            foreach (var kvp in temp_mapping)
            {
                int map_to;
                int new_map_to = kvp.Value;

                do { map_to = new_map_to; }
                while (temp_mapping.TryGetValue(map_to, out new_map_to));

                mapping[kvp.Key] = map_to;
            }

            if (additional_merges.Count != 0)
            {
                var new_merges = new List<Merge>();

                foreach (var merge in additional_merges)
                {
                    int n1 = mapping[merge.X];
                    int n2 = mapping[merge.Y];
                    if (n1 != n2) // Could have worked out on its own
                        new_merges.Add(new Merge(n1, n2));
                }

                if (new_merges.Count != 0)
                {
                    var new_mapping = new DefaultDict<int, int>((int key) => key);
                    var new_dict = FindMapFromMerges(new_merges);
                    foreach (var kvp in mapping)
                        new_mapping[kvp.Key] = new_dict[kvp.Value];
                    return new_mapping;
                }
            }

            return mapping;
        }
    }
}
