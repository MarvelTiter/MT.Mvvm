using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET_Uap
using Windows.UI.Xaml.Navigation;
#endif

#if NET_Fx
using System.Windows.Navigation;
#endif

namespace MT.Mvvm.Navigation {
    public interface INavigable {
        void OnNavigateFrom(NavigatedArgs e);

#if NET_Uap
        void OnNavigateTo(NavigationEventArgs e);

        void OnNavigatingFrom(NavigatingCancelEventArgs e);
#endif

#if NET_Fx
        void OnNavigateTo(NavigationEventArgs e);

        void OnNavigatingFrom(NavigatingCancelEventArgs e);
#endif

    }
}
