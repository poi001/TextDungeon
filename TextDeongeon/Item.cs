using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextDeongeon
{
    internal class Item
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _status;
        public int status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _description;
        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _statText;
        public string statText
        {
            get { return _statText; }
            set { _statText = value; }
        }

        //public bool IsEquiped = false;

        public bool IsWeapon = true;
        public int cost;
    }
}
