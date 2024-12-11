using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PokemonGoApp {
    public class Pokemon {
        public int DexNr {
            get; set;
        }
        public string JapaneseName {
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
    }

    public class Names {
        public string Japanese {
            get; set;
        }
    }
}



