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
using System.Windows.Media.Animation;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        public MainWindow ParentWindow { get; set; }

        private OrderControl orderControl;

        public NewOrder()
        {
            InitializeComponent();

            CreateNewOrder();

            PartyDatePicker.Text = DateTime.Now.ToShortDateString();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Show();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (orderControl.CustomerID == Guid.Empty)
                return;

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                Order order = new Order();
                order.CustomerID = orderControl.CustomerID;
                order.IsPaid = orderControl.IsPaid;
                order.OrderDate = Convert.ToDateTime(PartyDatePicker.Text);
                order.OrderID = Guid.NewGuid();
                order.OrderShipping = orderControl.ShippingTotal;
                order.OrderSubTotal = orderControl.ProductSubTotal;
                order.OrderTax = orderControl.TaxTotal;
                order.OrderTotal = orderControl.TotalPrice;

                if (order.IsPaid && orderControl.PaymentTypeID != null)
                    order.PaymentTypeID = orderControl.PaymentTypeID;
                else
                    order.PaymentTypeID = GetNonePaymentTypeID();

                te.AddToOrders(order);
                te.SaveChanges();
            }

            orderControl.BeginRemoveStoryboard();

            Storyboard saveCompleteSB = (Storyboard)FindResource("SaveCompleteStoryboard");
            saveCompleteSB.Begin();
        }

        #region Custom Methods

        private Guid GetNonePaymentTypeID()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                return te.PaymentTypes.SingleOrDefault(t => t.PaymentTypeName == "None").PaymentTypeID;
            }
        }

        private void CreateNewOrder()
        {
            orderControl = new OrderControl();
            orderControl.Height = 167;
            orderControl.Width = 570;
            orderControl.Name = "oc2";
            orderControl.Opacity = 0;
            orderControl.IsClosable = false;
            orderControl.IsParty = false;
            orderControl.DeleteControl += new EventHandler(oc_DeleteControl);

            OrderStackPanel.Children.Add(orderControl);
        }

        #endregion

        private void oc_DeleteControl(object sender, EventArgs e)
        {
            OrderControl oc2 = sender as OrderControl;

            oc2.DeleteControl -= new EventHandler(oc_DeleteControl);

            OrderStackPanel.Children.Remove(oc2);

            CreateNewOrder();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer ac = new AddCustomer();
            ac.ParentWindow = this;
            ac.CustomerAddCompleted += (s, es) =>
            {
                foreach (UIElement ele in OrderStackPanel.Children)
                {
                    if (ele is OrderControl)
                    {
                        (ele as OrderControl).PopulateCustomerComboBox();
                    }
                }
            };

            this.Hide();
            ac.Show();
        }
    }
}
