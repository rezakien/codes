using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace GameMontebank.Classes
{
    class Deck
    {
        public List<Card> CardList = new List<Card>();
        public bool MinusCard(Card card)
        {
            if(this.CardList.Remove(card))
                return true;
            return false;
            
        }
        public void Shuffle(List<Card> cardlist)
        {
            cardlist = this.CardList;
            Random rand = new Random();
            cardlist = CardList.OrderBy(x => rand.Next()).ToList();
            this.CardList = cardlist;
        }
        public void ShowCards()
        {
            int count = 0;
            string n = "";
            foreach (var i in this.CardList)
            {
                n += "Номер: " + i.Number + "\tМасть: " + i.Mast + "\n";
                count++;
            }
            MessageBox.Show(n + "\nCount: " + count);
        }
        public void FillDeck()
        {
            int i = 1;
            int j = 2;
            char[] mastiB = { 'H', 'S', 'C', 'D' };
            string[] mastiS = { "Черви", "Бубен", "Крести", "Пики" };
            while (i <= 40)
            {
                if (j != 10 && j != 9 && j != 8)
                {
                    for(int k =0; k<mastiB.Length; k++)
                    {

                        this.CardList.Add(new Card(j, mastiB[k].ToString(), j.ToString() + mastiB[k]));
                    }
                    i += 4;
                }
                j++;
            }
        }
    }
}
