using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zikken {
    // 性別を表す列挙型
    public enum GenderType {
        None,
        Men,
        Women
    }

    // DataGridに表示するデータ群
    public class Customer {
        //ユーザー名
        public string UserName {
            get; set;
        }
        //名前
        public string Name {
            get; set;
        }
        //性別
        public GenderType Gender {
            get; set;
        }
        //会員かどうか
        public bool IsMember {
            get; set;
        }
        //編集するかどうか
        public bool IsEdit {
            get; set;
        }
        public Customer(string username, string name, GenderType gender, bool isMember, bool isEdit) {
            UserName = username;
            Name = name;
            Gender = gender;
            IsMember = isMember;
            IsEdit = isEdit;
        }
    }
}
