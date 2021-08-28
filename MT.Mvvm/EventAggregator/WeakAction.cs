using System;

namespace MT.Mvvm
{
    public class WeakAction : WeakDelegate<Action>
    {
        public WeakAction(Action action) : base(action) { }

        public override void ExecuteWithObject(object parameter)
        {
            GetDelegate()?.Invoke();
        }

        protected override Action GetDelegate() => Callback;
        private void Callback()
        {
            var a = base.GetRealDelegate();
            if (a != null) a.Invoke();
        }
    }

}
