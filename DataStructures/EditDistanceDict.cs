using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.DataStructures
{
    /// <summary>
    /// Regular dictionary, except it has a default value.
    /// </summary>
    public class EditDistanceDict<TValue> : Dictionary<String, TValue>
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
                if (key == null) throw new Exception(this.GetType().FullName + " Null Key");
                else if (this.Count == 0) throw new Exception(this.GetType().FullName + " Empty");
                else if (this.TryGetValue(key, out t)) return t;
                else
                {
                    float best_edit = float.MaxValue;

                    KeyValuePair<String, TValue> best_kvp = new KeyValuePair<string, TValue>(null, default(TValue));

                    foreach (var kvp in this)
                    {
                        float edit = NLP.EditDistance.Calculate(
                            kvp.Key,
                            key
                            );

                        if (edit < best_edit)
                        {
                            best_edit = edit;
                            best_kvp = kvp;
                        }
                    }   
                                    
                    this[key] = best_kvp.Value;
                    this._NewKeyMapping?.Invoke(this, key, best_kvp.Key);
                    return best_kvp.Value;
                }
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
