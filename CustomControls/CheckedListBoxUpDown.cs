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
    public delegate void ObjectRemoved(object sender, object Removed);

    public partial class CheckedListBoxUpDown : UserControl
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

        public CheckedListBoxUpDown()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Thread Safe!  Call on any thread.
        /// </summary>
        public IEnumerable<Object> CheckedItems
        {
            get
            {
                if (this.InvokeRequired)
                {
                    Logger.WriteLine("OH NO: CLBUP Not thread safe yet");
                    yield break;
                }
                else
                {
                    foreach (var o in this.checkedListBox1.CheckedItems)
                        yield return o;
                }
            }
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
                    Logger.WriteLine("OH NO: CLBUP Not thread safe yet");
                    yield break;
                }
                else
                {
                    foreach (var ob in this.checkedListBox1.Items)
                        yield return ob;
                }
            }
        }

        public Object ItemAt(int index)
        {            
            if (index >= 0)
                if (index < this.checkedListBox1.Items.Count)                    
                    return this.checkedListBox1.Items[index];

            return null;
        }

        public int Count
        {
            get
            {
                return this.checkedListBox1.Items.Count;
            }               
        }

        public void Add(Object o, bool v)
        {
            this.checkedListBox1.Items.Add(o, v);
        }

        public Object SelectedItem
        {
            get
            {
                return this.checkedListBox1.SelectedItem;
            }
            set
            {
                this.checkedListBox1.SelectedItem = value;
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this.checkedListBox1.SelectedIndex;
            }
            set
            {
                this.checkedListBox1.SelectedIndex = value;
            }
        }

        public event EventHandler _SelectedValueChanged;

        private void clb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var si = this.checkedListBox1.SelectedItem;

            if (si == null) this.bRemove.Enabled = false;
            else this.bRemove.Enabled = this.checkedListBox1.Items.Count != 0;

            this.bUp.Enabled = si != null;
            this.bDown.Enabled = si != null;

            if (this._SelectedValueChanged != null)
                this._SelectedValueChanged(this, e);
        }

        public void SetItemChecked(int index, bool is_checked)
        {
            this.checkedListBox1.SetItemChecked(index, is_checked);
        }

        public bool GetItemChecked(int index)
        {
            return this.checkedListBox1.GetItemChecked(index);
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            int current_index = this.checkedListBox1.SelectedIndex;
            if (current_index < 1) return;

            bool is_checked = this.checkedListBox1.GetItemChecked(current_index);
            var item = this.checkedListBox1.SelectedItem;
            this.checkedListBox1.Items.RemoveAt(current_index);

            current_index--;

            this.checkedListBox1.Items.Insert(current_index, item);
            this.checkedListBox1.SetItemChecked(current_index, is_checked);
            this.checkedListBox1.SelectedIndex = current_index;
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            int current_index = this.checkedListBox1.SelectedIndex;
            if (current_index < 0) return;
            if (current_index == this.checkedListBox1.Items.Count - 1) return;

            bool is_checked = this.checkedListBox1.GetItemChecked(current_index);
            var item = this.checkedListBox1.SelectedItem;
            this.checkedListBox1.Items.RemoveAt(current_index);

            current_index++;

            this.checkedListBox1.Items.Insert(current_index, item);
            this.checkedListBox1.SetItemChecked(current_index, is_checked);
            this.checkedListBox1.SelectedIndex = current_index;
        }

        public event ObjectRemoved _ObjectRemoved;
        private void bRemove_Click(object sender, EventArgs e)
        {
            var si = this.checkedListBox1.SelectedItem;

            if (si != null)
            {
                this.checkedListBox1.Items.Remove(si);
                if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
            }
        }

        private void CheckedListBoxUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                this.checkedListBox1.SelectedItem = null;
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAll();
        }

        public void ClearAll()
        {
            while (this.checkedListBox1.Items.Count > 0)
            {
                object si = this.checkedListBox1.Items[0];
                this.checkedListBox1.Items.RemoveAt(0);
                if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
            }
        }

        public event EventHandler _AddClick;
        private void bAdd_Click(object sender, EventArgs e)
        {
            if (this._AddClick != null)
                this._AddClick(this, e);
        }

        public event ItemCheckEventHandler _ItemCheck;
        private bool _InCheckEvent = false;
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // For some reason, the ItemCheck event is called before the CheckState is updates.
            // So we manually update it.
            if (this._InCheckEvent) return;
            this._InCheckEvent = true;

            this.checkedListBox1.SetItemCheckState(e.Index, e.NewValue);
            if (this._ItemCheck != null)
                this._ItemCheck(sender, e);

            this._InCheckEvent = false;
        }

        private void CheckedListBoxUpDown_Load(object sender, EventArgs e)
        {
            this.checkedListBox1.ContextMenuStrip = this.contextMenuStrip1;
        }
    }
}
