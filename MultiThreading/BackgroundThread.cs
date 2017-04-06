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
        private volatile bool _Working = true;
        private volatile bool _ShouldContinue = true;

        public BackgroundThread(
            BackgroundThreadMethod meth, 
            Form f, 
            String name = null)
        {
            this.Setup(meth, f, name);
        }

        public BackgroundThread(
            BackgroundThreadMethodOut meth,
            object param,
            BackgroundThreadFinisher fin,
            Form f,
            String name = null)
        {
            this.Setup(meth, fin, f, name);
        }

        public BackgroundThread(
            ParameterizedBackgroundThreadMethod meth,
            object param, 
            Form f,
            String name = null)
        {
            this.Setup((ContinueCheck continue_on) => {
                meth(continue_on, param);
            }, f, name);
        }

        public BackgroundThread(
                ParameterizedBackgroundThreadMethodOut meth,
                object param,
                BackgroundThreadFinisher fin,
                Form f,
                String name = null)
        {
            this.Setup((ContinueCheck continue_on) => {
                return meth(continue_on, param);
            }, fin, f, name);
        }




        /// <summary>
        /// Called in Constructor
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="param"></param>
        private void Setup(BackgroundThreadMethod meth, Form f, String name)
        {
            this.Setup((ContinueCheck c) =>
            {
                meth(c);
                return null;
            },
            null, f, name);
        }

        private void Setup(BackgroundThreadMethodOut meth, BackgroundThreadFinisher fin, Form f, String name)
        {
            if (f == null)
                throw new ArgumentNullException();

            f.FormClosing += FormClosing;

            (new Thread(this._BackgroundThread)).Start(new object[] {
                meth,
                fin,
                f,
                name                
            });

        }

        private void _BackgroundThread(object oargs)
        {
            var args = oargs as object[];
            Thread.CurrentThread.Name = (args[3] == null) ? "Background Thread" : args[3] as String;

            var method = args[0] as BackgroundThreadMethodOut;
            var finish = args[1] as BackgroundThreadFinisher;
            var form = args[2] as Form;

            var ob = method(this._ShouldContinueMethod);

            form.Invoke(
                (Action)(() => {
                if (finish != null)
                    finish(ob);
                form.FormClosing -= this.FormClosing;
            }));

            this._Working = false;
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
                return this._Working;
            }
        }
    }
}
