using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET_Uap
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
#endif

#if NET_Fx
using System.Windows.Controls;
#endif

namespace MT.Mvvm.Navigation {
    public interface INavigationService {


#if NET_Uap
        Frame CurrentFrame { get; set; }

        bool NavigateTo(string pageKey, NavigationTransitionInfo transInfo = null);

        bool NavigateTo(string pageKey, object parameter, NavigationTransitionInfo transInfo = null);
#endif

#if NET_Fx
        Frame CurrentFrame { get; set; }

        bool NavigateTo(string pageKey, object parameter);
#endif

        string CurrentPageKey { get; }

        void GoBack();

        void GoForward();
    }

}
