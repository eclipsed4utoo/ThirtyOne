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
    /// Interaction logic for AddCustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public Window ParentWindow { get; set; }

        public event EventHandler CustomerAddCompleted;

        public AddCustomer()
        {
            InitializeComponent();
        }

        private void OnCustomerAddCompleted()
        {
            EventHandler handler = CustomerAddCompleted;
            if (handler != null)
            {
                handler(null, null);
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text))
                return;

            this.Cursor = Cursors.Wait;

            try
            {
                using (ThirtyOneEntities toe = new ThirtyOneEntities())
                {
                    Customer cust = toe.Customers.SingleOrDefault(t => t.CustomerName == CustomerNameTextBox.Text.Trim());

                    if (cust != null)
                    {
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show("Customer already exists.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                string error = string.Format("{0}{1}{2}{3}", ex.Message, Environment.NewLine, (ex.InnerException != null) ? ex.InnerException.Message : "", ex.StackTrace);
                MessageBox.Show(error);
                return;
            }

            try
            {
                using (ThirtyOneEntities te = new ThirtyOneEntities())
                {
                    Customer c = te.CreateObject<Customer>();
                    c.CustomerID = Guid.NewGuid();
                    c.CustomerName = CustomerNameTextBox.Text.Trim();

                    te.Customers.AddObject(c);
                    te.SaveChanges();
                }

                CustomerNameTextBox.Text = string.Empty;
                OnCustomerAddCompleted();

                Storyboard saveCompleteSB = (Storyboard)FindResource("SaveCompleteStoryboard");
                saveCompleteSB.Begin();

                CustomerNameTextBox.Focus();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message);
                return;
            }

            this.Cursor = Cursors.Arrow;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ParentWindow.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CustomerNameTextBox.Focus();
        }
    }
}
