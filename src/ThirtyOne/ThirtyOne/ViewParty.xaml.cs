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
    /// Interaction logic for ViewParty.xaml
    /// </summary>
    public partial class ViewParty : Window
    {
        #region Local Variables

        private TaskScheduler scheduler = null;
        private ThirtyOneEntities te = null;

        public Window ParentWindow { get; set; }

        #endregion

        #region Constructor

        public ViewParty()
        {
            InitializeComponent();

            this.scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            te = new ThirtyOneEntities();
        }

        #endregion

        #region Window Events

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (te != null)
                te.Dispose();

            ParentWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PopulatePartiesComboBox();
            PopulatePaymentTypeComboBox();
        }

        #endregion

        #region Custom Methods

        public void PopulatePartiesComboBox()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                //PartyDateComboBox.DisplayMemberPath = "PartyDate";
                //PartyDateComboBox.SelectedValuePath = "PartyID";

                Task.Factory.StartNew<List<Party>>(() => GetParties())
                            .ContinueWith((list) => PartyDateComboBox.ItemsSource = list.Result, this.scheduler);
            }
        }

        private List<Party> GetParties()
        {
            List<Party> list = new List<Party>();

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var custs = from c in te.Parties
                            orderby c.PartyDate descending
                            select c;

                list = custs.ToList();
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

        private void PopulatePartyData()
        {
            Party party = PartyDateComboBox.SelectedItem as Party;

            var orders = from t in te.Orders
                         where t.PartyID == party.PartyID
                         select t;

            PartyDataGrid.ItemsSource = orders;
        }

        #endregion

        #region SelectionChanged Events

        private void PartyDateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartyDateComboBox.SelectedIndex < 0)
                return;

            PopulatePartyData();
        }

        private void PartyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartyDataGrid.SelectedIndex < 0)
                return;

            Order order = PartyDataGrid.SelectedItem as Order;

            CustomerNameTextBlock.Text = order.Customer.CustomerName;
            OrderTotalTextBlock.Text = order.OrderTotal.ToString();
            IsPaidCheckBox.IsChecked = order.IsPaid;
            PaymentTypeComboBox.SelectedValue = order.PaymentTypeID;
        }

        #endregion

        #region Click Events

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (ThirtyOneEntities toe = new ThirtyOneEntities())
            {
                Guid orderID = (PartyDataGrid.SelectedItem as Order).OrderID;
                Order order = toe.Orders.Single(t => t.OrderID == orderID);

                order.IsPaid = IsPaidCheckBox.IsChecked.Value;

                if (PaymentTypeComboBox.SelectedItem != null)
                    order.PaymentTypeID = (PaymentTypeComboBox.SelectedItem as PaymentType).PaymentTypeID;
                else
                    order.PaymentTypeID = GetNonePaymentTypeID();

                toe.SaveChanges();
            }

            PartyDataGrid.SelectedIndex = -1;
            CustomerNameTextBlock.Text = string.Empty;
            OrderTotalTextBlock.Text = string.Empty;
            IsPaidCheckBox.IsChecked = false;
            PaymentTypeComboBox.SelectedIndex = -1;

            te.Dispose();
            te = new ThirtyOneEntities();

            Guid partyID = (PartyDateComboBox.SelectedItem as Party).PartyID;

            var orders = from t in te.Orders
                            where t.PartyID == partyID
                            select t;

            PartyDataGrid.ItemsSource = orders;
        }

        #endregion
    }
}
