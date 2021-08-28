using System;

namespace MT.Mvvm
{
    public sealed class WeakAction<T> : WeakDelegate<Action<T>>
    {
        public WeakAction(Action<T> action) : base(action) { }

        public override void ExecuteWithObject(object parameter)
        {
            if (parameter is null)
                GetDelegate()?.Invoke(default(T));
            else
                GetDelegate()?.Invoke((T)parameter);
        }

        protected override Action<T> GetDelegate() => Callback;
        private void Callback(T obj)
        {
            var a = GetRealDelegate();
            if (a != null) a.Invoke(obj);
        }
    }

}
