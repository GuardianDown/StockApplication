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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        AddProductWindow addProductWindow;

        public MainWindow()
        {
            InitializeComponent();

            // Создаем контекстное меню для таблицы "На складе" и таблицы "Принят"
            DataGridStock.ContextMenu = CreateDataGridStockContextMenu();
            DataGridAccepted.ContextMenu = CreateDataGridAcceptedContextMenu();

            // Отображаем данные из базы данных в таблице перед запуском приложения
            UpdateDataGrid("DataGridAccepted");
            UpdateDataGrid("DataGridStock");
            UpdateDataGrid("DataGridSold");
            UpdateDataGrid("DataGridAll");

            // Добавляем события выбора даты
            DatePickerFrom.SelectedDateChanged += DatePickerFrom_SelectedDateChange;
            DatePickerTo.SelectedDateChanged += DatePickerTo_SelectedDateChange;
        }

        #region Методы
        // Метод создания контекстного меню для таблицы "На складе"
        ContextMenu CreateDataGridStockContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Продать";
            contextMenu.Items.Add(menuItem);
            menuItem.Click += DataGridStockContextMenu_Click;
            return contextMenu;
        }

        // Метод создания контекстного меню для таблицы "Принят"
        ContextMenu CreateDataGridAcceptedContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "На склад";
            contextMenu.Items.Add(menuItem);
            menuItem.Click += DataGridAcceptedContextMenu_Click;
            return contextMenu;
        }

        // Метод обновления таблиц
        public void UpdateDataGrid(string dataGridName)
        {
            // Если не выбрана дата, то задаем минимальное/максимальное значение для фильтра
            DateTime from = DatePickerFrom.SelectedDate ?? new DateTime(0001, 01, 01);
            DateTime to = DatePickerTo.SelectedDate ?? new DateTime(9999, 12, 30);

            // Обновление таблицы "Принят"
            if (dataGridName == "DataGridAccepted")
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<ProductAccepted> productAcceptedList = new List<ProductAccepted>();
                    foreach (var element in db.ProductsAccepted.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productAcceptedList.Add(element);
                    }
                    DataGridAccepted.ItemsSource = productAcceptedList;
                }
            }
            // Обновление таблицы "На складе"
            else if (dataGridName == "DataGridStock")
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<ProductStock> productStockList = new List<ProductStock>();
                    foreach (var element in db.ProductsStock.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productStockList.Add(element);
                    }
                    DataGridStock.ItemsSource = productStockList;
                }
            }
            // Обновление таблицы "Продано"
            else if (dataGridName == "DataGridSold")
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<ProductSold> productSoldList = new List<ProductSold>();
                    foreach (var element in db.ProductsSold.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productSoldList.Add(element);
                    }
                    DataGridSold.ItemsSource = productSoldList;
                }
            }
            // Обновление таблицы "Все"
            else if (dataGridName == "DataGridAll")
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<Product> productAllList = new List<Product>();
                    foreach (var element in db.ProductsAccepted.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productAllList.Add(element);
                    }
                    foreach (var element in db.ProductsStock.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productAllList.Add(element);
                    }
                    foreach (var element in db.ProductsSold.ToList())
                    {
                        if (element.Date >= from && element.Date <= to.AddDays(1).AddMilliseconds(-1))
                            productAllList.Add(element);
                    }
                    DataGridAll.ItemsSource = productAllList;
                }
            }
        }
        #endregion

        #region События
        // При нажатии на кнопку "Добавить" создаем и открывает модальное окно
        private void ButtonAcceptedAdd_Click(object sender, RoutedEventArgs e)
        {
            addProductWindow = new AddProductWindow();
            addProductWindow.Closed += AddProductWindow_Closed;
            addProductWindow.ShowDialog();
        }

        // Когда модальное окно закрывается, обновляем таблицы "Все" и "Принято"
        private void AddProductWindow_Closed(object sender, EventArgs e)
        {
            UpdateDataGrid("DataGridAccepted");
            UpdateDataGrid("DataGridAll");
        }

        // Перемещаем выбранный товар из таблицы "Принято" в таблицу "На складе"
        private void DataGridAcceptedContextMenu_Click(object sender, EventArgs e)
        {
            ProductAccepted productAccepted = (ProductAccepted)DataGridAccepted.SelectedItem;
            if (productAccepted != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.ProductsAccepted.Remove(productAccepted);
                    ProductStock productStock = new ProductStock()
                    {
                        Name = productAccepted.Name,
                        Category = productAccepted.Category,
                        Cost = productAccepted.Cost,
                        Date = DateTime.Now
                    };
                    db.ProductsStock.Add(productStock);
                    db.SaveChanges();
                }
                UpdateDataGrid("DataGridAccepted");
                UpdateDataGrid("DataGridStock");
                UpdateDataGrid("DataGridAll");
            }
        }

        // Перемещаем выбранный товар из таблицы "На складе" в таблицу "Продано"
        private void DataGridStockContextMenu_Click(object sender, EventArgs e)
        {
            ProductStock productStock = (ProductStock)DataGridStock.SelectedItem;
            if (productStock != null)
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.ProductsStock.Remove(productStock);
                    ProductSold productSold = new ProductSold()
                    {
                        Name = productStock.Name,
                        Category = productStock.Category,
                        Cost = productStock.Cost,
                        Date = DateTime.Now
                    };
                    db.ProductsSold.Add(productSold);
                    db.SaveChanges();
                }
                UpdateDataGrid("DataGridStock");
                UpdateDataGrid("DataGridSold");
                UpdateDataGrid("DataGridAll");
            }
        }

        // При смене даты в фильтре обновляем таблицы
        private void DatePickerFrom_SelectedDateChange(object sender, EventArgs e)
        {
            UpdateDataGrid("DataGridAccepted");
            UpdateDataGrid("DataGridStock");
            UpdateDataGrid("DataGridSold");
            UpdateDataGrid("DataGridAll");
        }

        private void DatePickerTo_SelectedDateChange(object sender, EventArgs e)
        {
            UpdateDataGrid("DataGridAccepted");
            UpdateDataGrid("DataGridStock");
            UpdateDataGrid("DataGridSold");
            UpdateDataGrid("DataGridAll");
        }
        #endregion
    }
}
