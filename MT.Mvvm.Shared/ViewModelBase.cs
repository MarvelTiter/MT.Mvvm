using MT.Mvvm.Navigation;
#if NET_Uap
using Windows.UI.Xaml.Navigation;
#endif

#if NET_Fx
using System.Windows.Navigation;
#endif

namespace MT.Mvvm {
    public class ViewModelBase : ObservableObject, INavigable {

        public virtual void OnNavigateFrom(NavigatedArgs e) {
        }
#if NET_Uap
        public virtual void OnNavigateTo(NavigationEventArgs e) {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        }
#endif
#if NET_Fx
        public virtual void OnNavigateTo(NavigationEventArgs e) {
        }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e) {
        }
#endif
    }
}
