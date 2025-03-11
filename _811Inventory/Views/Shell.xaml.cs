using _811Inventory.ViewModels;
using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _811Inventory.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : MetroWindow
    {

        public Shell(IRegionManager regionManager)
        {
            InitializeComponent();

            RegionManager.SetRegionManager(ContentRegion, regionManager);
            RegionManager.SetRegionName(ContentRegion, "ContentRegion");
            
            Loaded += Shell_Loaded;
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (ShellViewModel)DataContext;

            HamburgerMenuControl.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is MyMenuItem menuItem)
                {
                    viewModel.NavigateCommand.Execute(menuItem.NavigationDestination);
                }
            };

            var firstItem = HamburgerMenuControl.Items.OfType<MyMenuItem>().First();
            HamburgerMenuControl.SelectedItem = firstItem;
            // _viewModel.NavigateCommand.Execute(firstItem.NavigationDestination);

        }
    }
}
