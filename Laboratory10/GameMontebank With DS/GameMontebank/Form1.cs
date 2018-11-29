using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace GameMontebank
{
    public partial class Game : Form
    {
        //static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\MonteBank.mdf;Integrated Security=True;";
        string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Reza\source\repos\ПИС\5 Семестр\GameMontebank With DS\GameMontebank\MonteBank.mdf;Integrated Security=True;Connect Timeout=30";
        SqlConnection sql = new SqlConnection();
        
        
        //Элементы для добавления игроков

        TextBox Namee;
        TextBox Cash;
        Label NameLabel;
        Label RuleGamers;
        Label CashLabel;
        Button Start;

        //Элементы для  самого процесса игры
        PictureBox[] Cards = new PictureBox[4];
        PictureBox gateCards;
        
        //
        Button betUp;
        Button betDown;
        Button betBoth;
        
        //
        Label labelBetLabel;
        TextBox labelBetTextBox;
        Button ChangeBetButton;
        
        //
        Point point;
        
        //
        Label labelNamePlayer;
        Label labelCashPlayer;
        
        //
        Label labelNameCroupier;
        Label labelCashCroupier;

        //
        List<Classes.Card> Temp = new List<Classes.Card>();
        List<Classes.Card> CardList = new List<Classes.Card>();
        List<Classes.Gamer> gamers = new List<Classes.Gamer>();
        Classes.Session s = new Classes.Session();
        Classes.Deck deck;
        Classes.Bet b;

        double bet;
        bool cardsAreDistributed = false;
        public Game()
        {
            InitializeComponent();
            //если игроков 2 и больше, то кнопка старт активируется
            s.onCount += Count;
            //если все игроки поставили, то кнопка сдающий раздаст карты и покажет карту ворот
            s.onRichCountBets += DistrCards;
            s.onRichCountBets += DistrGateCard;
            s.ChangeT += ChangeTurn;
            s.Notice += NoticeGamers;
            sql.ConnectionString = connString;
        }
        public void NoticeGamers()
        {
            MessageBox.Show("Карт не осталось! Игра закончилась. Сейчас игра закроется.");
            this.Dispose();
        }
        private Image returnImage(string mast, int n)
        {
            string path = "../assets/images/cards/";
            if (mast == "Черви") { return Image.FromFile(path + n + "H.png"); }
            if (mast == "Бубен") { return Image.FromFile(path + n + "S.png"); }
            if (mast == "Крести") { return Image.FromFile(path + n + "C.png"); }
            if (mast == "Пики") { return Image.FromFile(path + n + "D.png"); }
            else { return null; }
        }
        private void Count()
        {
            RuleGamers.Visible = false;
            Start.Enabled = true;
        }

        public void HideControls()
        {
            foreach (Control c in Game.ActiveForm.Controls)
            {
                if (c != button2)
                    c.Visible = false;
            }
        }
        public void ShowControlsForAdding()
        {
            button1.Visible = true;

            //Cash textbox options
            Cash = new TextBox();
            Cash.Location = new Point(budjet.Location.X, budjet.Location.Y);
            Cash.Size = budjet.Size;
            Cash.Font = budjet.Font;
            this.Controls.Add(Cash);
            Cash.TextChanged += Cash_TextChanged;

            //CashLabel label options
            CashLabel = new Label();
            CashLabel.Location = new Point(budjet.Location.X - 150, budjet.Location.Y);
            CashLabel.Font = label1.Font;
            CashLabel.Text = "Cash: ";
            CashLabel.Size = label1.Size;
            this.Controls.Add(CashLabel);

            //Name textbox options
            Namee = new TextBox();
            Namee.Location = new Point(budjet.Location.X, budjet.Location.Y - 80);
            Namee.Size = budjet.Size;
            Namee.Font = budjet.Font;
            this.Controls.Add(Namee);
            Namee.TextChanged += Namee_TextChanged;
            //Name label options
            NameLabel = new Label();
            NameLabel.Location = new Point(budjet.Location.X - 150, budjet.Location.Y - 80);
            NameLabel.Font = label1.Font;
            NameLabel.Size = label1.Size;
            NameLabel.Text = "Name: ";
            this.Controls.Add(NameLabel);


            //RuleGamers label options
            RuleGamers = new Label();
            RuleGamers.Location = new Point(budjet.Location.X / 2, budjet.Location.Y + 140);
            RuleGamers.Font = new Font("Lucida Sans", 8F);
            RuleGamers.Size = label1.Size;
            RuleGamers.Text = "Игроков должно быть больше 2";
            this.Controls.Add(RuleGamers);


            //Submit button options
            button1.Text = "Add";
            button1.Location = new Point(button1.Location.X - 150, button1.Location.Y);

            //start button options
            Start = new Button();
            Start.Enabled = false;
            Start.Location = new Point(button1.Location.X + 200, button1.Location.Y);
            Start.Font = button1.Font;
            Start.FlatStyle = button1.FlatStyle;
            Start.Size = button1.Size;
            Start.Text = "Start";
            this.Controls.Add(Start);
            Start.Click += Start_Click;
        }

        private void Namee_TextChanged(object sender, EventArgs e)
        {
            string name = Namee.Text;
            if (Regex.IsMatch(name, @"[^a-zA-Z ]"))
            {
                MessageBox.Show("Имя должно хранить только английский алфавит!");
                Namee.Text = "";
            }
        }

        private void Cash_TextChanged(object sender, EventArgs e)
        {
            int count = 0;
            Regex r = new Regex(@"\d$");
            if (!r.IsMatch(Cash.Text))
            {
                count++;
                Cash.Text = "";
            }
            if (count > 0)
            {
                MessageBox.Show("Вводите число!");
            }
            count = 0;
        }

        public void Start_THE_GAME()
        {
            //Выбор крупье - сдающий. В данном случает он первый игрок 
            s.croupier = new Classes.Croupier();
            s.croupier.Name = s.gamers.First().Name;
            s.croupier.LostBets = s.gamers.First().Cash;
            s.gamers.Remove(s.gamers.First());
            
            //заполнение колоды карт и растасовка
            s.deck = new Classes.Deck();
            s.deck.FillDeck();
            s.deck.Shuffle(s.deck.CardList);
            deck = s.deck;

        }
        public void ChangeTurn()
        {
            labelNamePlayer.Text = "Игрок: " + s.CurrentPlayer().Name;
        }
        public void ShowGameControls()
        {
            Start_THE_GAME();
            TakePlace();
            ShowCards();
            ShowInputs();
        }

        public void CheckWinners()
        {
            double sum = 0;
            foreach (Classes.Bet i in s.bets)
            {
                sum += i.bet;

                if (i.BetType == "Верх" && ((Temp[0].Mast == Temp.Last().Mast) || (Temp[1].Mast == Temp.Last().Mast)))
                {
                    i.Win = true;
                }
                if (i.BetType == "Низ" && ((Temp[2].Mast == Temp.Last().Mast) || (Temp[3].Mast == Temp.Last().Mast)))
                {
                    i.Win = true;
                }
                if (i.BetType == "Оба" && ((Temp[0].Mast == Temp.Last().Mast || Temp[1].Mast == Temp.Last().Mast)) && (Temp[2].Mast == Temp.Last().Mast || Temp[3].Mast == Temp.Last().Mast))
                {
                    i.Win = true;
                }
            }
            int k = 0;
            foreach (var i in s.bets)
            {
                if (i.Win)
                {
                    k++;
                }
            }
            double res = 0;
            if (k != 0) { res = sum / k; }
            else { res = sum; }


            foreach (var i in s.bets)
            {
                if (i.Win)
                {
                    sql.Open();
                    i.Gamer.Cash += res;
                    int id_player = i.Gamer.ID;
                    int result = UpdateAccount(id_player, res);
                    sql.Close();
                    sql.Close();
                }
                else
                {
                    int id_player = i.Gamer.ID;
                    sql.Open();
                    int result = UpdateAccount(id_player, -res);
                    sql.Close();
                }
            }
            if (k == 0) {
                sql.Open();
                int id_player = int.Parse(SelectPlayer(s.croupier.Name).ToString());
                UpdateAccount(id_player, res);
                s.croupier.LostBets += res;
                sql.Close();
            }
        }
        public void DistrCards()
        {
            point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
            int h = 0;
            foreach (var i in s.deck.CardList)
            {
                Temp.Add(i);
                if (h == 3)
                    break;
                h++;
            }
            CardList = s.croupier.DistributeCards(s.deck);
            int j = 0;
            foreach(Classes.Card c in CardList)
            {
                if (j == 2 || j == 3)
                    point.Y += Cards[0].Size.Height + 20;
                if (j == 1 || j == 3)
                    point.X += Cards[0].Size.Width + 20;
                string name = c.Number + c.Mast + ".png";
                Cards[j].Image = Image.FromFile("../assets/images/cards/"+ name);
                point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
                j++;
            }
            s.Turn = 0;
            CardList.Clear();
        }
        public void DistrGateCard()
        {
            string name = s.deck.CardList[0].Number + s.deck.CardList[0].Mast + ".png";
            gateCards.Image = Image.FromFile("../assets/images/cards/" + name);
            Temp.Add(s.deck.CardList.First());
            s.deck.MinusCard(s.deck.CardList.First());
            cardsAreDistributed = true;
            betUp.Enabled = false;
            betDown.Enabled = false;
            betBoth.Enabled = false;
            ChangeBetButton.Enabled = true;
            CheckWinners();
            s.bets.Clear();
            Temp.Clear();
            labelCashPlayer.Text = "Счет: " + s.CurrentPlayer().Cash.ToString();
            labelCashCroupier.Text = "Счет: " + s.croupier.LostBets;
        }
        public void TakePlace()
        {
            point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
            int j = 0;
            for (j = 0; j < 4; j++)
            {
                if (j == 2 || j == 3)
                    point.Y += Cards[0].Size.Height + 20;
                if (j == 1 || j == 3)
                    point.X += Cards[0].Size.Width + 20;
                Cards[j] = new PictureBox();
                Cards[j].BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
                Cards[j].Image = null;
                Cards[j].Name = "Layout";
                Cards[j].Location = point;
                Cards[j].Size = new Size(80, 150);
                Cards[j].SizeMode = PictureBoxSizeMode.Zoom;
                this.Controls.Add(Cards[j]);
                point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
            }
            point = new Point(this.Width / 2 - 180, this.Height / 2 - 85);
            gateCards = new PictureBox();
            gateCards.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            gateCards.Image = null;
            gateCards.Location = point;
            gateCards.Size = new Size(80, 150);
            gateCards.SizeMode = PictureBoxSizeMode.Zoom;
            this.Controls.Add(gateCards);
        }
        public void ShowCards()
        {
            point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
            
            int j = 0;
            for (j = 0; j < 4; j++)
            {
                if (j==2||j==3)
                    point.Y += Cards[0].Size.Height + 20;
                if(j==1||j==3)
                    point.X += Cards[0].Size.Width + 20;
                Cards[j].Image = Image.FromFile("../assets/gray.png");
                this.Controls.Add(Cards[j]);
                point = new Point(this.Width / 2 - 90, this.Height / 2 - 160);
            }
            point = new Point(this.Width / 2 - 180, this.Height / 2 - 85);
            gateCards.Image = Image.FromFile("../assets/red.png");
            gateCards.Location = point;
            this.Controls.Add(gateCards);
        }
        public void ShowInputs()
        {
            labelNamePlayer = new Label();
            labelCashPlayer = new Label();
            labelBetLabel = new Label();
            labelBetTextBox = new TextBox();
            ChangeBetButton = new Button();

            labelNameCroupier = new Label();
            labelCashCroupier = new Label();
            
            
            betUp = new Button();
            betDown = new Button();
            betBoth = new Button();

            int marginY = 40;
            int marginX = 20;
            labelNamePlayer.Text = "Игрок: " + s.CurrentPlayer().Name;
            labelNamePlayer.Location = new Point(marginX, marginY);
            labelNamePlayer.ForeColor = Color.White;
            labelNamePlayer.Font = new Font("Lucida Sans", 14F);
            labelNamePlayer.Size = new Size(200, 40);

            labelCashPlayer.Text = "Счет: " + s.CurrentPlayer().Cash.ToString();
            labelCashPlayer.Location = new Point(marginX, labelNamePlayer.Location.Y + marginY);
            labelCashPlayer.ForeColor = Color.White;
            labelCashPlayer.Font = new Font("Lucida Sans", 14F);
            labelCashPlayer.Size = new Size(200, 40);

            labelBetLabel.Text = "Сделайте ставку";
            labelBetLabel.Location = new Point(marginX, labelCashPlayer.Location.Y + marginY);
            labelBetLabel.ForeColor = Color.White;
            labelBetLabel.Font = new Font("Lucida Sans", 14F);
            labelBetLabel.Size = new Size(200, 40);

            labelBetTextBox.Text = "";
            labelBetTextBox.Location = new Point(marginX, labelBetLabel.Location.Y + marginY);
            labelBetTextBox.Font = new Font("Lucida Sans", 14F);
            labelBetTextBox.Size = new Size(140, 40);

            ChangeBetButton.Text = "Изменить ставку";
            ChangeBetButton.Location = new Point(marginX, labelBetLabel.Location.Y + marginY + 40);
            ChangeBetButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            ChangeBetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            ChangeBetButton.Font = new System.Drawing.Font("Lucida Sans", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            ChangeBetButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            ChangeBetButton.Size = new System.Drawing.Size(150, 65);
            ChangeBetButton.Click += ChangeBetButton_Click;


            betUp.Text = "Верхний";
            betUp.Location = new Point(marginX, ChangeBetButton.Location.Y + marginY + 40);
            betUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            betUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            betUp.Font = new System.Drawing.Font("Lucida Sans", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            betUp.ForeColor = System.Drawing.SystemColors.ButtonFace;
            betUp.Size = new System.Drawing.Size(150, 65);
            betUp.Enabled = false;
            betUp.Click += BetUp_Click;

            betDown.Text = "Нижний";
            betDown.Location = new Point(marginX, betUp.Location.Y + marginY + 40);
            betDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            betDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            betDown.Font = new System.Drawing.Font("Lucida Sans", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            betDown.ForeColor = System.Drawing.SystemColors.ButtonFace;
            betDown.Size = new System.Drawing.Size(150, 65);
            betDown.Enabled = false;
            betDown.Click += BetDown_Click;

            betBoth.Text = "Оба";
            betBoth.Location = new Point(marginX, betDown.Location.Y + marginY + 30);
            betBoth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            betBoth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            betBoth.Font = new System.Drawing.Font("Lucida Sans", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            betBoth.ForeColor = System.Drawing.SystemColors.ButtonFace;
            betBoth.Size = new System.Drawing.Size(150, 65);
            betBoth.Enabled = false;
            betBoth.Click += BetBoth_Click;


            labelNameCroupier.Text = "Сдающий: " + s.croupier.Name.ToString();
            labelNameCroupier.Location = new Point(this.Width - marginX-200, marginY);
            labelNameCroupier.ForeColor = Color.White;
            labelNameCroupier.Font = new Font("Lucida Sans", 14F);
            labelNameCroupier.Size = new Size(200, 40);

            labelCashCroupier.Text = "Счет: " + s.croupier.LostBets.ToString();
            labelCashCroupier.Location = new Point(labelNameCroupier.Location.X, labelNameCroupier.Location.Y + marginY);
            labelCashCroupier.ForeColor = Color.White;
            labelCashCroupier.Font = new Font("Lucida Sans", 14F);
            labelCashCroupier.Size = new Size(200, 40);


            this.Controls.Add(labelNamePlayer);
            this.Controls.Add(labelCashPlayer);
            this.Controls.Add(labelBetLabel);
            this.Controls.Add(labelBetTextBox);
            this.Controls.Add(ChangeBetButton);

            this.Controls.Add(betUp);
            this.Controls.Add(betDown);
            this.Controls.Add(betBoth);

            this.Controls.Add(labelNameCroupier);
            this.Controls.Add(labelCashCroupier);

        }

        private void ChangeBetButton_Click(object sender, EventArgs e)
        {
            bet = double.Parse(labelBetTextBox.Text);
            if(cardsAreDistributed)
                ShowCards();
            betUp.Enabled = true;
            betDown.Enabled = true;
            betBoth.Enabled = true;
            ChangeBetButton.Enabled = false;
        }

        private void BetUp_Click(object sender, EventArgs e)
        {
            b = new Classes.Bet(s.CurrentPlayer(), bet, "Верх");
            s.AddBet(b);
        }
        private void BetDown_Click(object sender, EventArgs e)
        {
            b = new Classes.Bet(s.CurrentPlayer(), double.Parse(labelBetTextBox.Text), "Низ");
            s.AddBet(b);
        }
        private void BetBoth_Click(object sender, EventArgs e)
        {
            b = new Classes.Bet(s.CurrentPlayer(), double.Parse(labelBetTextBox.Text), "Оба");
            s.AddBet(b);
        }
        private void Start_Click(object sender, EventArgs e)
        {
            Game.ActiveForm.BackColor = ColorTranslator.FromHtml("#009900");
            button1.Visible = false;
            HideControls();
            ShowGameControls();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            s.gamers = gamers;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //Выбирает игрока(проверка: существует ли такой игрок)
        private object SelectPlayer(string name)
        {
            string sqlExpression = "Select_Player";
            
                    
                    SqlCommand command = new SqlCommand(sqlExpression, sql);
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter nameParam = new SqlParameter
                    {
                        ParameterName = "@name",
                        Value = name
                    };
                    command.Parameters.Add(nameParam);
                    if (command.ExecuteScalar() == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return command.ExecuteScalar();
                    }
            
        }
        //Добавление Нового игрока
        private int AddPlayer(string name)
        {
            string sqlExpression = "Insert_Player";
                SqlCommand command = new SqlCommand(sqlExpression, sql);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlParameter nameParam = new SqlParameter
                {
                    ParameterName = "@name",
                    Value = name
                };
                command.Parameters.Add(nameParam);
                int result = int.Parse(command.ExecuteScalar().ToString());
                return result;
        }
        //Добавление счета для Нового игрока
        private int AddAccount(int Player_ID, double All_Cash)
        {
            string sqlExpression = "Insert_Account";
            SqlCommand command = new SqlCommand(sqlExpression, sql);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter Player_IDParam = new SqlParameter
            {
                ParameterName = "@Player_ID",
                Value = Player_ID
            };
            command.Parameters.Add(Player_IDParam);

            SqlParameter All_CashParam = new SqlParameter
            {
                ParameterName = "@All_Cash",
                Value = All_Cash
            };
            command.Parameters.Add(All_CashParam);
            int result = int.Parse(command.ExecuteScalar().ToString());
            return result;
        }
        private int UpdateAccount(int Player_ID, double All_Cash)
        {
            string sqlExpression = "Update_Account";
            SqlCommand command = new SqlCommand(sqlExpression, sql);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter Player_IDParam = new SqlParameter
            {
                ParameterName = "@Player_ID",
                Value = Player_ID
            };
            command.Parameters.Add(Player_IDParam);

            SqlParameter All_CashParam = new SqlParameter
            {
                ParameterName = "@All_Cash",
                Value = All_Cash
            };
            command.Parameters.Add(All_CashParam);
            int result = command.ExecuteNonQuery();
            return result;
        }
        private DataTable GetUsers()
        {
            sql.Open();
            string sqlExpression = "Select_All_Users";
            SqlCommand command = new SqlCommand(sqlExpression, sql);
            
            command.CommandType = System.Data.CommandType.StoredProcedure;
            DataTable myTable = new DataTable();
            myTable.Load(command.ExecuteReader());
            sql.Close();
            return myTable;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            switch (button1.Text.ToString())
            {
                case "Begin":
                    if (budjet.Text != "")
                    {
                        s.SessionCashLimit = double.Parse(budjet.Text);
                        HideControls();
                        ShowControlsForAdding();
                        
                    }
                    break;
                case "Add":
                    string name = Namee.Text;
                    double cash = double.Parse(Cash.Text);
                    sql.Open();
                    var queryExecute = SelectPlayer(name);
                    if (int.Parse(queryExecute.ToString()) == 0)
                    {
                        if (cash>s.SessionCashLimit/3)
                        {
                            int Player_ID = AddPlayer(name); // ID of Player
                            int Account_ID = AddAccount(Player_ID, cash); // ID of Account

                            Classes.Gamer gamer = new Classes.Gamer(Player_ID, Namee.Text, cash);
                            s.AddGamers(gamer);
                            MessageBox.Show("Добро пожаловать в игру Монтебанк для частного круга!, {0}", name);
                        }
                        else
                        {
                            MessageBox.Show("У вас не хватает денег. Сумма денег должна " +
                                "быть больше трети от общей суммы допустимого бюджета игры.");
                        }
                    }
                    else
                    {
                        int Select_Player = int.Parse(SelectPlayer(name).ToString());
                        Classes.Gamer gamer = new Classes.Gamer(Select_Player, Namee.Text, cash);
                        s.AddGamers(gamer);
                        MessageBox.Show("Добро пожаловать, {0}!",name);
                    }
                    Namee.Clear();
                    Cash.Clear();
                    sql.Close();
                    break;
                case "Start":
                    HideControls();
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.StartPosition = this.StartPosition;
            f.Size = new Size(400, 200);
            DataGridView dt = new DataGridView();
            dt.ClientSize = this.Size;
            f.Text = "Records Table";
            dt.DataSource = GetUsers();
            f.Controls.Add(dt);
            f.ShowDialog();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int count = 0;
            Regex r = new Regex(@"\d$");
            if (!r.IsMatch(budjet.Text))
            {
                count++;
                budjet.Text = "";
            }
            if (count > 0)
            {
                MessageBox.Show("Введите число!");
            }
            count = 0;
        }
    }
}
