using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamSeifert.Utilities.CustomControls
{
    /// <summary>
    /// Proper use: using (new EventSuspender(this, nameof(this._SelectedItemChanged)))
    /// </summary>
    public class EventSuspender : IDisposable
    {
        private readonly object _Object;
        private readonly List<Delegate> _Subscribers = new List<Delegate>();
        private EventInfo _EventInfo;

        static BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }

        public void Dispose()
        {
            if (this._Object == null) return;
            if (this._EventInfo == null) return;

            foreach (Delegate del in this._Subscribers)
                this._EventInfo.AddEventHandler(this._Object, del);
        }

        /// <summary>
        /// Proper use: using (new EventSuspender(this, nameof(this._SelectedItemChanged)))
        /// this._SelectedItemChanged has to be field, not property!
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name_of_event"></param>
        public EventSuspender(object obj, string name_of_event) 
        {
            if (obj == null) return;

            Type t = obj.GetType();

            var ei = t.GetEvent(name_of_event);

            if (ei == null) return; // Should throw exception in the GetEvent method

            FieldInfo fi = ei.DeclaringType.GetField(ei.Name, AllBindings);

            if (fi == null) return;

            // Alot of the remainder of this constructor is taken from https://www.codeproject.com/Articles/103542/Removing-Event-Handlers-using-Reflection
            // After hours and hours of research and trial and error, it turns out that
            // STATIC Events have to be treated differently from INSTANCE Events...
            if (fi.IsStatic)
            {
                MethodInfo mi = t.GetMethod("get_Events", AllBindings);
                EventHandlerList static_event_handlers = (EventHandlerList)mi.Invoke(obj, new object[] { });

                object idx = fi.GetValue(obj);
                Delegate eh = static_event_handlers[idx];
                if (eh == null) return;

                Delegate[] dels = eh.GetInvocationList();
                if (dels == null) return;

                foreach (Delegate del in dels)
                {
                    this._Subscribers.Add(del);
                    ei.RemoveEventHandler(obj, del);
                }
            }
            else
            {
                // INSTANCE EVENT
                if (ei != null)
                {
                    object val = fi.GetValue(obj);
                    Delegate mdel = (val as Delegate);
                    if (mdel != null)
                    {
                        foreach (Delegate del in mdel.GetInvocationList())
                        {
                            this._Subscribers.Add(del);
                            ei.RemoveEventHandler(obj, del);
                        }
                    }
                }
            }

            this._EventInfo = ei;
            this._Object = obj;
        }
    }
}
