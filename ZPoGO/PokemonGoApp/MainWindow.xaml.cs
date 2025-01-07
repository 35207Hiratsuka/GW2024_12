using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PokemonGoApp {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData() {
            string apiUrl = "https://pokemon-go-api.github.io/pokemon-go-api/api/pokedex.json";
            try {
                string jsonData = await GetPokemonDataAsync(apiUrl);
                List<Pokemon> pokemons = ParsePokemonData(jsonData);

                // dexNrでソートしてデータグリッドに表示
                pokemons.Sort((p1, p2) => p1.DexNr.CompareTo(p2.DexNr));
                PokemonDataGrid.ItemsSource = pokemons;
            } catch(HttpRequestException httpEx) {
                MessageBox.Show(httpEx.Message);
            } catch(TaskCanceledException tcEx) {
                MessageBox.Show(tcEx.Message);
            } catch(Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task<string> GetPokemonDataAsync(string apiUrl) {
            using(HttpClient client = new HttpClient()) {
                client.Timeout = TimeSpan.FromSeconds(30); // タイムアウトを30秒に設定

                for(int i = 0; i < 3; i++) {
                    try {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode();
                        string responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    } catch(TaskCanceledException ex) when(ex.InnerException is TimeoutException) {
                        await Task.Delay(TimeSpan.FromSeconds(5)); // 5秒の遅延
                    }
                }

                throw new Exception("Failed to fetch data after multiple attempts.");
            }
        }

        public List<Pokemon> ParsePokemonData(string jsonData) {
            List<PokemonData> pokemonData = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData);
            List<Pokemon> pokemons = new List<Pokemon>();

            string defaultImagePath = "https://png.pngtree.com/png-clipart/20200731/ourlarge/pngtree-cartoon-sign-sign-simple-illustration-cute-image-red-and-white-slash-png-image_2317707.jpg";

            foreach(var data in pokemonData) {
                pokemons.Add(new Pokemon {
                    DexNr = data.DexNr,
                    JapaneseName = data.Names.Japanese,
                    EnglishName = data.Names.English,
                    Types = FormatTypes(data.PrimaryType, data.SecondaryType),
                    Stats = FormatStats(data.Stats),
                    iconImage = GetImage(string.IsNullOrEmpty(data.Assets?.Image) ? defaultImagePath : data.Assets.Image),
                    iconShinyImage = GetImage(string.IsNullOrEmpty(data.Assets?.ShinyImage) ? defaultImagePath : data.Assets.ShinyImage)
                });
            }

            return pokemons;
        }

        private string FormatTypes(Type primaryType, Type secondaryType) {
            string primary = primaryType != null && primaryType.Names != null ? $"タイプ１:{primaryType.Names.Japanese}" : "タイプ１:(なし)";
            string secondary = secondaryType != null && secondaryType.Names != null ? $"タイプ２:{secondaryType.Names.Japanese}" : "タイプ２:(なし)";
            return $"{primary}\n{secondary}";
        }

        private string FormatStats(Stats stats) {
            if(stats == null) {
                return $"ステータス情報なし\nステータス情報なし\nステータス情報なし";
            }
            return $"HP:{stats.Stamina}\nこうげき:{stats.Attack}\nぼうぎょ:{stats.Defense}";
        }

        private BitmapImage GetImage(string url) {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.EndInit();
            return image;
        }
    }
}
