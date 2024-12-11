using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace PokemonApp {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }

        private async void LoadData() {
            string apiUrl = "https://api.sleepapi.net/api/pokemon"; // API URL
            string jsonData = await GetPokemonDataAsync(apiUrl);
            List<string> pokemonNames = ParsePokemonData(jsonData);
            PokemonListBox.ItemsSource = pokemonNames;
        }

        public async Task<string> GetPokemonDataAsync(string apiUrl) {
            using(HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();
                return responseData;
            }
        }

        public List<string> ParsePokemonData(string jsonData) {
            List<string> pokemonNames = JsonConvert.DeserializeObject<List<string>>(jsonData);
            return pokemonNames;
        }
    }
}
