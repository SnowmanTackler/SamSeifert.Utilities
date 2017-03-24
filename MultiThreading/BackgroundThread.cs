using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SamSeifert.Utilities.MultiThreading
{
    public class BackgroundThread
    {
        private volatile Thread _Thread = null;
        private volatile bool _ShouldContinue = true;

        private readonly String _Name;
        private Form _ClosingForm;

        public BackgroundThread(BackgroundThreadMethod meth, Form f, String name = null)
        {
            this._Name = name;
            this._ClosingForm = f;
            this.Setup(meth);
        }

        public BackgroundThread(BackgroundThreadMethodWithParam meth, object param, Form f, String name = null)
        {
            this._Name = name;
            this._ClosingForm = f;
            this.Setup((ContinueCheck continue_on) => {
                meth(continue_on, param);
            });
        }

        /// <summary>
        /// Called in Constructor
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="param"></param>
        private void Setup(BackgroundThreadMethod meth)
        {
            if (this._ClosingForm != null)
                this._ClosingForm.FormClosing += FormClosing;

            this._Thread = new Thread(this._BackgroundThread);
            this._Thread.Start(meth);
        }

        private void _BackgroundThread(object oargs)
        {
            Thread.CurrentThread.Name = (this._Name == null) ? "Background Thread" : this._Name;

            var args = oargs as BackgroundThreadMethod;

            args(this._ShouldContinueMethod);

            this._Thread = null;

            if (this._ClosingForm != null)
            {
                Action a = () =>
                {
                    this._ClosingForm.FormClosing -= this.FormClosing;
                    this._ClosingForm = null;
                };

                if (this._ClosingForm.IsDisposed) return;
                else if (this._ClosingForm.InvokeRequired) this._ClosingForm.BeginInvoke(a);
                else a();
            }
        }

        private bool _ShouldContinueMethod()
        {
            return this._ShouldContinue;
        }

        private void FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            this._ShouldContinue = false;

            if (this._ThreadSafe_Running)
            {
                e.Cancel = true;
            }
        }

        public void _ThreadSafe_Stop()
        {
            this._ShouldContinue = false;
        }


        public bool _ThreadSafe_Running
        {
            get
            {
                return this._Thread != null;
            }
        }
    }
}
