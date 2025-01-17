//Pokemon.cs
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using static PokemonGoApp.MainWindow;

namespace PokemonGoApp {
    public class Pokemon {
        public int DexNr {
            get; set;
        }
        public string JapaneseName {
            get; set;
        }
        public string EnglishName {
            get; set;
        }
        public string Types {
            get; set;
        }
        public string Stats {
            get; set;
        }
        public BitmapImage iconImage {
            get; set;
        }
        public BitmapImage iconShinyImage {
            get; set;
        }
        public string NormalAttack {
            get; set;
        }
        public string EliteNormalAttack {
            get; set;
        }
        public string SpecialAttack {
            get; set;
        }
        public string EliteSpecialAttack {
            get; set;
        }
    }


    public class PokemonData {
        public string Id {
            get; set;
        }
        public string FormId {
            get; set;
        }
        public int DexNr {
            get; set;
        }
        public int Generation {
            get; set;
        }
        public Names Names {
            get; set;
        }
        public Type PrimaryType {
            get; set;
        }
        public Type SecondaryType {
            get; set;
        }
        public Stats Stats {
            get; set;
        }
        public Assets Assets {
            get; set;
        }
        public Dictionary<string, Move> QuickMoves {
            get; set;
        }
        [JsonConverter(typeof(EliteMovesConverter))]
        public Dictionary<string, Move> EliteQuickMoves {
            get; set;
        }
        public Dictionary<string, Move> CinematicMoves {
            get; set;
        } // 追加
        [JsonConverter(typeof(EliteMovesConverter))] // CinematicMovesと同じ形式で変換
        public Dictionary<string, Move> EliteCinematicMoves {
            get; set;
        } // 追加
    }


    public class Names {
        public string Japanese {
            get; set;
        }
        public string English {
            get; set;
        }
    }

    public class Type {
        public Names Names {
            get; set;
        }
    }

    public class Stats {
        public int Stamina {
            get; set;
        }
        public int Attack {
            get; set;
        }
        public int Defense {
            get; set;
        }
    }

    public class Assets {
        public string Image {
            get; set;
        }
        public string ShinyImage {
            get; set;
        }
    }

    public class Move {
        public string Id {
            get; set;
        }
        public int Power {
            get; set;
        }
        public int Energy {
            get; set;
        }
        public int DurationMs {
            get; set;
        }
        public Type Type {
            get; set;
        }
        public Names Names {
            get; set;
        }
    }
}
