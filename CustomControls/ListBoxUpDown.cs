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
    public partial class ListBoxUpDown : UserControl
    {
        public readonly object _SwapLock = new object();

        public bool _AddEnabled
        {
            get
            {
                return this.bAdd.Enabled;
            }
            set
            {
                this.bAdd.Enabled = value;
            }
        }

        public ListBoxUpDown()
        {
            InitializeComponent();
        }

        public IEnumerable<Object> Items
        {
            get
            {
                foreach (var ob in this.listBox1.Items)
                {
                    yield return ob;
                }
            }
        }

        public int Count
        {
            get
            {
                return this.listBox1.Items.Count;
            }
        }

        public void Add(Object o)
        {
            this.listBox1.Items.Add(o);
        }

        public Object SelectedItem
        {
            get
            {
                return this.listBox1.SelectedItem;
            }
            set
            {
                this.listBox1.SelectedItem = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this.listBox1.SelectedIndex;
            }
            set
            {
                this.listBox1.SelectedIndex = value;
            }
        }

        public event EventHandler _SelectedItemChanged;

        private void clb_SelectedValueChanged(object sender, EventArgs e)
        {
            var si = this.listBox1.SelectedItem;

            if (si == null) this.bRemove.Enabled = false;
            else this.bRemove.Enabled = this.listBox1.Items.Count != 0;

            this.bUp.Enabled = si != null;
            this.bDown.Enabled = si != null;

            if (this._SelectedItemChanged != null)
                this._SelectedItemChanged(this, e);
        }


        private void bUp_Click(object sender, EventArgs e)
        {
            lock (this._SwapLock)
            {
                int current_index = this.listBox1.SelectedIndex;
                if (current_index < 1) return;

                var item = this.listBox1.SelectedItem;
                this.listBox1.Items.RemoveAt(current_index);

                current_index--;

                this.listBox1.Items.Insert(current_index, item);
                this.listBox1.SelectedIndex = current_index;
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            lock (this._SwapLock)
            {
                int current_index = this.listBox1.SelectedIndex;
                if (current_index < 0) return;
                if (current_index == this.listBox1.Items.Count - 1) return;

                var item = this.listBox1.SelectedItem;
                this.listBox1.Items.RemoveAt(current_index);

                current_index++;

                this.listBox1.Items.Insert(current_index, item);
                this.listBox1.SelectedIndex = current_index;
            }
        }

        public event ObjectRemoved _ObjectRemoved;
        private void bRemove_Click(object sender, EventArgs e)
        {
            var si = this.listBox1.SelectedItem;

            if (si != null)
            {
                this.listBox1.Items.Remove(si);

                if (this._ObjectRemoved != null)
                {
                    this._ObjectRemoved(this, si);
                }
            }
        }

        public event EventHandler _AddClick;
        private void bAdd_Click(object sender, EventArgs e)
        {
            if (this._AddClick != null)
                this._AddClick(this, e);
        }

        private void ListBoxUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                this.listBox1.SelectedItem = null;
        }
    }
}
