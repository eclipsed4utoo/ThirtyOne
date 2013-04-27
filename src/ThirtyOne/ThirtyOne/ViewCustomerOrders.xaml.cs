using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for ViewCustomerOrders.xaml
    /// </summary>
    public partial class ViewCustomerOrders : Window
    {
        public MainWindow ParentWindow { get; set; }

        private TaskScheduler scheduler = null;
        private ThirtyOneEntities te = null;

        public ViewCustomerOrders()
        {
            InitializeComponent();

            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            te = new ThirtyOneEntities();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                PopulateCustomersComboBox();
            }
        }

        private List<Customer> GetCustomers()
        {
            List<Customer> list = new List<Customer>();

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var custs = from c in te.Orders
                            select c.Customer;

                list = custs.Distinct().ToList();
            }

            return list;
        }

        private void PopulateCustomerData()
        {
            Customer customer = CustomerNameComboBox.SelectedItem as Customer;
            var orders = from t in te.Orders
                         where t.CustomerID == customer.CustomerID
                         select t;

            OrdersDataGrid.ItemsSource = orders;
        }

        private void PopulateCustomersComboBox()
        {
            Task.Factory.StartNew<List<Customer>>(() => GetCustomers())
                            .ContinueWith((list) => CustomerNameComboBox.ItemsSource = list.Result, this.scheduler);
        }

        private void CustomerNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerNameComboBox.SelectedIndex < 0)
                return;

            PopulateCustomerData();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            ParentWindow.Show();
        }
    }
}
