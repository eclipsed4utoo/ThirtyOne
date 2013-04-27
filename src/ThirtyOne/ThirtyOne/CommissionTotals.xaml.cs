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
    /// Interaction logic for CommissionTotals.xaml
    /// </summary>
    public partial class CommissionTotals : UserControl
    {
        public CommissionTotals()
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

            double ordersTotal = 0;
            double commissionTotal = 0;

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

                if (thisMonth.Count() > 0)
                {
                    ordersTotal = (thisMonth.First().MonthlyTotal.HasValue) ? thisMonth.First().MonthlyTotal.Value : 0;
                    commissionTotal = Math.Round(ordersTotal * .25, 2);
                    ThisMonthTotal.Text = string.Format("{0:c}", commissionTotal);
                }
                else
                {
                    ThisMonthTotal.Text = string.Format("{0:c}", 0);
                }

                var thisYear = from c in te.Orders
                               where c.OrderDate >= startOfYear
                               && c.OrderDate < endOfYear
                               && c.IsPaid == true
                               group c by c.IsPaid into g
                               select new
                               {
                                   YearlyTotal = g.Sum(t => t.OrderSubTotal)
                               };

                if (thisYear.Count() > 0)
                {
                    ordersTotal = (thisYear.First().YearlyTotal.HasValue) ? thisYear.First().YearlyTotal.Value : 0;
                    commissionTotal = Math.Round(ordersTotal * .25, 2);

                    ThisYearTotal.Text = string.Format("{0:c}", commissionTotal);
                }
                else
                {
                    ThisYearTotal.Text = string.Format("{0:c}", 0);
                }

                var total = from c in te.Orders
                            where c.IsPaid == true
                            group c by c.IsPaid into g
                            select new
                            {
                                TotalTotal = g.Sum(t => t.OrderSubTotal)
                            };

                if (total.Count() > 0)
                {
                    ordersTotal = (total.First().TotalTotal.HasValue) ? total.First().TotalTotal.Value : 0;
                    commissionTotal = Math.Round(ordersTotal * .25, 2);

                    Total.Text = string.Format("{0:c}", commissionTotal);
                }
                else
                {
                    Total.Text = string.Format("{0:c}", 0);
                }
            }
        }
    }
}
