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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for OrderControl.xaml
    /// </summary>
    public partial class OrderControl : UserControl
    {
        #region Local Variables

        double productSubTotal = 0;
        double taxPercent = 0.08;
        double shippingPercentage = 0.08;
        double taxTotal = 0;
        double totalPrice = 0;
        double shippingTotal = 0;

        private TaskScheduler scheduler = null;

        #endregion

        #region Public Properties

        public static readonly DependencyProperty IsClosableProperty =
                DependencyProperty.Register("IsClosable", typeof(bool), typeof(OrderControl));

        public bool IsClosable
        {
            get { return (bool)GetValue(IsClosableProperty); }
            set { SetValue(IsClosableProperty, value); }
        }

        public static readonly DependencyProperty IsPartyProperty =
              DependencyProperty.Register("IsParty", typeof(bool), typeof(OrderControl));

        public bool IsParty
        {
            get { return (bool)GetValue(IsPartyProperty); }
            set { SetValue(IsPartyProperty, value); }
        }

        public Guid PaymentTypeID
        {
            get 
            {
                PaymentType pt = PaymentTypeComboBox.SelectedItem as PaymentType;
                if (pt == null)
                    return GetNonePaymentTypeID();
                else
                    return pt.PaymentTypeID;
            }
        }

        public bool IsPaid
        {
            get { return PaidCheckBox.IsChecked.Value; }
        }

        public Guid CustomerID
        {
            get 
            {
                Customer cust = CustomersComboBox.SelectedItem as Customer;
                if (cust == null)
                    return Guid.Empty;
                else
                    return cust.CustomerID; 
            }
        }

        public Customer Customer
        {
            get { return (CustomersComboBox.SelectedItem as Customer); }
        }

        public double ProductSubTotal
        {
            get { return productSubTotal; }
        }

        public double TaxTotal
        {
            get { return taxTotal; }
        }

        public double TotalPrice
        {
            get { return totalPrice; }
        }

        public double ShippingTotal
        {
            get { return shippingTotal; }
        }

        public double OldTotal { get; set; }

        #endregion

        #region Custom Events

        public event EventHandler DeleteControl;
        public event EventHandler TotalUpdated;

        private void OnTotalUpdated()
        {
            EventHandler handler = TotalUpdated;
            if (handler != null)
                handler(this, null);
        }

        private void OnDeleteControl()
        {
            EventHandler handler = DeleteControl;
            if (handler != null)
                handler(this, null);
        }

        #endregion

        #region Constructor

        public OrderControl()
        {
            InitializeComponent();

            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        #endregion

        #region Control Events

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PopulateCustomerComboBox();
            PopulatePaymentTypeComboBox();

            RemoveButton.Visibility = (this.IsClosable) ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion

        #region Custom Methods

        public void PopulateCustomerComboBox()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CustomersComboBox.DisplayMemberPath = "CustomerName";
                CustomersComboBox.SelectedValuePath = "CustomerID";

                Task.Factory.StartNew<List<Customer>>(() => GetCustomers())
                            .ContinueWith((list) => SetCustomersDataSource(list.Result), this.scheduler);
            }
        }

        public void SetCustomersDataSource(List<Customer> list)
        {
            Customer customer = CustomersComboBox.SelectedItem as Customer;

            CustomersComboBox.ItemsSource = list;

            if (customer == null)
                return;

            CustomersComboBox.SelectedValue = customer.CustomerID;
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

        private List<Customer> GetCustomers()
        {
            List<Customer> list = new List<Customer>();
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var custs = te.Customers;

                list = custs.ToList();
            }

            return list;
        }

        public void BeginRemoveStoryboard()
        {
            Storyboard sb = (Storyboard)FindResource("OrderControlUnloadedStoryboard");
            sb.Completed += (s, ea) =>
            {
                OnDeleteControl();
            };

            sb.Begin();
        }

        private Guid GetNonePaymentTypeID()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                return te.PaymentTypes.SingleOrDefault(t => t.PaymentTypeName == "None").PaymentTypeID;
            }
        }

        #endregion

        #region Click Events

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SubTotalTextBox.Text) || !string.IsNullOrWhiteSpace(ShippingTextBox.Text))
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this Order?\n\nCurrent order data will be lost.", "Delete Order", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No)
                    return;
            }

            BeginRemoveStoryboard();
        }

        #endregion

        #region Textbox Events

        private void SubTotalTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductSubTotalTextBox.Text))
                return;

            OldTotal = totalPrice;

            double oldProductSubTotal = productSubTotal;
            double oldTaxTotal = taxTotal;
            double oldShipping = shippingTotal;

            if (!double.TryParse(ProductSubTotalTextBox.Text, out productSubTotal))
                return;

            totalPrice -= oldTaxTotal + oldProductSubTotal + oldShipping;
            shippingTotal = Math.Round(productSubTotal * shippingPercentage, 2);

            if (!IsParty)
                shippingTotal += 4;

            taxTotal = Math.Round((productSubTotal + shippingTotal) * taxPercent, 2);
            totalPrice += Math.Round(productSubTotal + taxTotal + shippingTotal, 2);

            double subTotal = productSubTotal + shippingTotal;

            SubTotalTextBox.Text = string.Format("{0:c}", subTotal);
            TaxTextBox.Text = string.Format("{0:c}", taxTotal);
            TotalTextBox.Text = string.Format("{0:c}", totalPrice);
            ShippingTextBox.Text = string.Format("{0:c}", shippingTotal);

            OnTotalUpdated();
        }

        private void ShippingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (string.IsNullOrWhiteSpace(ShippingTextBox.Text))
            //    return;

            //OldTotal = totalPrice;

            //double oldShippingTotal = shippingTotal;

            //if (!double.TryParse(ShippingTextBox.Text, out shippingTotal))
            //    return;

            //totalPrice -= oldShippingTotal;
            //totalPrice += Math.Round(shippingTotal, 2);

            //TotalTextBox.Text = string.Format("{0:c}", totalPrice);

            //OnTotalUpdated();
        }

        #endregion
    }
}
