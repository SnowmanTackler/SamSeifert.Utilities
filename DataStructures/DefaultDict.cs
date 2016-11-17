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
        private readonly TValue _DefaultValue;

        public DefaultDict(TValue default_value)
            : base()
        {
            this._DefaultValue = default_value;
        }

        public new TValue this[TKey key]
        {
            get
            {
                TValue t;
                if (this.TryGetValue(key, out t)) return t;
                else return this._DefaultValue;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
