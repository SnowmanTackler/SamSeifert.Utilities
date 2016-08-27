using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.CustomControls
{
    public class EnumPicker<T> : ComboBox where T : struct
    {
        private T[] _Ts = null;
        private readonly T _Default;

        public EnumPicker(T default_T) : base()
        {
            this._Default = default_T;

            var ls = new List<T>();

            int select = 0;

            foreach (var ev in EnumUtil.GetValues<T>())
            {
                this.Items.Add(ev.GetDescription());

                if (this._Default.Equals(ev))
                    select = ls.Count;

                ls.Add(ev);
            }

            this._Ts = ls.ToArray();

            this.SelectedIndex = select;
        }

        public T _T
        {
            get
            {
                int dex = this.SelectedIndex;

                if (dex < 0) return this._Default;
                if (dex >= this.Items.Count) return this._Default;
                return this._Ts[dex];
            }
        }
    }
}
