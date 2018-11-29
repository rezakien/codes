using System.Collections.Generic;
using System.Windows.Forms;

namespace GameMontebank.Classes
{
    public delegate void Count();
    public delegate void CountBets();
    public delegate void ChangeTurn();
    public delegate void End();
    class Session
    {
        public event Count onCount;
        public event CountBets onRichCountBets;
        public event ChangeTurn ChangeT;
        public event End Notice;
        public List<Gamer> gamers = new List<Gamer>();
        public List<Bet> bets = new List<Bet>();
        public Deck deck = new Deck();                  //new
        
        public Croupier croupier { get; set; }
        public double SessionCashLimit { get; set; }
        public double MatchCashLimit { get; set; }
        public int Turn;
        public void AddGamers(Gamer gamer)
        {
            this.gamers.Add(gamer);
            if (gamers.Count >= 2)
                onCount();
        }
        public void AddBet(Bet bet)
        {
            this.bets.Add(bet);
            this.gamers[this.Turn].Cash -= bet.bet;
            this.Turn++;
            if (this.deck.CardList.Count <= 0)
                Notice();
            ChangeT();
            if (bets.Count == gamers.Count)
                onRichCountBets();
        }
        public Gamer CurrentPlayer()
        {
            if (this.Turn < gamers.Count)
                return gamers[this.Turn];
            else
            {
                this.Turn = 0;
                return gamers[this.Turn];
            }
        }
        public void ShowGamers()
        {
            string s = "";
            foreach (Gamer g in gamers)
            {
                s += "ID: " + g.ID.ToString() + "\n" + "Name: " + g.Name.ToString() + "\n" + "Cash: " + g.Cash.ToString() + "\n\n";
            }
            MessageBox.Show(s, "Gamers");
        }
    }
}
