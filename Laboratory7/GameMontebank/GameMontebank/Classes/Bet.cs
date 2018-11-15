using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMontebank.Classes
{
    class Bet
    {
        public Gamer Gamer;
        public double bet { get; set; }
        public string BetType { get; set; }
        public bool Win { get; set; }
        public Bet(Gamer Gamer, double Bet, string BetType)
        {
            this.Gamer = Gamer;
            this.bet = Bet;
            this.BetType = BetType;
        }
    }
}
