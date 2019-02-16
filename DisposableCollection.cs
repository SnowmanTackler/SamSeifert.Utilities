using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities
{
    /// <summary>
    /// Disposable Collection.  Disposable Collector
    /// </summary>
    public class DisposableCollection : IDisposable
    {
        private IDisposable[] _Stuff;

        public DisposableCollection(params IDisposable[] stuff)
        {
            this._Stuff = stuff;
        }

        public void Dispose()
        {
            foreach (var d in this._Stuff)
                d.Dispose();
        }
    }
}
