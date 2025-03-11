using _811Inventory.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _811Inventory.ViewModels
{
    public class MyMenuItem : HamburgerMenuIconItem
    {
        public static readonly DependencyProperty NavigationDestinationProperty = DependencyProperty.Register(
          nameof(NavigationDestination), typeof(string), typeof(MyMenuItem), new PropertyMetadata(default(string)));

        public string NavigationDestination
        {
            get => (string)this.GetValue(NavigationDestinationProperty);
            set => this.SetValue(NavigationDestinationProperty, value);
        }

        public static readonly DependencyProperty NavigationTypeProperty = DependencyProperty.Register(
          nameof(NavigationType), typeof(Type), typeof(MyMenuItem), new PropertyMetadata(default(Type)));

        public Type NavigationType
        {
            get => (Type)this.GetValue(NavigationTypeProperty);
            set => this.SetValue(NavigationTypeProperty, value);
        }

        public bool IsNavigation => this.NavigationDestination != null;
    }
    public class ShellViewModel
    {
        private DelegateCommand<string> _navigateCommand;
        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand
        {
            get { return _navigateCommand ??= new DelegateCommand<string>(OnNavigate); }
        }

        private void OnNavigate(string navPath)
        {
            _regionManager.RequestNavigate("ContentRegion", navPath);
        }
        public ObservableCollection<MyMenuItem> Menu { get; set; } = [];
        public ShellViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;


            Menu.Add(new MyMenuItem
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.FileZipperRegular },
                Label = "Inventory",
                NavigationType = typeof(Inventory),
                NavigationDestination = "Inventory"

            });

            Menu.Add(new MyMenuItem
            {
                Icon = new PackIconFontAwesome() { Kind = PackIconFontAwesomeKind.ToolboxSolid },
                Label = "Settings",
                NavigationType = typeof(Settings),
                NavigationDestination = "Settings"

            });

        }
    }
}
