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
    public class DefaultDict<TKey, TValue> : Dictionary<TKey, TValue>
    {
        private readonly Func<TKey, TValue> _DefaultFunc;

        public DefaultDict(Func<TValue> default_function)
        {
            this._DefaultFunc = (TKey k) => default_function();
        }

        public DefaultDict(Func<TKey, TValue> default_function)
        {
            this._DefaultFunc = default_function;
        }

        public DefaultDict(TValue default_value)
            : base()
        {
            this._DefaultFunc = (TKey k) => default_value;            
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue t;
                if (this.TryGetValue(key, out t)) return t;
                else
                {
                    var newel = this._DefaultFunc(key);
                    this[key] = newel;
                    return newel;
                }
            }
            set
            {
                base[key] = value;
            }
        }

        /// <summary>
        /// Returns value for key if in dictionary, or default value (null).
        /// Handy when using dict.TryGetValue(key)?.Method();
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue TryGetValue(TKey key)
        {
            TValue t;
            if (this.TryGetValue(key, out t)) return t;
            else return default(TValue); // Null or Zero typically
        }
    }
}
