using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SamSeifert.Utilities.Logging;

namespace SamSeifert.Utilities.CustomControls
{
    public partial class ListBoxUpDown : UserControl
    {
        public event EventHandler _SelectedItemChanged;
        public event ObjectRemoved _ObjectRemoved;
        public event EventHandler _AddClick;
        public event DuplicateEvent _Duplicate;

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
                foreach (var ob in this.listBox1.Items)
                    yield return ob;
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

                switch (value)
                {
                    case SelectionMode.MultiExtended:
                    case SelectionMode.MultiSimple:
                        this.selectAllToolStripMenuItem.Enabled = true;
                        break;
                    default:
                        this.selectAllToolStripMenuItem.Enabled = false;
                        break;
                }
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
            if (this.listBox1.DataSource == null) return;

            using (this.Suspender())
            {
                int cnt = this.listBox1.Items.Count;
                var sel = new bool[cnt];
                for (int i = 0; i < cnt; i++)
                    sel[i] = this.listBox1.GetSelected(i);

                String temp_member = this.listBox1.DisplayMember;
                this.listBox1.DisplayMember = "";
                this.listBox1.DisplayMember = temp_member;

                for (int i = 0; i < cnt; i++)
                    this.listBox1.SetSelected(i, sel[i]);
            }
        }


        private void clb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var si = this.listBox1.SelectedItem;

            this.bRemove.Enabled = (si != null) && (this.listBox1.Items.Count != 0);
            this.duplicateToolStripMenuItem.Enabled = this.bRemove.Enabled && (this._Duplicate != null);

            this.bUp.Enabled = si != null;
            this.bDown.Enabled = si != null;

            this._SelectedItemChanged?.Invoke(this, e);
        }


        private void bUp_Click(object sender, EventArgs e)
        {
            if (this.listBox1.DataSource == null)
            {
                using (this.Suspender())
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
            else
            {
                Logger.Default.Warn("No DataSource");
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (this.listBox1.DataSource == null)
            {
                using (this.Suspender())
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
            else
            {
                Logger.Default.Warn("No DataSource");
            }
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            using (this.Suspender())
            {
                foreach (var si in this.listBox1.SelectedItems.Cast<object>().ToArray()) // Cast to array so we can modify the control
                {
                    if (this.listBox1.DataSource == null)
                    {
                        this.listBox1.Items.Remove(si);
                        this._ObjectRemoved?.Invoke(this, si);
                    }
                    else
                    {
                        if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
                        else Logger.Default.Warn("Needs to implement _ObjectRemoved");
                    }
                }
            }

            this._SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ListBoxUpDown_Load(object sender, EventArgs e)
        {
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
        }

        private void deleteAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ClearAll();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (this.Suspender())
                for (int i = 0; i < this.listBox1.Items.Count; i++)
                    this.listBox1.SetSelected(i, true);
            this._SelectedItemChanged?.Invoke(this, e);
        }

        public void ClearAll()
        {
            using (this.Suspender())
            { 
                while (this.listBox1.Items.Count > 0)
                {
                    object si = this.listBox1.Items[0];

                    if (this.listBox1.DataSource == null)
                    {
                        this.listBox1.Items.RemoveAt(0);
                        this._ObjectRemoved?.Invoke(this, si);
                    }
                    else
                    {
                        if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
                        else Logger.Default.Warn("Needs to implement _ObjectRemoved");
                    }
                }
            }

            this._SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

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

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sel = this.listBox1.SelectedItem;
            if (sel != null)
                this._Duplicate?.Invoke(this, sel);
        }

        public void ClearSelected()
        {
            this.listBox1.ClearSelected();
        }

        public void SetSelected(int index, bool v)
        {
            this.listBox1.SetSelected(index, v);
        }

        public bool GetSelected(int i)
        {
            return this.listBox1.GetSelected(i);
        }

        public IDisposable Suspender()
        {
            return new DisposableCollection(
                new LayoutSuspender(this),
                new EventSuspender(this, nameof(this._SelectedItemChanged))
                );
        }
    }
}
