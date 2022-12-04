using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleTimeTracker.View
{
    /// <summary>
    /// TimeTrackerPage.xaml の相互作用ロジック
    /// </summary>
    public partial class TimeTrackerPage : UserControl
    {
        public TimeTrackerPage()
        {
            InitializeComponent();
        }

        private void TopMost_Unchecked(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Topmost = false;
        }

        private void TopMost_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.Topmost = true;
        }
    }
}
