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
using System.Windows.Shapes;

namespace Reminder
{
    /// <summary>
    /// Interaction logic for standardWorkName.xaml
    /// </summary>
    public partial class standardWorkName : Window
    {
        MainWindow parentW;   
        public standardWorkName(MainWindow sender)
        {
            InitializeComponent();
            parentW = sender;
            this.Left = parentW.btnAddSW.Margin.Left + parentW.Left;
            this.Top = parentW.btnAddSW.Margin.Top + parentW.Top;
        }

        private void tbWorkName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Hide();
                this.Close();
            }
        }

        private void swnForm_Loaded(object sender, RoutedEventArgs e)
        {
            tbWorkName.SelectAll();
            tbWorkName.Focus();
        }
    }
}
