using _811Inventory.Models;
using ClosedXML.Excel;
using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Data;
using System.Windows.Forms;
using ZXing;
using ZXing.Windows.Compatibility;

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

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set { SetProperty(ref _searchText, value); }
        }

        private DelegateCommand _PrintQrCommand;
        private InventoryItem[] _selectedForPrinting;
        private int _currentIndex;

        public DelegateCommand PrintQrCommand
        {
            get { return _PrintQrCommand ??= new DelegateCommand(OnPrintQr); }
        }

        private void OnPrintQr()
        {
            _selectedForPrinting = _items.Where(i => i.IsSelected).ToArray();
            _currentIndex = 0;

            var pd = new PrintDocument();
            pd.DefaultPageSettings.PaperSize = new PaperSize("Custom", 827, 1169); // A4 paper size in 1/100 inch units
            pd.DefaultPageSettings.Margins = new Margins(16, 16, 16, 16); // Margins in 1/100 inch (0.5 inches)
            pd.PrinterSettings.PrinterName = "Microsoft Print to PDF"; // Use the Microsoft Print to PDF printer
            pd.PrintPage += PrintPage;
            pd.Print();
            // var preview = new PrintPreviewDialog { Document = pd };
            // preview.ShowDialog(); // Show print preview before actual printing
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            const int dpi = 300; // High-quality DPI
            const int fontSizeEm = 6;
            const int pixelFontHeight = 6 * dpi / 300;

            float inchToPixel = dpi; // 1 inch = DPI pixels
            int qrSize = (int)(0.5 * inchToPixel); // QR Code size (1 inch)
            int marginBetweenQrs = (int)(0.05 * inchToPixel); // Space between QR codes (0.2 inches)

            // Retrieve margin sizes (converted from 1/100 inch to pixels)
            int marginLeft = (int)(e.PageSettings.Margins.Left / 100.0 * dpi);
            int marginTop = (int)(e.PageSettings.Margins.Top / 100.0 * dpi);
            int marginRight = (int)(e.PageSettings.Margins.Right / 100.0 * dpi);
            int marginBottom = (int)(e.PageSettings.Margins.Bottom / 100.0 * dpi);

            // Calculate usable print area
            int usableWidth = e.PageBounds.Width - marginLeft - marginRight;
            int usableHeight = e.PageBounds.Height - marginTop - marginBottom;

            // Calculate how many QR codes fit in the available space
            int cols = usableWidth / (qrSize + marginBetweenQrs);
            int rows = usableHeight / (qrSize + marginBetweenQrs);

            int maxPerPage = cols * rows; // Maximum QR codes per page

            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = qrSize,
                    Height = qrSize,
                    Margin = 0,
                    PureBarcode = true
                }
            };

            Graphics g = e.Graphics;
            g.Clear(Color.White);

            int count = 0; // Track QR codes printed on this page

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (_currentIndex >= _selectedForPrinting.Length)
                        break;

                    var currentItem = _selectedForPrinting[_currentIndex];
                    var qrData = $"{currentItem.Id}::{currentItem.OldPropertyNo}::{currentItem.NewPropertyNo}";

                    var qrBitmap = writer.Write(qrData);

                    // Adjust X, Y positions to account for margins
                    int x = marginLeft + col * (qrSize + marginBetweenQrs);
                    int y = marginTop + row * (qrSize + marginBetweenQrs + pixelFontHeight);

                    g.DrawImage(qrBitmap, x, y, qrSize, qrSize);

                    // Optional: Add text below the QR code
                    g.DrawString($"{currentItem.ArticleName}({currentItem.OldPropertyNo})", new Font("Arial", fontSizeEm), Brushes.Black, x, y + qrSize);

                    _currentIndex++;
                    count++;

                    if (count >= maxPerPage)
                        break; // Stop printing if page is full
                }
                if (count >= maxPerPage)
                    break;
            }

            // If more QR codes need to be printed, continue on a new page
            e.HasMorePages = _currentIndex < _selectedForPrinting.Length;
        }


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
            AppDataPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "_811Inventory");
            Directory.CreateDirectory(AppDataPath);
            DbPath = System.IO.Path.Combine(AppDataPath, "_811Inventory.data");

            Items = CollectionViewSource.GetDefaultView(_items);


            PropertyChanged += InventoryViewModel_PropertyChanged;
        }

        private void InventoryViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SearchText):
                    if (!string.IsNullOrEmpty(SearchText) && SearchText.Length >= 3)
                    {
                        Items.Filter = s =>
                        {
                            var inventoryItem = s as InventoryItem;
                            var vector = $"{inventoryItem.Remarks} {inventoryItem.Description} {inventoryItem.Location} {inventoryItem.ArticleName}".ToLower();
                            var searchTerms = SearchText.ToLower().Split(' ');

                            return searchTerms.All(term => vector.Contains(term));
                        };
                    }
                    else
                    {
                        Items.Filter = null;
                    }
                    break;
                default:
                    break;
            }
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

