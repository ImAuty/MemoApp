using ShoppingMemo.Models;
using System.Collections.ObjectModel;

namespace ShoppingMemo.ViewModels
{
    public class TabViewModel : BaseViewModel
    {
        public ObservableCollection<ItemViewModel> ItemViewModels { get; }

        private Tab _tab;
        public Tab Tab
        {
            get { return _tab; }
            set { SetProperty(ref _tab, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); Tab.Name = _name; }
        }

        public TabViewModel(Tab tab)
        {
            ItemViewModels = new ObservableCollection<ItemViewModel>();
            Tab = tab;
            _name = tab.Name;
        }
    }
}
