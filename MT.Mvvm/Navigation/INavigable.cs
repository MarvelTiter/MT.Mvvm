using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET40_OR_GREATER
using System.Windows.Navigation;
#else
using Windows.UI.Xaml.Navigation;
#endif

namespace MT.Mvvm.Navigation {
    public interface INavigable {
        void OnNavigateFrom(NavigatedArgs e);
#if NET40_OR_GREATER
        void OnNavigateTo(NavigationEventArgs e);

        void OnNavigatingFrom(NavigatingCancelEventArgs e);
#else
        void OnNavigateTo(NavigationEventArgs e);

        void OnNavigatingFrom(NavigatingCancelEventArgs e);
#endif



    }
}
