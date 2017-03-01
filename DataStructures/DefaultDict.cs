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
        private readonly Func<TValue> _DefaultFunc;

        public DefaultDict(Func<TValue> default_function)
        {
            this._DefaultFunc = default_function;
        }

        public DefaultDict(TValue default_value)
            : base()
        {
            this._DefaultFunc = new Func<TValue>(() =>
            {
                return default_value;
            });
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue t;
                if (this.TryGetValue(key, out t)) return t;
                else
                {
                    var newel = this._DefaultFunc();
                    this[key] = newel;
                    return newel;
                }
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
