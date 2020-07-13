using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingMemo.Models
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Tab>().Wait();
            _database.CreateTableAsync<Item>().Wait();
        }

        public Task<List<Tab>> GetTabsAsync()
        {
            return _database.Table<Tab>().ToListAsync();
        }

        public Task<List<Item>> GetItemsAsync(int tabId)
        {
            return _database.Table<Item>().Where(i => i.TabId == tabId).ToListAsync();
        }

        public Task<Tab> GetTabAsync(int id)
        {
            return _database.Table<Tab>().FirstAsync(tab => tab.Id == id);
        }

        public Task<Item> GetItemAsync(int id)
        {
            return _database.Table<Item>().FirstAsync(item => item.Id == id);
        }

        public Task<int> SaveTabAsync(Tab tab)
        {
            return _database.InsertAsync(tab);
        }

        public Task<int> SaveItemAsync(Item item)
        {
            return _database.InsertAsync(item);
        }

        public Task<int> UpdateTabAsync(Tab tab)
        {
            return _database.UpdateAsync(tab);
        }

        public Task<int> UpdateItemAsync(Item item)
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> RemoveTabAsync(Tab tab)
        {
            return _database.DeleteAsync(tab);
        }

        public Task<int> RemoveItemAsync(Item item)
        {
            return _database.DeleteAsync(item);
        }

        //public Task<int> ClearTabAsync()
        //{
        //    return _database.DeleteAllAsync<Tab>();
        //}

        //public Task<int> ClearItemAsync()
        //{
        //    return _database.DeleteAllAsync<Item>();
        //}

    }
}
