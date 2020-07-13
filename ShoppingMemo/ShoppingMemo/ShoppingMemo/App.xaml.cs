using ShoppingMemo.Models;
using ShoppingMemo.Views;
using System;
using System.IO;
using Xamarin.Forms;

namespace ShoppingMemo
{
    public partial class App : Application
    {
        private const string DB_FILE_NAME = "shopping_memo.db3";

        private static Database _database;
        public static Database Database
        {
            get
            {
                if (_database == null)
                {
                    var localFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _database = new Database(Path.Combine(localFolderPath, DB_FILE_NAME));
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
