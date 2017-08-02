using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Humb.Core.Events
{
    public class Subscriber : WeakReference
    {
        public MethodInfo _methodInfo { get; set; }
        public Delegate _methodHandler { get; set; }
        public Subscriber(Delegate handler) : base(handler.Target)
        {
            _methodHandler = handler;
            _methodInfo = handler.Method;
        }
        public override bool IsAlive
        {
            get
            {
                return _methodInfo.IsStatic || base.IsAlive;
            }
        }
        public Action<T> CreateAction<T>()
        {
            if (!IsAlive)
                return null;

            try
            {
                if (_methodInfo.IsStatic)
                    return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), _methodInfo);
                else
                    return (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), base.Target, _methodInfo.Name);
            }
            catch
            {
                return null;
            }
        }
    }
}
