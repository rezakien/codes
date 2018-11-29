using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameMontebank.Classes
{
    class Croupier
    {
        public string Name { get; set; }
        public double LostBets { get; set; }
        public List<Card> DistributeCards(Deck deck)
        {
            int cardsCount = 0;
            List<Card> CardList = new List<Card>();
            while (cardsCount < 4)
            {
                Card card = deck.CardList.First();
                CardList.Add(card);
                deck.MinusCard(card);
                cardsCount++;
            }
            return CardList;

        }
        public Card ShowGateCard(Deck deck)
        {
            Card card = deck.CardList.First();
            deck.MinusCard(card);
            return card;
        }
        public void TakeLostMoney(List<Bet> lostBet)
        {
            foreach (Bet bet in lostBet)
                LostBets += bet.bet;
        }
    }
}
