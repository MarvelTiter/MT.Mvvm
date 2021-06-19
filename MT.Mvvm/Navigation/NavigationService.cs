using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET40_OR_GREATER
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

#endif

namespace MT.Mvvm.Navigation {
    public class NavigationService : INavigationService {
        private const string Root = "__Root__";
        private const string Unknow = "__Unknow__";

        public event NavigationFailedEventHandler NavigationFailed;
        public event NavigatedEventHandler Navigated;

        public NavigationService() {

        }

        public NavigationService(Enum key) {
            this.customFrameName = key.ToString();
        }

        private void FindFrameFromVisualTreeByName(DependencyObject parent) {
            DependencyObject root;
            if (parent is Frame f) {
                if (f.Name == customFrameName) {
                    _currentFrame = f;
                    return;
                }
                root = f.Content as FrameworkElement;
            } else
                root = parent;
            var count = VisualTreeHelper.GetChildrenCount(root);
            if (count == 0) {
                return;
            }
            for (var i = 0; i < count; i++) {
                var fe = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
                if (fe is Frame frame) {
                    if (frame.Name == customFrameName) {
                        _currentFrame = frame;
                        return;
                    }
                }
                FindFrameFromVisualTreeByName(fe);
            }
        }

        public string CurrentPageKey {
            get {
                lock (_pagesByKey) {
                    if (!CurrentFrame.BackStack.GetEnumerator().MoveNext()) {
                        return Root;
                    }

                    if (CurrentFrame.Content == null) {
                        return Unknow;
                    }

                    Type currentType = CurrentFrame.Content.GetType();
                    if (!_pagesByKey.ContainsValue(currentType)) {
                        return Unknow;
                    }

                    return _pagesByKey.FirstOrDefault((KeyValuePair<string, Type> i) => (object)i.Value == currentType).Key;
                }
            }
        }
        private readonly Dictionary<string, Type> _pagesByKey = new Dictionary<string, Type>();
        private readonly string customFrameName;
        private Frame _currentFrame;
        public Frame CurrentFrame {
            get {
                if (_currentFrame == null) {
                    FindFrameFromVisualTreeByName(null);
                    RegisterFrameEvents();
                }
                return _currentFrame;
            }
            set {
                UnregisterFrameEvents();
                _currentFrame = value;
                RegisterFrameEvents();
            }
        }

        public void GoBack() {
            if (CurrentFrame.CanGoBack) {
                var currentPage = CurrentFrame.Content as Page;
                if (currentPage?.DataContext is INavigable nav) {
                    nav.OnNavigateFrom(new NavigatedArgs {
                        Content = CurrentFrame.Content,
                        NavigationMode = NavigationMode.Back
                    });
                }
                CurrentFrame.GoBack();
            }
        }

        public void GoForward() {
            if (CurrentFrame.CanGoForward) {
                var currentPage = CurrentFrame.Content as Page;
                if (currentPage?.DataContext is INavigable nav) {
                    nav.OnNavigateFrom(new NavigatedArgs {
                        Content = CurrentFrame.Content,
                        NavigationMode = NavigationMode.Forward
                    });
                }
                CurrentFrame.GoForward();
            }
        }


        public void Configura<T>() {
            var type = typeof(T);
            var key = type.Name;
            Configura(key, type);
        }

        public void Configura(string key, Type pageType) {
            lock (_pagesByKey) {
                if (_pagesByKey.ContainsKey(key)) {
                    throw new ArgumentException("This key is already used: " + key);
                }

                if (_pagesByKey.ContainsValue(pageType)) {
                    throw new ArgumentException("This type is already configured with key " + _pagesByKey.First((KeyValuePair<string, Type> p) => (object)p.Value == pageType).Key);
                }
                _pagesByKey.Add(key, pageType);
            }
        }

        private void RegisterFrameEvents() {
            if (CurrentFrame != null) {
                CurrentFrame.Navigated += Frame_Navigated;
                CurrentFrame.Navigating += Frame_Navigating;
                CurrentFrame.NavigationFailed += Frame_NavigationFailed;
            }
        }

        private void UnregisterFrameEvents() {
            if (CurrentFrame != null) {
                CurrentFrame.Navigated -= Frame_Navigated;
                CurrentFrame.Navigating -= Frame_Navigating;
                CurrentFrame.NavigationFailed += Frame_NavigationFailed;
            }
        }


        private void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) {
            NavigationFailed?.Invoke(sender, e);
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e) {
            var currentPage = CurrentFrame.Content as Page;
            if (currentPage?.DataContext is INavigable nav)
                nav.OnNavigatingFrom(e);
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e) {
            var destPage = e.Content as Page;
            if (destPage?.DataContext is INavigable nav)
                nav.OnNavigateTo(e);
            Navigated?.Invoke(sender, e);
        }
#if NET40_OR_GREATER
        public bool NavigateTo(string pageKey, object parameter) {
            lock (_pagesByKey) {
                if (!_pagesByKey.ContainsKey(pageKey)) {
                    throw new ArgumentException($"No such page: {pageKey}.");
                }
                var currentPage = CurrentFrame.Content as Page;
                if (currentPage?.DataContext is INavigable nav) {
                    nav.OnNavigateFrom(new NavigatedArgs {
                        Content = CurrentFrame.Content,
                        NavigationMode = NavigationMode.New
                    });
                }
                var b = CurrentFrame.Navigate(_pagesByKey[pageKey], parameter);
                return b;
            }
        }
#else
        public bool NavigateTo(string pageKey, NavigationTransitionInfo transInfo = null) {
            throw new NotImplementedException();
        }

        public bool NavigateTo(string pageKey, object parameter, NavigationTransitionInfo transInfo = null) {
            throw new NotImplementedException();
        }
#endif
    }
}
