using _811Inventory.Models;
using _811Inventory.ViewModels;
using _811Inventory.Views;
using AutoMapper;
using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
using System.Data;
using System.Windows;

namespace _811Inventory;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<Shell>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<Settings>();
        containerRegistry.RegisterForNavigation<Inventory>();
        containerRegistry.RegisterForNavigation<ItemCrud>();
        containerRegistry.RegisterForNavigation<ItemDetailView>();

        containerRegistry.RegisterDialogWindow<MetroDialogWindow>();
        containerRegistry.RegisterDialog<ItemCrud>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<InventoryItem, ItemCrudViewModel>().ReverseMap();
            cfg.CreateMap<InventoryItem, ItemDetailViewModel>().ReverseMap();
        });

        containerRegistry.RegisterInstance(config.CreateMapper());

    }
}

