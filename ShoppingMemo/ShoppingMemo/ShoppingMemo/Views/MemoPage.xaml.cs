
using ShoppingMemo.ViewModels;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace ShoppingMemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MemoPage : Xamarin.Forms.TabbedPage
    {
        private const int TAB_LIMIT = 4;

        public MemoPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // タブ情報を取得する
            var tabs = await App.Database.GetTabsAsync();
            if (tabs.Count < 1)
            {
                foreach (var i in Enumerable.Range(1, TAB_LIMIT).ToArray())
                {
                    var tab = new Models.Tab() { Name = "Tab " + i.ToString(), Order = i };
                    await App.Database.SaveTabAsync(tab);
                    tabs.Add(tab);
                }
            }
            // タブ情報を設定する
            Children.Clear();
            foreach (var tab in tabs.OrderBy(t => t.Order).ToList())
            {
                var tabViewModel = new TabViewModel(tab);
                var tabPage = new TabPage(tabViewModel);
                var navPage = new NavigationPage(tabPage)
                {
                    BindingContext = tabViewModel,
                    IconImageSource = "article_black.png"
                };
                navPage.SetBinding(TitleProperty, "Name");
                Children.Add(navPage);
            }
            UpdateChildrenLayout();
        }
    }
}