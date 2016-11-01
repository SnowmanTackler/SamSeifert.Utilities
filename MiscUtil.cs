using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    public static class MiscUtil
    {
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
        public static Dictionary<int, int> FindMapFromMerges( IEnumerable<Merge> merges)
        {
           var temp_mapping = new Dictionary<int, int>();

            foreach (var merge in merges)
            {
                int cmap;

                if (temp_mapping.TryGetValue(merge._Index2, out cmap))
                    if (cmap <= merge._Index1)
                        continue;

                temp_mapping[merge._Index2] = merge._Index1;
            }

            var mapping = new Dictionary<int, int>();

            foreach (var kvp in temp_mapping)
            {
                int map_to;
                int new_map_to = kvp.Value;

                do { map_to = new_map_to; }
                while (temp_mapping.TryGetValue(map_to, out new_map_to));

                mapping[kvp.Key] = map_to;
            }

            return mapping;
        }


    }
}
