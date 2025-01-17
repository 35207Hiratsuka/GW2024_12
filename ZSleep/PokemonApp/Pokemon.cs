using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonApp {
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
        } // きのみの名前を追加
        public string BerryImageUrl {
            get; set;
        } // きのみの画像URLを追加
        public string TypeName {
            get; set;
        } // タイプの日本語名を追加
        public string Specialty {
            get; set;
        } // 得意を追加
        public string IngredientAmountDisplayA {
            get; set;
        } // 食材Aの表示を追加
        public string IngredientImageUrlA {
            get; set;
        } // 食材Aの画像URLを追加
        public string IngredientAmountDisplayB {
            get; set;
        } // 食材Bの量を追加
        public string IngredientImageUrlB {
            get; set;
        } // 食材Bの画像URLを追加
        public string IngredientAmountDisplayC {
            get; set;
        } // 食材Cの量を追加
        public string IngredientImageUrlC {
            get; set;
        } // 食材Cの画像URLを追加
    }
}
