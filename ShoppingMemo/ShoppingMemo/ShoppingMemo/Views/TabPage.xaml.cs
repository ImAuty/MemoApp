using ShoppingMemo.Models;
using ShoppingMemo.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ShoppingMemo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabPage : ContentPage
    {
        private const int ITEM_LIMIT = 100;
        private readonly TabViewModel _tabViewModel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="tabId">タブID</param>
        /// <param name="tabName">タブ名</param>
        public TabPage(TabViewModel tabViewModel)
        {
            InitializeComponent();
            _tabViewModel = tabViewModel;
            BindingContext = _tabViewModel;
        }

        /// <summary>
        /// ページがロードされた時のイベント
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RenewItems();
        }

        /// <summary>
        /// ビューモデルのアイテムリストを更新する
        /// </summary>
        /// <returns></returns>
        private async Task RenewItems()
        {
            var items = await App.Database.GetItemsAsync(_tabViewModel.Tab.Id);
            _tabViewModel.ItemViewModels.Clear();
            foreach (var item in items.OrderBy(i => i.Order).ThenBy(i => i.Id).ToList())
            {
                _tabViewModel.ItemViewModels.Add(new ItemViewModel(item));
            }
        }

        #region Toolbar

        /// <summary>
        /// ツールバーの更新ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RenewToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            // アイテムリストを更新する
            await RenewItems();
        }

        /// <summary>
        /// ツールバーの共有ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ShareToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            // アイテムの情報を取得する
            var items = await App.Database.GetItemsAsync(_tabViewModel.Tab.Id);
            var itemTexts = items.OrderBy(i => i.Order).ThenBy(i => i.Id)
                .Select(i => i.ToString())
                .ToArray();
            var texts = new string[3];
            texts[0] = "買い物メモ";
            texts[1] = _tabViewModel.Tab.Name;
            texts[2] = string.Join("\r\n", itemTexts);
            // 共有する
            await Share.RequestAsync(string.Join("\r\n", texts));
        }

        /// <summary>
        /// ツールバーの編集ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EditToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            // プロンプトを表示する
            var result = await DisplayPromptAsync(_tabViewModel.Tab.Name, "タブの名前を変更します", "OK", "Cancel", "タブの名前", -1, Keyboard.Text, _tabViewModel.Tab.Name);
            if (result == null) return;
            // タブの名前を変更する
            _tabViewModel.Name = result;
            await App.Database.UpdateTabAsync(_tabViewModel.Tab);
        }

        /// <summary>
        /// ツールバーの削除ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteToolbar_Clicked(object sender, System.EventArgs e)
        {
            // アラートを表示する
            var result = await DisplayAlert(_tabViewModel.Tab.Name, "タブを削除しますか？", "Yes", "No");
            if (!result) return;
            await App.Database.RemoveTabAsync(_tabViewModel.Tab);
            _tabViewModel.ItemViewModels.Clear();
            // TODO: 画面側のタブを削除する
        }

        #endregion

        #region ItemDisplayer

        /// <summary>
        /// アイテムがタップされた時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // 対象のアイテムを取得する
            var itemViewModel = e.Item as ItemViewModel;
            // 説明ラベルの表示・非表示を切り替える
            itemViewModel.ShowDescription = !itemViewModel.ShowDescription;
        }

        /// <summary>
        /// アイテムの編集ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditButton_Clicked(object sender, System.EventArgs e)
        {
            // 対象のアイテムを取得する
            var layout = (BindableObject)sender;
            var itemViewModel = (ItemViewModel)layout.BindingContext;
            // 表示レイアウト・編集レイアウトを切り替える
            itemViewModel.IsEditMode = !itemViewModel.IsEditMode;
        }

        /// <summary>
        /// アイテムの順序ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OrderButton_Clicked(object sender, System.EventArgs e)
        {
            var up = "一つ上に移動する";
            var down = "一つ下に移動する";

            // 対象のアイテムを取得する
            var layout = (BindableObject)sender;
            var srcItem = (ItemViewModel)layout.BindingContext;

            // アイテムの位置を確認する
            var index = _tabViewModel.ItemViewModels.IndexOf(srcItem);
            if (index >= _tabViewModel.ItemViewModels.Count - 1) down = null;
            if (index <= 0) up = null;

            var action = await DisplayActionSheet("アイテムを移動する", "Cancel", null, up, down);

            if (action == down)
            {
                if (index >= _tabViewModel.ItemViewModels.Count - 1) return;
                // アイテムの順序を一つ後のアイテムと入れ替える
                var dstItem = _tabViewModel.ItemViewModels.ElementAt(index + 1);
                var srcOrder = srcItem.Order;
                srcItem.Order = dstItem.Order;
                dstItem.Order = srcOrder;
                // DBを更新する
                await App.Database.UpdateItemAsync(srcItem.Item);
                await App.Database.UpdateItemAsync(dstItem.Item);
                // 画面を更新する
                _tabViewModel.ItemViewModels.Insert(index + 2, srcItem);
                _tabViewModel.ItemViewModels.RemoveAt(index);
            }

            if (action == up)
            {
                if (index <= 0) return;
                // アイテムの順序を一つ前のアイテムと入れ替える
                var dstItem = _tabViewModel.ItemViewModels.ElementAt(index - 1);
                var srcOrder = srcItem.Order;
                srcItem.Order = dstItem.Order;
                dstItem.Order = srcOrder;
                // DBを更新する
                await App.Database.UpdateItemAsync(srcItem.Item);
                await App.Database.UpdateItemAsync(dstItem.Item);
                // 画面を更新する
                _tabViewModel.ItemViewModels.Insert(index - 1, srcItem);
                _tabViewModel.ItemViewModels.RemoveAt(index + 1);
            }
        }

        #endregion

        #region ItemEditor

        /// <summary>
        /// アイテムの削除ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RemoveButton_Clicked(object sender, System.EventArgs e)
        {
            // 対象のアイテムを取得する
            var layout = (BindableObject)sender;
            var itemViewModel = (ItemViewModel)layout.BindingContext;
            // アラートを表示する
            var result = await DisplayAlert(itemViewModel.Name, "アイテムを削除しますか？", "Yes", "No");
            if (!result) return;
            // アイテムを削除する
            await App.Database.RemoveItemAsync(itemViewModel.Item);
            _tabViewModel.ItemViewModels.Remove(itemViewModel);
        }

        /// <summary>
        /// アイテムの確定ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DoneButton_Clicked(object sender, System.EventArgs e)
        {
            // 対象のアイテムを取得する
            var layout = (BindableObject)sender;
            var itemViewModel = (ItemViewModel)layout.BindingContext;
            // アイテムの更新を確定する
            itemViewModel.SetProperty();
            await App.Database.UpdateItemAsync(itemViewModel.Item);
            // 表示レイアウト・編集レイアウトを切り替える
            itemViewModel.IsEditMode = !itemViewModel.IsEditMode;
            // 説明ラベルの表示を確認する
            if (!string.IsNullOrEmpty(itemViewModel.Item.Description))
            {
                itemViewModel.ShowDescription = true;
            }
        }

        /// <summary>
        /// アイテムの取消ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Clicked(object sender, System.EventArgs e)
        {
            // 対象のアイテムを取得する
            var layout = (BindableObject)sender;
            var itemViewModel = (ItemViewModel)layout.BindingContext;
            // アイテムの更新を取消する
            itemViewModel.ResetProperty();
            // 表示レイアウト・編集レイアウトを切り替える
            itemViewModel.IsEditMode = !itemViewModel.IsEditMode;
        }

        #endregion

        /// <summary>
        /// 追加ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddButton_Clicked(object sender, System.EventArgs e)
        {
            // アイテム数の上限を超えていないか確認する
            var itemCount = _tabViewModel.ItemViewModels.Count;
            if (itemCount >= ITEM_LIMIT)
            {
                await DisplayAlert("", ITEM_LIMIT.ToString("登録は{0}個まで可能です"), "OK");
                return;
            }
            // 新しいアイテムを作成する
            var order = 0;
            if (itemCount > 0) order = _tabViewModel.ItemViewModels.Max(i => i.Order);
            var item = new Item()
            {
                TabId = _tabViewModel.Tab.Id,
                Name = "Item " + (order + 1).ToString(),
                Number = 0,
                Description = string.Empty,
                Order = order + 1
            };
            // アイテムを追加する
            _tabViewModel.ItemViewModels.Add(new ItemViewModel(item));
            await App.Database.SaveItemAsync(item);
        }

        /// <summary>
        /// 全削除ボタンが押された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DeleteButton_Clicked(object sender, System.EventArgs e)
        {
            // アラートを表示する
            var result = await DisplayAlert(_tabViewModel.Tab.Name, "アイテムをすべて削除しますか？", "Yes", "No");
            if (!result) return;
            // タブのアイテムをすべて削除する
            foreach (var itemViewModel in _tabViewModel.ItemViewModels)
            {
                await App.Database.RemoveItemAsync(itemViewModel.Item);
            }
            _tabViewModel.ItemViewModels.Clear();
        }
    }
}