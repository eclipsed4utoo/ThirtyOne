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
using System.Windows.Media.Animation;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for NewParty.xaml
    /// </summary>
    public partial class NewParty : Window
    {
        #region Local Variables and Properties

        private double partyTotal;

        public Window ParentWindow { get; set; }

        #endregion

        #region Constructor

        public NewParty()
        {
            InitializeComponent();

            PartyDatePicker.Text = DateTime.Now.ToShortDateString();
            PartyTotalTextBlock.Text = "$0.00";
        }

        #endregion

        #region Window Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Show();
        }

        #endregion

        #region Click Events

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderStackPanel.Children.Count == 0)
                return;

            this.Cursor = Cursors.Wait;

            bool hasOrderBeenAdded = false;

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                Guid partyID = Guid.NewGuid();
                Party p = new Party();
                p.PartyID = partyID;
                p.PartyDate = Convert.ToDateTime(PartyDatePicker.Text);
                p.PartyTotal = partyTotal;

                foreach (UIElement ele in OrderStackPanel.Children)
                {
                    if (ele is OrderControl)
                    {
                        OrderControl oc = ele as OrderControl;

                        if (oc.CustomerID == Guid.Empty)
                            continue;

                        if (oc.ProductSubTotal == 0)
                            continue;

                        hasOrderBeenAdded = true;

                        Order order = new Order();
                        order.CustomerID = oc.CustomerID;
                        order.IsPaid = oc.IsPaid;
                        order.OrderDate = p.PartyDate;
                        order.OrderID = Guid.NewGuid();
                        order.OrderShipping = oc.ShippingTotal;
                        order.OrderSubTotal = oc.ProductSubTotal;
                        order.OrderTax = oc.TaxTotal;
                        order.OrderTotal = oc.TotalPrice;
                        order.PartyID = p.PartyID;

                        if (order.IsPaid)
                            order.PaymentTypeID = oc.PaymentTypeID;
                        else
                            order.PaymentTypeID = GetNonePaymentTypeID();

                        te.AddToOrders(order);

                        //oc.BeginRemoveStoryboard();
                    }
                }

                if (hasOrderBeenAdded)
                {
                    te.AddToParties(p);
                    te.SaveChanges();
                }
            }

            this.Cursor = Cursors.Arrow;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            OrderStackPanel.Height += 167;

            OrderControl oc = new OrderControl();
            oc.Height = 167;
            oc.Width = 570;
            oc.Name = "oc";
            oc.Opacity = 0;
            oc.IsClosable = true;
            oc.IsParty = true;
            oc.DeleteControl += new EventHandler(OrderControl_DeleteControl);
            oc.TotalUpdated += new EventHandler(OrderControl_TotalUpdated);

            OrderStackPanel.Children.Add(oc);
        }

        private void NewCustomerButton_Click(object sender, RoutedEventArgs e)
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

        #endregion

        #region Custom Methods

        private Guid GetNonePaymentTypeID()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                return te.PaymentTypes.SingleOrDefault(t => t.PaymentTypeName == "None").PaymentTypeID;
            }
        }

        #endregion

        #region Order Control Events

        private void OrderControl_DeleteControl(object sender, EventArgs e)
        {
            OrderStackPanel.Height -= 167;
            OrderStackPanel.Children.Remove(sender as OrderControl);
        }

        private void OrderControl_TotalUpdated(object sender, EventArgs e)
        {
            OrderControl oc = sender as OrderControl;

            partyTotal -= oc.OldTotal;
            partyTotal += oc.TotalPrice;

            PartyTotalTextBlock.Text = string.Format("{0:c}", partyTotal);
        }

        #endregion
    }
}
