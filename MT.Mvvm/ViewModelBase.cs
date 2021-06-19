using MT.Mvvm.Navigation;

#if NET40_OR_GREATER
using System.Windows.Navigation;
#else 
using Windows.UI.Xaml.Navigation;
#endif

namespace MT.Mvvm {
    public class ViewModelBase : ObservableObject, INavigable {

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
