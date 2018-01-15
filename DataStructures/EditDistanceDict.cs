using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.DataStructures
{
    /// <summary>
    /// Regular dictionary, except it has a default value.
    /// Tuple is (best value, original key for best value, edit distance between original and query and key)
    /// This queried keys will be saved in the dictionary.
    /// </summary>
    public class EditDistanceDict<TValue> : Dictionary<String, Tuple<TValue, String, float>>  
    {
        public delegate void NewKeyMapping(object sender, String user_key, String mapped_to_key);
        public event NewKeyMapping _NewKeyMapping;

        public EditDistanceDict()
        {
        }       

        public new TValue this[String key]
        {
            get
            {
                TValue t;
                String best_key;
                this.Get(key, out best_key, out t);
                return t;
            }
            set
            {
                base[key] = new Tuple<TValue, string, float>(value, key, 0);
            }
        }

        public bool TryGetValue(String key, out TValue t)
        {
            Tuple<TValue, String, float> tup;
            bool returned = base.TryGetValue(key, out tup);
            t = tup.Item1;
            return returned;
        }

        public float Get(String key, out String best_key, out TValue best_value)
        {
            Tuple<TValue, String, float> tup;

            if (key == null) throw new Exception(this.GetType().FullName + " Null Key");
            else if (this.Count == 0) throw new Exception(this.GetType().FullName + " Empty");
            else if (this.TryGetValue(key, out tup))
            {
                // the edit dist dictionary will add keys for items it doens't have, so edit distance might not necessarily be zero
                best_value = tup.Item1;
                best_key = tup.Item2;
                return tup.Item3;
            }
            else
            {
                float best_edit = float.MaxValue;

                best_key = null;
                best_value = default(TValue);

                foreach (var kvp in this)
                {
                    if (kvp.Value.Item3 != 0) continue; // Ignore any of our own entries.

                    float edit = NLP.EditDistance.Calculate(
                        kvp.Key,
                        key
                        );

                    if (edit < best_edit)
                    {
                        best_edit = edit;
                        best_key = kvp.Key;
                        best_value = kvp.Value.Item1;
                    }
                }

                base[key] = new Tuple<TValue, string, float>(best_value, best_key, best_edit);
                this._NewKeyMapping?.Invoke(this, key, best_key);
                return best_edit;
            }
        }
    }
}
