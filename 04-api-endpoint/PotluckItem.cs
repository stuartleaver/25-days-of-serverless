using Microsoft.WindowsAzure.Storage.Table;

namespace EzrasPotluck
{
    public class PotluckItem : TableEntity
    {
        public string Name { get; set; }
        
        public string FoodItem { get; set; }

        public PotluckItem() { }

        public PotluckItem(string name, string foodItem)
        {
            PartitionKey = name;

            RowKey = string.Concat(name);

            Name = name;

            FoodItem = foodItem;
        }
    }
}