using System.Windows.Media.Imaging;

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
}
