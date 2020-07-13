using SQLite;

namespace ShoppingMemo.Models
{
    public class Tab
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}
