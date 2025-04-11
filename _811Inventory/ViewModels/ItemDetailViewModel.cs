using _811Inventory.Models;
using AutoMapper;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _811Inventory.ViewModels
{
    class ItemDetailViewModel : BindableBase, INavigationAware
    {
        private IMapper _mapper;

        private int _id;
        private string? _articleName;
        private string? _description;
        private string? _oldPropertyNo;
        private string? _newPropertyNo;
        private string? _unitOfMeasurement;
        private double? _unitValue;
        private int? _quantityPerPropertyCard;
        private int? _quantityPerPhysicalCount;
        private string? _location;
        private string? _condition;
        private string? _remarks;

        public int Id { get => _id; set => SetProperty(ref _id, value); }
        public string? ArticleName { get => _articleName; set => SetProperty(ref _articleName, value); }
        public string? Description { get => _description; set => SetProperty(ref _description, value); }
        public string? OldPropertyNo { get => _oldPropertyNo; set => SetProperty(ref _oldPropertyNo, value); }
        public string? NewPropertyNo { get => _newPropertyNo; set => SetProperty(ref _newPropertyNo, value); }
        public string? UnitOfMeasurement { get => _unitOfMeasurement; set => SetProperty(ref _unitOfMeasurement, value); }
        public double? UnitValue { get => _unitValue; set => SetProperty(ref _unitValue, value); }
        public int? QuantityPerPropertyCard { get => _quantityPerPropertyCard; set => SetProperty(ref _quantityPerPropertyCard, value); }
        public int? QuantityPerPhysicalCount { get => _quantityPerPhysicalCount; set => SetProperty(ref _quantityPerPhysicalCount, value); }
        public string? Location { get => _location; set => SetProperty(ref _location, value); }
        public string? Condition { get => _condition; set => SetProperty(ref _condition, value); }
        public string? Remarks { get => _remarks; set => SetProperty(ref _remarks, value); }
        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.TryGetValue<InventoryItem>("InventoryItem", out var inventoryItem))
            {
                _mapper.Map(inventoryItem, this);
            }
        }

        public ItemDetailViewModel(IMapper mapper)
        {
            _mapper = mapper;
        }
    }
}
