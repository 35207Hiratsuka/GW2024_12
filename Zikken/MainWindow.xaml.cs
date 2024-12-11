using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zikken;

namespace Zikken { //View
    public partial class MainWindow : Window {
        private ObservableCollection<Customer> customers;

        public MainWindow() {
            InitializeComponent();
        }
    }
}