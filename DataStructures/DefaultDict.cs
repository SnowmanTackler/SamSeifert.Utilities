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

        public bool ArgMax(out TKey key, out TValue value)
        {
            key = default(TKey);
            value = default(TValue);

            if (!typeof(IComparable).IsAssignableFrom(typeof(TValue)))
                return false;

            bool first = true;
            foreach (var kvp in this)
            {
                if (first) first = false;
                else if ((kvp.Value as IComparable).CompareTo(value) < 0) continue;
                key = kvp.Key;
                value = kvp.Value;
            }

            return true;
        }

        public bool ArgMin(out TKey key, out TValue value)
        {
            key = default(TKey);
            value = default(TValue);

            if (!typeof(IComparable).IsAssignableFrom(typeof(TValue)))
                return false;

            bool first = true;
            foreach (var kvp in this)
            {
                if (first) first = false;
                else if ((kvp.Value as IComparable).CompareTo(value) > 0) continue;
                key = kvp.Key;
                value = kvp.Value;
            }

            return true;
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
    }
}
