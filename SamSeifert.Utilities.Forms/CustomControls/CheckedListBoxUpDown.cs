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
    public delegate void ObjectRemoved(object sender, object removed);
    public delegate void DuplicateEvent(object sender, object args);

    public partial class CheckedListBoxUpDown : UserControl
    {
        public event ItemCheckEventHandler _ItemCheck;
        public event DuplicateEvent _Duplicate;
        public event ObjectRemoved _ObjectRemoved;
        public event EventHandler _SelectedItemChanged;
        public event EventHandler _AddClick;

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

        public void RemoveAt(int index )
        {
            this.checkedListBox1.Items.RemoveAt(index);
        }

        public void Insert(int index, object o,bool check)
        {
            this.checkedListBox1.Items.Insert(index, o);
            this.checkedListBox1.SetItemChecked(index, check);
        }

        public void ReplaceAt(int index, object o, bool check)
        {
            this.SuspendLayout();
            int i = this.SelectedIndex;
            this.Insert(index, o, check);
            this.RemoveAt(index + 1);
            this.SelectedIndex = i;
            this.ResumeLayout();
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

        public System.Windows.Forms.SelectionMode SelectionMode
        {
            get
            {
                return this.checkedListBox1.SelectionMode;
            }
            set
            {
                this.checkedListBox1.SelectionMode = value;

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
                return this.checkedListBox1.SelectedItems;
            }
        }

        public string DisplayMember
        {
            set
            {
                this.checkedListBox1.DisplayMember = value;
            }
        }

        public object DataSource
        {
            set
            {
                this.checkedListBox1.DataSource = value;
            }
        }

        /// <summary>
        /// Only works with DataSource!
        /// </summary>
        public void RefreshNames()
        {
            if (this.checkedListBox1.DataSource == null) return;

            using (this.Suspender())
            {
                int cnt = this.checkedListBox1.Items.Count;
                var sel = new bool[cnt];
                for (int i = 0; i < cnt; i++)
                    sel[i] = this.checkedListBox1.GetSelected(i);

                String temp = this.checkedListBox1.DisplayMember;
                this.checkedListBox1.DisplayMember = "";
                this.checkedListBox1.DisplayMember = temp;

                for (int i = 0; i < cnt; i++)
                    this.checkedListBox1.SetSelected(i, sel[i]);
            }
        }

        private void clb_SelectedIndexChanged(object sender, EventArgs e)
        {
            var si = this.checkedListBox1.SelectedItem;

            this.bRemove.Enabled = (si != null) && (this.checkedListBox1.Items.Count != 0);
            this.duplicateToolStripMenuItem.Enabled = this.bRemove.Enabled && (this._Duplicate != null);

            this.bUp.Enabled = si != null;
            this.bDown.Enabled = si != null;

            this._SelectedItemChanged?.Invoke(this, e);
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
            if (this.checkedListBox1.DataSource == null)
            {
                using (this.Suspender())
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
            }
            else
            {
                Logger.WriteError(this, "Down Click With DataSource");
            }
        }

        private void bDown_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.DataSource == null)
            {
                using (this.Suspender())
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
            }
            else
            {
                Logger.WriteError(this, "Down Click With DataSource");
            }
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            using (this.Suspender())
            {
                foreach (var si in this.checkedListBox1.SelectedItems.Cast<object>().ToArray()) // Cast to array so we can modify the control
                {
                    if (this.checkedListBox1.DataSource == null)
                    {
                        this.checkedListBox1.Items.Remove(si);
                        this._ObjectRemoved?.Invoke(this, si);
                    }
                    else
                    {
                        if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
                        else Logger.WriteLine("CheckedListBoxUpDown with DataSource needs to implement _ObjectRemoved");
                    }
                }
            }

            this._SelectedItemChanged?.Invoke(this, EventArgs.Empty);
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

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (this.Suspender())
                for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
                    this.checkedListBox1.SetSelected(i, true);
            this._SelectedItemChanged?.Invoke(this, e);
        }

        public void ClearAll()
        {
            using (this.Suspender())
            {
                while (this.checkedListBox1.Items.Count > 0)
                {
                    object si = this.checkedListBox1.Items[0];

                    if (this.checkedListBox1.DataSource == null)
                    {
                        this.checkedListBox1.Items.RemoveAt(0);
                        this._ObjectRemoved?.Invoke(this, si);
                    }
                    else
                    {
                        if (this._ObjectRemoved != null) this._ObjectRemoved(this, si);
                        else Logger.WriteLine("ListBoxUpDown with DataSource needs to implement _ObjectRemoved");
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

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sel = this.checkedListBox1.SelectedItem;
            if (sel != null)
                this._Duplicate?.Invoke(this, sel);
        }

        public void ClearSelected()
        {
            this.checkedListBox1.ClearSelected();
        }

        public void SetSelected(int index, bool v)
        {
            this.checkedListBox1.SetSelected(index, v);
        }

        public bool GetSelected(int i)
        {
            return this.checkedListBox1.GetSelected(i);
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
