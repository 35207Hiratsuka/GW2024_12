using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zikken;

namespace Zikken {
    public partial class MainWindow : Window {
        private ObservableCollection<Customer> customers;

        public MainWindow() {
            InitializeComponent();

            //コンボボックスに選択できる項目のリストを追加
            cmbGender.ItemsSource = GenderNames;

            //DataGridに行を追加
            customers = new ObservableCollection<Customer>
            {
                new Customer("yamada", "山田太郎", GenderType.Men, false, true),
                new Customer("tanaka", "田中直樹", GenderType.Men, true, false),
                new Customer("satouu", "佐藤七海", GenderType.Women, false, true),
            };

            DataGrid1.ItemsSource = customers;
        }

        // Enumの定義名変更プロパティ
        public Dictionary<GenderType, string> GenderNames {
            get;
        } = new Dictionary<GenderType, string> {
            [GenderType.Men] = "男性",
            [GenderType.Women] = "女性",
            [GenderType.None] = "ー",
        };

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            customers.Add(new Customer("hirose", "広瀬花奈", GenderType.Women, true, false));
            DataGrid1.ItemsSource = customers;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            var tag = ((Button)sender).Tag as Customer;
            MessageBox.Show($"ユーザー名：{tag.UserName}\r\n名前：{tag.Name}");
        }
    }
}