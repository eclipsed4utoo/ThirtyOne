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
using System.IO;

namespace ThirtyOne
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool updateData = false;

        public MainWindow()
        {
            InitializeComponent();

#if !DEBUG
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThirtyOne");
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
#else
            //string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThirtyOne");
            //Directory.CreateDirectory(path);
#endif

            Database db = new Database();
            db.CheckForDatabase();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer ac = new AddCustomer();
            ac.ParentWindow = this;
            this.Hide();
            ac.Show();
            updateData = true;
        }

        private void NewParty_Click(object sender, RoutedEventArgs e)
        {
            NewParty party = new NewParty();
            party.ParentWindow = this;
            this.Hide();
            party.Show();
            updateData = true;
        }

        private void ViewParty_Click(object sender, RoutedEventArgs e)
        {
            ViewParty party = new ViewParty();
            party.ParentWindow = this;
            this.Hide();
            party.Show();
            updateData = true;
        }

        private void NewOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrder order = new NewOrder();
            order.ParentWindow = this;
            this.Hide();
            order.Show();
            updateData = true;
        }

        private void ViewOrders_Click(object sender, RoutedEventArgs e)
        {
            ViewOrders order = new ViewOrders();
            order.ParentWindow = this;
            this.Hide();
            order.Show();
            updateData = true;
        }

        private void ViewCustomerOrders_Click(object sender, RoutedEventArgs e)
        {
            ViewCustomerOrders customerOrders = new ViewCustomerOrders();
            customerOrders.ParentWindow = this;
            this.Hide();
            customerOrders.Show();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (!updateData)
                return;

            UnpaidOrdersControl.UpdateData();
            CommissionTotalsControl.UpdateData();
            SalesTotalsControl.UpdateData();
            updateData = false;
        }
    }
}
