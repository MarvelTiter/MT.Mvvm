using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET40_OR_GREATER
using System.Windows.Controls;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

namespace MT.Mvvm.Navigation {
    public interface INavigationService {

#if NET40_OR_GREATER
        Frame CurrentFrame { get; set; }

        bool NavigateTo(string pageKey, object parameter);
#else
        Frame CurrentFrame { get; set; }

        bool NavigateTo(string pageKey, NavigationTransitionInfo transInfo = null);

        bool NavigateTo(string pageKey, object parameter, NavigationTransitionInfo transInfo = null);
#endif



        string CurrentPageKey { get; }

        void GoBack();

        void GoForward();
    }

}
