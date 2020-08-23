using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WpfApp1
{
    public partial class AddProductWindow : Window
    {
        public AddProductWindow()
        {
            InitializeComponent();
        }

        private void CancelAddProductButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkAddProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем и инициализируем новый продукт
            ProductAccepted product = new ProductAccepted() 
            { 
                Name = ProductName.Text, 
                Category = ProductCategory.Text, 
                Cost = Decimal.Parse(ProductCost.Text, CultureInfo.InvariantCulture), 
                Date = DateTime.Now
            };
            // Добавляем продукт в базу данных
            using(ApplicationContext db = new ApplicationContext())
            {
                db.ProductsAccepted.Add(product);
                db.SaveChanges();
            }
            this.Close();
        }
    }
}
