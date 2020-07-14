using ShoppingMemo.Models;

namespace ShoppingMemo.ViewModels
{
    public class ItemViewModel : BaseViewModel
    {
        public Item Item { get; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private int _number;
        public int Number
        {
            get { return _number; }
            set { SetProperty(ref _number, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private int _order;
        public int Order
        {
            get { return _order; }
            set { SetProperty(ref _order, value); Item.Order = value; }
        }

        private bool _showDescription = false;
        public bool ShowDescription
        {
            get { return _showDescription && _isDisplayMode; }
            set { SetProperty(ref _showDescription, value); }
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); IsDisplayMode = !value; }
        }

        private bool _isDisplayMode = true;
        public bool IsDisplayMode
        {
            get { return _isDisplayMode; }
            private set { SetProperty(ref _isDisplayMode, value); }
        }

        public ItemViewModel(Item item)
        {
            Item = item;
            ResetProperty();
            Order = Item.Order;
        }

        public void ResetProperty()
        {
            Name = Item.Name;
            Number = Item.Number;
            Description = Item.Description;
        }

        public void SetProperty()
        {
            Item.Name = Name;
            Item.Number = Number;
            Item.Description = Description;
        }
    }
}
