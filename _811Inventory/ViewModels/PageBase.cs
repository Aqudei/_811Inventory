using Prism.Mvvm;
using Prism.Regions;
using System.IO;

namespace _811Inventory.ViewModels
{
    public class PageBase : BindableBase, INavigationAware
    {
        public string AppDataPath { get; }
        public string DbPath { get; }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public PageBase()
        {
            AppDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "_811Inventory");
            Directory.CreateDirectory(AppDataPath);
            DbPath = System.IO.Path.Combine(AppDataPath, "_811Inventory.data");
        }
    }
}