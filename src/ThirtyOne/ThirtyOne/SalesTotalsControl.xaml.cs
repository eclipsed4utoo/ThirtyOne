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
    /// Interaction logic for SalesTotalsControl.xaml
    /// </summary>
    public partial class SalesTotalsControl : UserControl
    {
        public SalesTotalsControl()
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
            DateTime now = DateTime.Now;
            DateTime startOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endOfMonth = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            DateTime startOfYear = new DateTime(now.Year, 1, 1);
            DateTime endOfYear = new DateTime(now.Year, 1, 1).AddYears(1);

            using (ThirtyOneEntities te = new ThirtyOneEntities())
            {
                var thisMonth = from c in te.Orders
                                where c.OrderDate >= startOfMonth
                                && c.OrderDate < endOfMonth
                                && c.IsPaid == true
                                group c by c.IsPaid into g
                                select new
                                {
                                    MonthlyTotal = g.Sum(t => t.OrderSubTotal)
                                };

                if (thisMonth.Count() != 0)
                    ThisMonthTotal.Text = string.Format("{0:c}", thisMonth.First().MonthlyTotal);
                else
                    ThisMonthTotal.Text = string.Format("{0:c}", 0);

                var thisYear = from c in te.Orders
                               where c.OrderDate >= startOfYear
                               && c.OrderDate < endOfYear
                               && c.IsPaid == true
                               group c by c.IsPaid into g
                               select new
                               {
                                   YearlyTotal = g.Sum(t => t.OrderSubTotal)
                               };

                if (thisYear.Count() != 0)
                    ThisYearTotal.Text = string.Format("{0:c}", thisYear.First().YearlyTotal);
                else
                    ThisYearTotal.Text = string.Format("{0:c}", 0);

                var total = from c in te.Orders
                            where c.IsPaid == true
                            group c by c.IsPaid into g
                            select new
                            {
                                TotalTotal = g.Sum(t => t.OrderSubTotal)
                            };
                if(total.Count() != 0)
                    Total.Text = string.Format("{0:c}", total.First().TotalTotal);
                else
                    Total.Text = string.Format("{0:c}", 0);
            }
        }
    }
}
