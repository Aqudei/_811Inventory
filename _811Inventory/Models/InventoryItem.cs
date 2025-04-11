using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _811Inventory.Models
{
    class InventoryItem : BindableBase
    {
        private bool _isSelected;

        [LiteDB.BsonIgnore]
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }

        public int Id { get; set; }
        public string? ArticleName { get; set; }
        public string? Description { get; set; }
        public string? OldPropertyNo { get; set; }
        public string? NewPropertyNo { get; set; }
        public string? UnitOfMeasurement { get; set; }
        public double? UnitValue { get; set; }
        public int? QuantityPerPropertyCard { get; set; }
        public int? QuantityPerPhysicalCount { get; set; }
        public string? Location { get; set; }
        public string? Condition { get; set; }
        public string? Remarks { get; set; }
        public List<string> Images { get; set; } = [];
    }
}
