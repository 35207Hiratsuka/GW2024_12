using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System;
using System.Linq;

namespace PokemonApp {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            LoadData();
        }

        private static readonly HttpClient client = new HttpClient();

        private async void LoadData() {
            string sleepApiUrl = "https://api.sleepapi.net/api/pokemon"; // SleepAPI URL
            string pokeApiUrl = "https://pokeapi.co/api/v2/pokemon?limit=10000&offset=0"; // PokeAPI URL
            string pokeSpeciesApiUrl = "https://pokeapi.co/api/v2/pokemon-species"; // PokeAPI Species URL

            var sleepTask = GetPokemonDataAsync(sleepApiUrl);
            var pokeTask = GetPokemonDataAsync(pokeApiUrl);

            await Task.WhenAll(sleepTask, pokeTask);

            string sleepJsonData = await sleepTask;
            string pokeJsonData = await pokeTask;

            List<string> sleepPokemonNames = ParseSleepPokemonData(sleepJsonData);
            List<Pokemon> pokePokemonData = ParsePokePokemonData(pokeJsonData);

            var japaneseNameTasks = pokePokemonData.Select(async p => {
                p.JapaneseName = await GetJapaneseName(p.Id, pokeSpeciesApiUrl);
            }).ToList();

            await Task.WhenAll(japaneseNameTasks);

            var additionalJapaneseNames = new Dictionary<string, string>();

            var additionalTasks = sleepPokemonNames.Select(async sleepName => {
                var formattedName = (sleepName.ToLower() == "mimikyu") ? "mimikyu-disguised" : sleepName.ToLower().Replace("_", "-");
                if(!pokePokemonData.Any(p => p.Name == formattedName)) {
                    var originalId = GetOriginalPokemonId(sleepName, pokePokemonData);
                    if(originalId != -1) {
                        var japaneseName = await GetJapaneseName(originalId, pokeSpeciesApiUrl);
                        additionalJapaneseNames[sleepName] = japaneseName;
                    } else {
                        additionalJapaneseNames[sleepName] = "不明";
                    }
                }
            }).ToList();

            await Task.WhenAll(additionalTasks);

            var berryNameTasks = sleepPokemonNames.Select(async sleepName => {
                var berryName = await GetBerryName(sleepName, "https://api.sleepapi.net/api/pokemon/");
                var berryJapaneseName = await GetJapaneseBerryName(berryName, "https://pokeapi.co/api/v2/berry/");
                var berryImageUrl = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/berries/{berryName.ToLower()}.png";
                var typeName = await GetTypeName(sleepName, "https://api.sleepapi.net/api/pokemon/", "https://pokeapi.co/api/v2/type/");
                var specialty = await GetSpecialty(sleepName, "https://api.sleepapi.net/api/pokemon/");
                var ingredientData = await GetIngredientData(sleepName, "https://api.sleepapi.net/api/pokemon/");
                return new {
                    PokemonName = sleepName,
                    BerryName = berryJapaneseName,
                    BerryImageUrl = berryImageUrl,
                    TypeName = typeName,
                    Specialty = specialty,
                    IngredientAmountA = ingredientData.amountDisplayA,
                    IngredientImageUrlA = ingredientData.imageUrlA,
                    IngredientAmountB = ingredientData.amountDisplayB,
                    IngredientImageUrlB = ingredientData.imageUrlB,
                    IngredientAmountC = ingredientData.amountDisplayC,
                    IngredientImageUrlC = ingredientData.imageUrlC
                };
            }).ToList();

            var berryNames = await Task.WhenAll(berryNameTasks);

            var berryNameDict = berryNames.ToDictionary(b => b.PokemonName, b => new {
                b.BerryName,
                b.BerryImageUrl,
                b.TypeName,
                b.Specialty,
                b.IngredientAmountA,
                b.IngredientImageUrlA,
                b.IngredientAmountB,
                b.IngredientImageUrlB,
                b.IngredientAmountC,
                b.IngredientImageUrlC
            });

            var combinedData = from sleepName in sleepPokemonNames
                               let formattedName = (sleepName.ToLower() == "mimikyu") ? "mimikyu-disguised" : sleepName.ToLower().Replace("_", "-")
                               join pokeData in pokePokemonData on formattedName equals pokeData.Name into pokes
                               from pokeData in pokes.DefaultIfEmpty()
                               select new PokemonData {
                                   Id = pokeData != null ? pokeData.Id : GetOriginalPokemonId(sleepName, pokePokemonData),
                                   Name = pokeData != null ? char.ToUpper(sleepName[0]) + sleepName.Substring(1).ToLower() : $"{char.ToUpper(sleepName[0]) + sleepName.Substring(1).ToLower()}+\n(Sleepオリジナルポケモン\nまたはフォルム違い)",
                                   JapaneseName = pokeData != null ? pokeData.JapaneseName : $"{additionalJapaneseNames[sleepName]}+\n(Sleepオリジナルポケモン\nまたはフォルム違い)",
                                   ImageUrl = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/pokemon/{sleepName.ToLower()}.png",
                                   ShinyImageUrl = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/pokemon/{sleepName.ToLower()}_shiny.png",
                                   BerryName = berryNameDict[sleepName].BerryName,
                                   BerryImageUrl = berryNameDict[sleepName].BerryImageUrl,
                                   TypeName = berryNameDict[sleepName].TypeName,
                                   Specialty = berryNameDict[sleepName].Specialty,
                                   IngredientAmountDisplayA = berryNameDict[sleepName].IngredientAmountA,
                                   IngredientImageUrlA = berryNameDict[sleepName].IngredientImageUrlA,
                                   IngredientAmountDisplayB = berryNameDict[sleepName].IngredientAmountB,
                                   IngredientImageUrlB = berryNameDict[sleepName].IngredientImageUrlB,
                                   IngredientAmountDisplayC = berryNameDict[sleepName].IngredientAmountC,
                                   IngredientImageUrlC = berryNameDict[sleepName].IngredientImageUrlC
                               };

            PokemonDataGrid.ItemsSource = combinedData
                .OrderBy(p => p.Id)
                .ThenBy(p => p.Name)
                .ToList();
        }

        private async Task<string> GetPokemonDataAsync(string apiUrl) {
            HttpResponseMessage response = await client.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
            string responseData = await response.Content.ReadAsStringAsync();
            return responseData;
        }

        private int GetOriginalPokemonId(string sleepName, List<Pokemon> pokePokemonData) {
            string originalName = sleepName.Split('_')[0].ToLower();
            var originalPokemon = pokePokemonData.FirstOrDefault(p => p.Name == originalName);
            return originalPokemon != null ? originalPokemon.Id : -1;
        }

        private async Task<string> GetJapaneseName(int id, string apiUrl) {
            string url = $"{apiUrl}/{id}";
            try {
                string jsonData = await GetPokemonDataAsync(url);
                var speciesData = JsonConvert.DeserializeObject<PokemonSpecies>(jsonData);
                var japaneseName = speciesData.Names.FirstOrDefault(n => n.Language.Name == "ja-Hrkt")?.Name;
                return japaneseName ?? "不明";
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return "不明";
            }
        }

        private async Task<string> GetBerryName(string pokemonName, string apiUrl) {
            string url = $"{apiUrl}{pokemonName.ToLower()}";
            try {
                string jsonData = await GetPokemonDataAsync(url);
                var pokemonData = JsonConvert.DeserializeObject<PokemonDetail>(jsonData);
                return pokemonData.Berry?.Name ?? "不明";
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return "不明";
            }
        }

        private async Task<string> GetJapaneseBerryName(string berryName, string apiUrl) {
            string url = $"{apiUrl}{berryName.ToLower()}";
            try {
                string jsonData = await GetPokemonDataAsync(url);
                var berryData = JsonConvert.DeserializeObject<BerryDetail>(jsonData);
                string itemUrl = berryData.Item.Url;
                jsonData = await GetPokemonDataAsync(itemUrl);
                var itemData = JsonConvert.DeserializeObject<ItemDetail>(jsonData);
                var japaneseName = itemData.Names.FirstOrDefault(n => n.Language.Name == "ja-Hrkt")?.Name;
                return japaneseName ?? "不明";
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return "不明";
            }
        }

        private async Task<string> GetTypeName(string pokemonName, string sleepApiUrl, string typeApiUrl) {
            string sleepUrl = $"{sleepApiUrl}{pokemonName.ToLower()}";
            try {
                string sleepJsonData = await GetPokemonDataAsync(sleepUrl);
                var pokemonData = JsonConvert.DeserializeObject<PokemonDetail>(sleepJsonData);
                string typeName = pokemonData.Berry?.Type?.ToLower() ?? "不明";
                string typeUrl = $"{typeApiUrl}{typeName}";
                string typeJsonData = await GetPokemonDataAsync(typeUrl);
                var typeData = JsonConvert.DeserializeObject<TypeDetail>(typeJsonData);
                var japaneseTypeName = typeData.Names.FirstOrDefault(n => n.Language.Name == "ja-Hrkt")?.Name;
                return japaneseTypeName ?? "不明";
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return "不明";
            }
        }

        private async Task<string> GetSpecialty(string pokemonName, string apiUrl) {
            string url = $"{apiUrl}{pokemonName.ToLower()}";
            try {
                string jsonData = await GetPokemonDataAsync(url);
                var pokemonData = JsonConvert.DeserializeObject<PokemonDetail>(jsonData);
                switch(pokemonData.Specialty) {
                    case "berry":
                        return "きのみ";
                    case "ingredient":
                        return "食材";
                    case "skill":
                        return "スキル";
                    default:
                        return "不明";
                }
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return "不明";
            }
        }

        private async Task<(string amountDisplayA, string imageUrlA,string amountDisplayB, string imageUrlB,string amountDisplayC, string imageUrlC)> GetIngredientData(string pokemonName, string apiUrl) {
            string url = $"{apiUrl}{pokemonName.ToLower()}";
            
            string imageUrlC;
            try {
                string jsonData = await GetPokemonDataAsync(url);
                var pokemonData = JsonConvert.DeserializeObject<PokemonDetail>(jsonData);




                string amountA1 = pokemonData.Ingredient0?.Amount.ToString() ?? "0";
                string amountA2 = pokemonData.Ingredient30?.FirstOrDefault()?.Amount.ToString() ?? "0";
                string amountA3 = pokemonData.Ingredient60?.FirstOrDefault()?.Amount.ToString() ?? "0";

                string amountDisplayA = $"1枠目:{amountA1}個\n2枠目:{amountA2}個\n3枠目:{amountA3}個";

                string ingredientNameA = pokemonData.Ingredient0?.ingredient?.Name.ToLower() ?? "unknown";
                string imageUrlA = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/ingredient/{ingredientNameA}.png";





                string ingredientNameB = pokemonData.Ingredient30 ?.FirstOrDefault(n=>n.ingredient.Name !=ingredientNameA)?.ingredient?.Name.ToLower() ?? "unknown";
                string imageUrlB;

                if(ingredientNameA == ingredientNameB1) {
                    imageUrlB = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/ingredient/{ingredientNameB2}.png";
                    string ingredientNameB = ingredientNameB2;
                    
                } else {
                    imageUrlB = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/ingredient/{ingredientNameB1}.png";
                    string ingredientNameB = ingredientNameB1;
                }
                
                
                
                
                string amountB1 = pokemonData.Ingredient30?.ElementAtOrDefault(1)?.Amount.ToString() ?? "null";

                string amountB2 = pokemonData.Ingredient60?.FirstOrDefault(n=>n.ingredient.Name == ) ?.Amount.ToString() ?? "null";

                string amountDisplayB = $"2枠目:{amountB1}個\n3枠目:{amountB2}個";
                







                string amountC = pokemonData.Ingredient60?.ElementAtOrDefault(2)?.Amount.ToString() ?? "null";

                string amountDisplayC = $"3枠目:{amountC}個";

                string ingredientNameC = pokemonData.Ingredient60?.ElementAtOrDefault(2)?.ingredient?.Name.ToLower() ?? "unknown";
                 imageUrlC = $"https://raw.githubusercontent.com/SleepAPI/SleepAPI/refs/heads/develop/frontend/public/images/ingredient/{ingredientNameA}.png";

                return (amountDisplayA,imageUrlA,amountDisplayB,imageUrlB,amountDisplayC,imageUrlC);
            } catch(HttpRequestException e) {
                Console.WriteLine($"Request error: {e.Message}");
                return ("食材情報無し","unknown","食材情報無し","unknown","食材情報無し","unknown");
            }
        }


        public List<string> ParseSleepPokemonData(string jsonData) {
            return JsonConvert.DeserializeObject<List<string>>(jsonData);
        }

        public List<Pokemon> ParsePokePokemonData(string jsonData) {
            var result = JsonConvert.DeserializeObject<PokeApiResponse>(jsonData);
            return result.Results.Select((p, index) => new Pokemon { Id = index + 1, Name = p.Name }).ToList();
        }
    }

    public class PokeApiResponse {
        public List<PokeApiResult> Results {
            get; set;
        }
    }

    public class PokeApiResult {
        public string Name {
            get; set;
        }
        public string Url {
            get; set;
        }
    }

    public class PokemonSpecies {
        public List<PokemonName> Names {
            get; set;
        }
    }

    public class PokemonName {
        public string Name {
            get; set;
        }
        public Language Language {
            get; set;
        }
    }

    public class Language {
        public string Name {
            get; set;
        }
    }

    public class PokemonDetail {
        public Berry Berry {
            get; set;
        }
        public string Specialty {
            get; set;
        }
        public Ingredient Ingredient0 {
            get; set;
        }
        public List<Ingredient> Ingredient30 {
            get; set;
        }
        public List<Ingredient> Ingredient60 {
            get; set;
        }
    }

    public class Berry {
        public string Name {
            get; set;
        }
        public string Type {
            get; set;
        }
    }

    public class BerryDetail {
        public Item Item {
            get; set;
        }
    }

    public class Item {
        public string Url {
            get; set;
        }
    }

    public class ItemDetail {
        public List<ItemName> Names {
            get; set;
        }
    }

    public class ItemName {
        public string Name {
            get; set;
        }
        public Language Language {
            get; set;
        }
    }

    public class TypeDetail {
        public List<TypeName> Names {
            get; set;
        }
    }

    public class TypeName {
        public string Name {
            get; set;
        }
        public Language Language {
            get; set;
        }
    }

    public class Ingredient {
        public int Amount {
            get; set;
        }
        public ingredient ingredient {
            get; set;
        }
    }

    public class ingredient {
        public string Name {
            get; set;
        }
    }
}

public class Pokemon {
    public int Id {
        get; set;
    }
    public string Name {
        get; set;
    }
    public string JapaneseName {
        get; set;
    }
    public string ImageUrl {
        get; set;
    }
    public string ShinyImageUrl {
        get; set;
    }
    public string BerryName {
        get; set;
    }
    public string BerryImageUrl {
        get; set;
    }
    public string TypeName {
        get; set;
    }
    public string Specialty {
        get; set;
    }
}

public class PokemonData {
    public int Id {
        get; set;
    }
    public string Name {
        get; set;
    }
    public string JapaneseName {
        get; set;
    }
    public string ImageUrl {
        get; set;
    }
    public string ShinyImageUrl {
        get; set;
    }
    public string BerryName {
        get; set;
    }
    public string BerryImageUrl {
        get; set;
    }
    public string TypeName {
        get; set;
    }
    public string Specialty {
        get; set;
    }
    public string IngredientAmountDisplayA {
        get; set;
    }
    public string IngredientImageUrlA {
        get; set;
    }
}
