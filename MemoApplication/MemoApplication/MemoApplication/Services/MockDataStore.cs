using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MemoApplication.Models;

namespace MemoApplication.Services
{
    // 表示アイテム管理クラス
    public class MockDataStore : IDataStore<Item>
    {
        /// <summary>
        /// 表示アイテムリスト
        /// </summary>
        readonly List<Item> items;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MockDataStore()
        {
            // TODO: 保存したデータの取得・設定
            items = new List<Item>()
            {
                //new Item { Id = Guid.NewGuid().ToString(), Text = "First item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Second item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Third item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Fourth item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Fifth item", Description="This is an item description." },
                //new Item { Id = Guid.NewGuid().ToString(), Text = "Sixth item", Description="This is an item description." }
            };
        }

        /// <summary>
        /// 表示アイテムを追加する
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True:成功、False:失敗</returns>
        public async Task<bool> AddItemAsync(Item item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 表示アイテムを更新する
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True:成功、False:失敗</returns>
        public async Task<bool> UpdateItemAsync(Item item)
        {
            var oldItem = items.Where((Item arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 表示アイテムを削除する
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((Item arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        /// <summary>
        /// 表示アイテムを取得する
        /// </summary>
        /// <param name="id"></param>
        /// <returns>表示アイテム（存在しない場合はnullを返す）</returns>
        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        /// <summary>
        /// 表示アイテムを取得する
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns>表示アイテムリスト</returns>
        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}