using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zikken {
    public class Item {
        public string Name {
            get; set;
        }
        public string Content {
            get; set;
        }
    }

    public class ItemsContainer {
        public string Name {
            get; set;
        }
        public List<Item> ItemList {
            get; set;
        }
        public int CurrentItemIndex {
            get; set;
        }



#region ContainerList変更通知プロパティ
private List<ItemsContainer> _ContainerList;

        public List<ItemsContainer> ContainerList {
            get {
                return _ContainerList;
            }
            set {
                if(_ContainerList == value)
                    return;
                _ContainerList = value;
            }
        }
        #endregion

        #region CurrentContainerIndex変更通知プロパティ
        private int _CurrentContainerIndex;

        public int CurrentContainerIndex {
            get {
                return _CurrentContainerIndex;
            }
            set {
                if(_CurrentContainerIndex == value)
                    return;
                _CurrentContainerIndex = value;
            }
        }
        #endregion


        public void Initialize() {
            var containerList = new List<ItemsContainer>();

            for(int i = 0; i < 2; ++i) {
                var container = new ItemsContainer() {
                    Name = "a" + i.ToString(),
                    ItemList = new List<Item>(),
                    CurrentItemIndex = 0,
                };

                for(int j = 0; j < 3; ++j) {
                    var item = new Item() {
                        Name = "b" + j.ToString(),
                        Content = "item content" + j.ToString(),
                    };
                    container.ItemList.Add(item);
                }

                containerList.Add(container);
            }

            this.ContainerList = containerList;
        }
    }
}
