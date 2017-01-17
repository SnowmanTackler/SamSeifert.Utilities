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

        public EnumPicker(T default_T, params T[] ignore_t) : base()
        {
            this._Default = default_T;

            // Overloaded Enums
            var dupes = new HashSet<T>();
            var ls_sort = new List<Tuple<T, String>>();

            foreach (var ev in EnumUtil.GetValues<T>())
            {
                if (dupes.Contains(ev)) continue;
                if (ignore_t.Contains(ev)) continue;
                dupes.Add(ev);
                ls_sort.Add(new Tuple<T, string>(ev, ev.GetDescription()));
            }

            ls_sort.Sort((x, y) => {
                return x.Item2.CompareTo(y.Item2);
            });


            var ls = new List<T>();
            int select = 0;
            foreach (var tup in ls_sort)
            {
                this.Items.Add(tup.Item2);

                if (this._Default.Equals(tup.Item1))
                    select = ls.Count;

                ls.Add(tup.Item1);
            }

            this._Ts = ls.ToArray();

            this.SelectedIndex = select;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
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
            set
            {
                this.SelectedItem = value.GetDescription();
            }
        }

        public string _Description
        {
            get
            {
                return EnumUtil.GetDescription(this._T);
            }
            set
            {
                this.SelectedItem = value;
            }
        }
    }
}
