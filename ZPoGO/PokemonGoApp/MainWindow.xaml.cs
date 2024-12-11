using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using static System.Net.WebRequestMethods;


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

                // dexNrでソートしてリストボックスに表示
                pokemons.Sort((p1, p2) => p1.DexNr.CompareTo(p2.DexNr));
                PokemonListBox.ItemsSource = pokemons;
                PokemonListBox.DisplayMemberPath = "JapaneseName";
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

                for(int i = 0; i < 3; i++) // 最大3回までリトライ
                {
                    try {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode();
                        string responseData = await response.Content.ReadAsStringAsync();
                        return responseData;
                    } catch(TaskCanceledException ex) when(ex.InnerException is TimeoutException) {
                        // 短い遅延を追加してから再試行
                        await Task.Delay(TimeSpan.FromSeconds(2));
                    }
                }

                throw new Exception("Failed to fetch data after multiple attempts.");
            }
        }


        public List<Pokemon> ParsePokemonData(string jsonData) {
            List<PokemonData> pokemonData = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData);
            List<Pokemon> pokemons = new List<Pokemon>();

            foreach(var data in pokemonData) {
                pokemons.Add(new Pokemon {
                    DexNr = data.DexNr,
                    JapaneseName = data.Names.Japanese
                });
            }

            return pokemons;
        }
    }
}
