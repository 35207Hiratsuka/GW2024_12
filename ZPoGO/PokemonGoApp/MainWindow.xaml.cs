// MainWindow.xaml.cs
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                for(int i = 0; i < 5; i++) {
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
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new EliteMovesConverter());
            List<PokemonData> pokemonData = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData, settings);
            List<Pokemon> pokemons = new List<Pokemon>();
            //ゲーム内データに画像未設定時の、代わりの画像を設定
            string defaultImagePath = "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEiJXYVCbpXdstc30mEWtspHcixWtjN83WZdccPF9QNtF2S9Bykwp5TcMVT8jB4FNEBModDyO_HR5BYIYCvqg_VzEXhbKy7gymQU35n5cpfBr53L_5l9rNqiiz6yR-D1aAOMlpdsvqgXMlI6/s400/mark_batsu.png";
            foreach(var data in pokemonData) {
                pokemons.Add(new Pokemon { 
                    DexNr = data.DexNr,
                    JapaneseName = data.Names.Japanese,
                    EnglishName = data.Names.English,
                    Types = FormatTypes(data.PrimaryType, data.SecondaryType),
                    Stats = FormatStats(data.Stats),
                    iconImage = GetImage(string.IsNullOrEmpty(data.Assets?.Image) ? defaultImagePath : data.Assets.Image),
                    iconShinyImage = GetImage(string.IsNullOrEmpty(data.Assets?.ShinyImage) ? defaultImagePath : data.Assets.ShinyImage),
                    NormalAttack = FormatNormalAttack(data.QuickMoves),
                    EliteNormalAttack = FormatEliteNormalAttack(data.EliteQuickMoves),
                    SpecialAttack = FormatSpecialAttack(data.CinematicMoves),
                    EliteSpecialAttack = FormatEliteSpecialAttack(data.EliteCinematicMoves)
                });
            }
            return pokemons;
        }

        private string FormatTypes(PokemonGoApp.Type primaryType, PokemonGoApp.Type secondaryType) {
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

        private static readonly Dictionary<string, BitmapImage> imageCache = new Dictionary<string, BitmapImage>();
        private BitmapImage GetImage(string url) {
            if(imageCache.ContainsKey(url)) {
                return imageCache[url];
            }
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(url, UriKind.Absolute);
            image.DecodePixelWidth = 50;
            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            imageCache[url] = image;
            return image;
        }

        private string FormatNormalAttack(Dictionary<string, Move> quickMoves) {
            List<string> moves = new List<string>();
            foreach(var move in quickMoves.Values) {
                moves.Add($"{move.Type.Names.Japanese}:{move.Names.Japanese}");
            }
            return string.Join("\n", moves);
        }

        private string FormatEliteNormalAttack(Dictionary<string, Move> eliteQuickMoves) {
            List<string> moves = new List<string>();
            foreach(var move in eliteQuickMoves.Values) {
                moves.Add($"{move.Type.Names.Japanese}:{move.Names.Japanese}*");
            }
            return string.Join("\n", moves);
        }

        private string FormatSpecialAttack(Dictionary<string, Move> cinematicMoves) {
            List<string> moves = new List<string>();
            foreach(var move in cinematicMoves.Values) {
                moves.Add($"{move.Type.Names.Japanese}:{move.Names.Japanese}");
            }
            return string.Join("\n", moves);
        }
        private string FormatEliteSpecialAttack(Dictionary<string, Move> eliteCinematicMoves) {
            List<string> moves = new List<string>();
            foreach(var move in eliteCinematicMoves.Values) {
                moves.Add($"{move.Type.Names.Japanese}:{move.Names.Japanese}*");
            }
            return string.Join("\n", moves);
        }


        public class EliteMovesConverter : JsonConverter {
            public override bool CanConvert(System.Type objectType) {
                return objectType == typeof(Dictionary<string, Move>);
            }

            public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
                var result = new Dictionary<string, Move>();
                if(reader.TokenType == JsonToken.StartArray) {
                    reader.Read(); // Consume the start array token
                    if(reader.TokenType == JsonToken.EndArray) {
                        return result; // Return an empty dictionary if the array is empty
                    }
                } else if(reader.TokenType == JsonToken.StartObject) {
                    JObject obj = JObject.Load(reader);
                    foreach(var property in obj.Properties()) {
                        var move = property.Value.ToObject<Move>(serializer);
                        result.Add(property.Name, move);
                    }
                    return result;
                }
                throw new JsonSerializationException("Unexpected token type: " + reader.TokenType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                throw new NotImplementedException();
            }
        }
    }
}
