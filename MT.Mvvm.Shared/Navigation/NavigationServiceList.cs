using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Mvvm.Navigation {
    public class NavigationList {

        private readonly Dictionary<string, INavigationService> _services = new Dictionary<string, INavigationService>();
        private static NavigationList _instance;
        public static NavigationList Instance => _instance ?? (_instance = new NavigationList());

        public INavigationService this[Enum key] => _services[key.ToString()];

        public void Register(Enum key, INavigationService nav) {
            if (IsRegistered(key))
                throw new ArgumentException("This key is already used:" + key);
            _services.Add(key.ToString(), nav);
        }
        public void Unregister(Enum key) {
            if (IsRegistered(key))
                _services.Remove(key.ToString());
        }

        public bool IsRegistered(Enum key) {
            return _services.ContainsKey(key.ToString());
        }
    }
}
