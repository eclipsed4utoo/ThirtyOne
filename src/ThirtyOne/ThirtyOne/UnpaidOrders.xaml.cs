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

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for UnpaidOrders.xaml
    /// </summary>
    public partial class UnpaidOrders : UserControl
    {
        public UnpaidOrders()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                UpdateData();
            }
        }

        public void UpdateData()
        {
            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var unpaidOrders = from c in te.Orders
                                   where c.IsPaid == false
                                   && c.OrderTotal != null
                                   select new UnpaidOrder
                                   {
                                       OrderDate = c.OrderDate,
                                       CustomerName = c.Customer.CustomerName,
                                       OrderTotal = c.OrderTotal.Value,
                                       IsPaid = false
                                   };

                UnpaidOrdersGrid.ItemsSource = unpaidOrders;
            }
        }
    }
}
