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

        /// <summary>
        /// Thread Safe!  Call on any thread.
        /// </summary>
        public IEnumerable<Object> Items
        {
            get
            {
                if (this.InvokeRequired)
                {
                    Logger.WriteLine("OH NO: LBUP Not thread safe yet");
                    yield break;
                }
                else
                {
                    foreach (var ob in this.listBox1.Items)
                        yield return ob;
                }
            }
        }


        public Object ItemAt(int index)
        {
            if (index >= 0)
                if (index < this.listBox1.Items.Count)
                    return this.listBox1.Items[index];
            return null;
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

        public System.Windows.Forms.SelectionMode SelectionMode
        {
            get
            {
                return this.listBox1.SelectionMode;
            }
            set
            {
                this.listBox1.SelectionMode = value;
            }
        }

        public ListBox.SelectedObjectCollection SelectedItems
        {
            get
            {
                return this.listBox1.SelectedItems;
            }
        }

        public string DisplayMember
        {
            set
            {
                this.listBox1.DisplayMember = value;
            }
        }

        public object DataSource
        {
            set
            {
                this.listBox1.DataSource = value;
            }
        }

        /// <summary>
        /// Only works with DataSource!
        /// </summary>
        public void RefreshNames()
        {
            var temp_event = this._SelectedItemChanged;
            this._SelectedItemChanged = null;

            int cnt = this.listBox1.Items.Count;
            var sel = new bool[cnt];
            for (int i = 0; i < cnt; i++)
                sel[i] = this.listBox1.GetSelected(i);

            String temp_member = this.listBox1.DisplayMember;
            this.listBox1.DisplayMember = "";
            this.listBox1.DisplayMember = temp_member;

            for (int i = 0; i < cnt; i++)
                this.listBox1.SetSelected(i, sel[i]);

            this._SelectedItemChanged = temp_event;
        }

        public event EventHandler _SelectedItemChanged;

        private void clb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var si = this.listBox1.SelectedItem;

            this.bRemove.Enabled = (si != null) && (this.listBox1.Items.Count != 0);
            this.duplicateToolStripMenuItem.Enabled = this.bRemove.Enabled && (this._Duplicate != null);

            this.bUp.Enabled = si != null;
            this.bDown.Enabled = si != null;

            if (this._SelectedItemChanged != null)
                this._SelectedItemChanged(this, e);
        }


        private void bUp_Click(object sender, EventArgs e)
        {
            int current_index = this.listBox1.SelectedIndex;
            if (current_index < 1) return;

            var item = this.listBox1.SelectedItem;
            this.listBox1.Items.RemoveAt(current_index);

            current_index--;

            this.listBox1.Items.Insert(current_index, item);
            this.listBox1.SelectedIndex = current_index;
        }

        private void bDown_Click(object sender, EventArgs e)
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

        public event ObjectRemoved _ObjectRemoved;
        private void bRemove_Click(object sender, EventArgs e)
        {
            var si = this.listBox1.SelectedItem;

            if (si != null)
            {
                if (this.listBox1.DataSource == null)
                {
                    this.listBox1.Items.Remove(si);
                    if (this._ObjectRemoved != null)
                        this._ObjectRemoved(this, si);
                }
                else
                {
                    if (this._ObjectRemoved != null)
                        this._ObjectRemoved(this, si);
                    else Logger.WriteLine("ListBoxUpDown with DataSource needs to implement _ObjectRemoved");
                }

            }
        }

        private void ListBoxUpDown_Load(object sender, EventArgs e)
        {
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAll();
        }

        public void ClearAll()
        {
            while (this.listBox1.Items.Count > 0)
            {
                object si = this.listBox1.Items[0];
                this.listBox1.Items.RemoveAt(0);
                if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
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

        public event DuplicateEvent _Duplicate;
        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sel = this.listBox1.SelectedItem;
            if (sel != null)
                this._Duplicate?.Invoke(this, sel);
        }
    }
}
