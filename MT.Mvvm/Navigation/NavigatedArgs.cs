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
    public class NavigatedArgs {
        public object Content { get; set; }
        public NavigationMode NavigationMode { get; set; }
    }
}
