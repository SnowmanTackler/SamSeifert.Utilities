using SamSeifert.Utilities.Concurrent;
using SamSeifert.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.MultiThreading
{
    public class BackgroundQueueForm : BackgroundQueue
    {
        private readonly Form Parent;
        public BackgroundQueueForm(
            Form f,
            String name,
            ThreadPriority tp = ThreadPriority.Normal,
            int threads = 1) : base(name, tp, threads)
        {
            if (f == null)
                throw new ArgumentNullException();

            f.FormClosing += this.FormClosing;

            this.Parent = f;
        }

        private void FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Stop();

            if (this.HasWorkingThreads)
            {
                // Prevent two children from creating a cascading number of BeginInvokes by
                // only executing this code if no one else has.
                if (!e.Cancel)
                {
                    e.Cancel = true;
                    this.Parent.BeginInvoke(new Action(() =>
                    {
                        this.Parent.Close();
                    }));
                }
            }
        }

        public override bool RunOnMainThread(Action a)
        {
            this.Parent.BeginInvoke(a);
            return true;
        }
    }
}
