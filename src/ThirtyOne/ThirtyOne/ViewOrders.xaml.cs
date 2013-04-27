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
using System.Threading.Tasks;
using System.ComponentModel;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for ViewOrders.xaml
    /// </summary>
    public partial class ViewOrders : Window
    {
        #region Local Variables

        private TaskScheduler scheduler = null;
        private ThirtyOneEntities te = null;

        public MainWindow ParentWindow { get; set; }

        #endregion

        #region Constructor

        public ViewOrders()
        {
            InitializeComponent();

            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            te = new ThirtyOneEntities();
        }

        #endregion

        #region Window Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulatePartiesComboBox();
            PopulatePaymentTypeComboBox();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (te != null)
                te.Dispose();

            ParentWindow.Show();
        }

        #endregion

        #region Click Events

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItems.Count == 0)
                return;

            using (ThirtyOneEntities toe = new ThirtyOneEntities())
            {
                Guid orderID = (OrdersDataGrid.SelectedItem as Order).OrderID;
                Order order = toe.Orders.Single(t => t.OrderID == orderID);

                toe.DeleteObject(order);

                toe.SaveChanges();
            }

            OrdersDataGrid.SelectedIndex = -1;
            CustomerNameTextBlock.Text = string.Empty;
            OrderTotalTextBlock.Text = string.Empty;
            IsPaidCheckBox.IsChecked = false;
            PaymentTypeComboBox.SelectedIndex = -1;

            te.Dispose();
            te = new ThirtyOneEntities();

            DateTime orderDate = Convert.ToDateTime(OrdersDateComboBox.SelectedValue.ToString());

            var orders = from t in te.Orders
                         where t.OrderDate == orderDate
                         && t.PartyID == null
                         select t;

            OrdersDataGrid.ItemsSource = orders;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItems.Count == 0)
                return;

            using (ThirtyOneEntities toe = new ThirtyOneEntities())
            {
                Guid orderID = (OrdersDataGrid.SelectedItem as Order).OrderID;
                Order order = toe.Orders.Single(t => t.OrderID == orderID);

                order.IsPaid = IsPaidCheckBox.IsChecked.Value;

                if (PaymentTypeComboBox.SelectedItem != null)
                    order.PaymentTypeID = (PaymentTypeComboBox.SelectedItem as PaymentType).PaymentTypeID;
                else
                    order.PaymentTypeID = GetNonePaymentTypeID();

                toe.SaveChanges();
            }

            OrdersDataGrid.SelectedIndex = -1;
            CustomerNameTextBlock.Text = string.Empty;
            OrderTotalTextBlock.Text = string.Empty;
            IsPaidCheckBox.IsChecked = false;
            PaymentTypeComboBox.SelectedIndex = -1;

            te.Dispose();
            te = new ThirtyOneEntities();

            DateTime orderDate = Convert.ToDateTime(OrdersDateComboBox.SelectedValue.ToString());

            var orders = from t in te.Orders
                         where t.OrderDate == orderDate
                         && t.PartyID == null
                         select t;

            OrdersDataGrid.ItemsSource = orders;
        }

        #endregion

        #region SelectionChanged Events

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDataGrid.SelectedIndex < 0)
                return;

            Order order = OrdersDataGrid.SelectedItem as Order;

            CustomerNameTextBlock.Text = order.Customer.CustomerName;
            OrderTotalTextBlock.Text = order.OrderTotal.ToString();
            IsPaidCheckBox.IsChecked = order.IsPaid;
            PaymentTypeComboBox.SelectedValue = order.PaymentTypeID;
        }

        private void OrdersDateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OrdersDateComboBox.SelectedIndex < 0)
                return;

            PopulateOrdersData();
        }

        #endregion

        #region Custom Methods

        public void PopulatePartiesComboBox()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //PartyDateComboBox.DisplayMemberPath = "PartyDate";
                //PartyDateComboBox.SelectedValuePath = "PartyID";

                Task.Factory.StartNew<List<DateTime>>(() => GetOrderDates())
                            .ContinueWith((list) => OrdersDateComboBox.ItemsSource = list.Result, this.scheduler);
            }
        }

        private List<DateTime> GetOrderDates()
        {
            List<DateTime> list = new List<DateTime>();

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var custs = from c in te.Orders
                            where c.PartyID == null
                            orderby c.OrderDate descending
                            select c.OrderDate;

                list = custs.Distinct().ToList();
            }

            return list;
        }

        private void PopulatePaymentTypeComboBox()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                PaymentTypeComboBox.DisplayMemberPath = "PaymentTypeName";
                PaymentTypeComboBox.SelectedValuePath = "PaymentTypeID";

                Task.Factory.StartNew<List<PaymentType>>(() => GetPaymentTypes())
                            .ContinueWith((list) => PaymentTypeComboBox.ItemsSource = list.Result, this.scheduler);
            }
        }

        private List<PaymentType> GetPaymentTypes()
        {
            List<PaymentType> list = new List<PaymentType>();

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var custs = from c in te.PaymentTypes
                            orderby c.PaymentTypeName descending
                            select c;

                list = custs.ToList();
            }

            return list;
        }

        private Guid GetNonePaymentTypeID()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                return te.PaymentTypes.SingleOrDefault(t => t.PaymentTypeName == "None").PaymentTypeID;
            }
        }

        private void PopulateOrdersData()
        {
            DateTime orderDate = Convert.ToDateTime(OrdersDateComboBox.SelectedValue.ToString());
            var orders = from t in te.Orders
                         where t.PartyID == null
                         && t.OrderDate == orderDate
                         select t;

            OrdersDataGrid.ItemsSource = orders;
        }

        #endregion


    }
}
