using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMontebank.Classes
{
    class Gamer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Cash { get; set; }
        public Gamer(int id,string name, double cash)
        {
            this.ID = id;
            this.Name = name;
            this.Cash = cash;
        }
        public bool DoBet(double bet, string betType) {
            if (bet <= 0)
            {
                Bet Bet = new Bet(this, bet, betType);
                return true;
            }
            else
                return false;
        }
    }
}
