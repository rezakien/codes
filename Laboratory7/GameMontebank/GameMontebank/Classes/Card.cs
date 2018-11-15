using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMontebank.Classes
{
    class Card
    {
        public int Number { get; set; }
        public string Mast { get; set; }
        public string Image { get; set; }
        public Card(int number, string mast, string image)
        {
            this.Number = number;
            this.Mast = mast;
            this.Image = image;
        }
    }
}
