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
using System.Net.Http;

namespace PokeDexSystemWPF {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        private HttpClient client = new HttpClient();
        public MainWindow() {
            InitializeComponent();
        }

        private async void btnGetPokemon_Click(object sender, RoutedEventArgs e) {
            imgPokemon.Source = await GetImage(txtID.Text);
        }


        public async Task<BitmapImage> GetImage(string id) {
            BitmapImage bmpImage = new BitmapImage();

            var imgUri = $"https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/{id}.png";
            var result = await client.GetAsync(imgUri);
            if(!result.IsSuccessStatusCode) {
                MessageBox.Show(result.StatusCode.ToString());
                return bmpImage;
            }

            using(var httpStream = await result.Content.ReadAsStreamAsync()) {
                bmpImage.BeginInit();
                bmpImage.StreamSource = httpStream;
                bmpImage.DecodePixelWidth = 500;
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.EndInit();
            }

            return bmpImage;
        }
    }
}
