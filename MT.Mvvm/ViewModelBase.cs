using MT.Mvvm.Navigation;
using System;

#if NET40_OR_GREATER
using System.Windows.Navigation;
#else 
using Windows.UI.Xaml.Navigation;
#endif

namespace MT.Mvvm {
    public class ViewModelBase : ObservableObject, INavigable {

        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public virtual void Loaded() => throw new NotImplementedException("not override Loaded method");

        public virtual void OnNavigateFrom(NavigatedArgs e) {
        }
#if NET40_OR_GREATER
        public virtual void OnNavigateTo(NavigationEventArgs e) {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        }
#else
        public virtual void OnNavigateTo(NavigationEventArgs e) {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        }
#endif
    }
}
