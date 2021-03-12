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
    public class NavigatedArgs {
        public object Content { get; set; }
        public NavigationMode NavigationMode { get; set; }
    }
}
