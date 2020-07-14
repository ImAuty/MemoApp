using SQLite;

namespace ShoppingMemo.Models
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TabId { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public override string ToString()
        {
            return Name
                + " " + Number.ToString()
                + (string.IsNullOrWhiteSpace(Description) ? string.Empty : string.Format(" ({0})", Description));
        }
    }
}
