using _811Inventory.Models;
using ClosedXML.Excel;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace _811Inventory.ViewModels
{
    class InventoryViewModel : PageBase
    {
        private DelegateCommand _importCommand;
        private string AppDataPath;
        private string DbPath;
        private bool _isAllSelected;

        public bool IsAllSelected
        {
            get { return _isAllSelected; }
            set
            {
                SetProperty(ref _isAllSelected, value);

                foreach (var item in _items)
                {
                    item.IsSelected = value;
                }
            }
        }

        private ObservableCollection<InventoryItem> _items = [];
        public ICollectionView Items { get; set; }

        public DelegateCommand ImportCommand
        {
            get { return _importCommand ??= new DelegateCommand(OnImport); }
        }
        public static IEnumerable<InventoryItem> ReadExcel(string filePath)
        {
            var records = new List<Dictionary<string, object>>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var worksheet = workbook.Worksheet(2);
                var rows = worksheet.RangeUsed().RowsUsed();
                var headers = new List<string>()
                {
                    "ArticleName",
                    "Description",
                    "OldPropertyNo",
                    "NewPropertyNo",
                    "UnitOfMeasurement",
                    "UnitValue",
                    "QuantityPerPropertyCard",
                    "QuantityPerPhysicalCount",
                    "Location",
                    "Condition",
                    "Remarks"
                };

                // Read data rows
                foreach (var row in rows.Skip(1))
                {
                    var dict = new Dictionary<string, object>();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        dict[headers[i]] = row.Cell(i + 1).GetValue<string>();
                    }
                    records.Add(dict);
                }
            }

            foreach (var rec in records)
            {
                var itemObj = new InventoryItem();

                itemObj.Location = rec["Location"]?.ToString();
                itemObj.ArticleName = rec["ArticleName"]?.ToString();
                itemObj.Condition = rec["Condition"]?.ToString();
                itemObj.UnitOfMeasurement = rec["UnitOfMeasurement"]?.ToString();
                itemObj.Description = rec["Description"]?.ToString();
                itemObj.NewPropertyNo = rec["NewPropertyNo"]?.ToString();
                itemObj.OldPropertyNo = rec["OldPropertyNo"]?.ToString();
                itemObj.Remarks = rec["Remarks"]?.ToString();
                if (int.TryParse(rec["QuantityPerPhysicalCount"]?.ToString(), out var quantityPerPhysicalCount))
                {
                    itemObj.QuantityPerPhysicalCount = quantityPerPhysicalCount;
                }

                if (int.TryParse(rec["QuantityPerPropertyCard"]?.ToString(), out var quantityPerPropertyCard))
                {
                    itemObj.QuantityPerPropertyCard = quantityPerPropertyCard;
                }

                if (double.TryParse(rec["UnitValue"]?.ToString(), out var unitValue))
                {
                    itemObj.UnitValue = unitValue;
                }

                yield return itemObj;
            }
        }

        public InventoryViewModel()
        {
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "_811Inventory");
            Directory.CreateDirectory(AppDataPath);
            DbPath = Path.Combine(AppDataPath, "_811Inventory.data");

            Items = CollectionViewSource.GetDefaultView(_items);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _items.Clear();

            using var db = new LiteDB.LiteDatabase(DbPath);
            _items.AddRange(db.GetCollection<InventoryItem>().FindAll());
        }

        private void OnImport()
        {
            var dialog = new Microsoft.WindowsAPICodePack.Dialogs.CommonOpenFileDialog
            {

            };

            dialog.Filters.Add(new Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogFilter("Excel File", "*.xlsx"));
            if (dialog.ShowDialog() != Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                return;

            var records = ReadExcel(dialog.FileName);
            if (!records.Any())
                return;


            using var db = new LiteDB.LiteDatabase(DbPath);
            db.DropCollection("InventoryItem");

            foreach (var item in records)
            {
                db.GetCollection<InventoryItem>().Insert(item);
            }
        }
    }
}

