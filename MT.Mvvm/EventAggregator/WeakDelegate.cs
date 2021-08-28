using System;

namespace MT.Mvvm
{
    public abstract class WeakDelegate
    {
        public abstract bool CheckAlive();
    }
    public abstract class WeakDelegate<TDelegate> : WeakDelegate, IExecutable where TDelegate : Delegate /* MulticastDelegate */
    {
        private struct WeakReference<T> : IDisposable where T : class
        {
            private WeakReference m_weakReference;
            public WeakReference(T target) { m_weakReference = new WeakReference(target); }
            public T Target { get { return (T)m_weakReference.Target; } }
            public void Dispose() { m_weakReference = null; }
        }

        private WeakReference<TDelegate> m_weakDelegate;
        private Action<TDelegate> m_removeDelegateCode;

        public WeakDelegate(TDelegate @delegate)
        {
            var md = (MulticastDelegate)(object)@delegate;
            if (md.Target == null)
                throw new ArgumentException("There is no reason to make a WeakDelegate to a static method.");
            // Save a WeakReference to the delegate
            m_weakDelegate = new WeakReference<TDelegate>(@delegate);
        }

        public Action<TDelegate> RemoveDelegateCode
        {
            set
            {
                m_removeDelegateCode = value;
            }
        }
        public override bool CheckAlive()
        {
            if (m_weakDelegate.Target == null)
            {
                m_weakDelegate.Dispose();
                return false;
            }
            else
            {
                return true;
            }
        }
        protected TDelegate GetRealDelegate()
        {
            // If the real delegate hasn't been GC'd yet, just return it
            TDelegate realDelegate = m_weakDelegate.Target;
            if (realDelegate != null) return realDelegate;

            // The real delegate was GC'd, we don't need our WeakReference to it anymore (it can be GC'd)
            m_weakDelegate.Dispose();

            // Remove the delegate from the chain (if the user told us how)
            if (m_removeDelegateCode != null)
            {
                m_removeDelegateCode(GetDelegate());
                m_removeDelegateCode = null;  // Let the remove handler delegate be GC'd
            }
            return null;   // The real delegate was GC'd and can't be called
        }

        // All derived classes must return a delegate to a private method matching the TDelegate type
        protected abstract TDelegate GetDelegate();
        public abstract void ExecuteWithObject(object parameter);
        public virtual void Execute()
        {
            ExecuteWithObject(default);
        }

        // Implicit conversion operator to convert a WeakDelegate object to an actual delegate
        public static implicit operator TDelegate(WeakDelegate<TDelegate> @delegate)
        {
            return @delegate.GetDelegate();
        }
    }

}
