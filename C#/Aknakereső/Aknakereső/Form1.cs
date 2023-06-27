using System;
using System.Drawing;
using System.Windows.Forms;

namespace Aknakereső
{
    public partial class Form1 : Form
    {
        static Negyzet[,] tabla;
        static int meret = 10;
        static int bombadb = 9;
        static int talalt = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Epites();
        }

        private void revealText(int a, int b)
        {
            if (a < 0 || b < 0 || a >= tabla.GetLength(0) || b >= tabla.GetLength(1))
                return;

            if (tabla[a, b].getButtonText().CompareTo("") != 0)
                return;

            tabla[a, b].setText(tabla[a, b].getText());

            if (tabla[a, b].getText().CompareTo("0") == 0)
            {
                revealText(a - 1, b);
                revealText(a - 1, b - 1);
                revealText(a - 1, b + 1);
                revealText(a, b - 1);
                revealText(a, b + 1);
                revealText(a + 1, b - 1);
                revealText(a + 1, b);
                revealText(a + 1, b + 1);
            }
        }

        private void onClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                string[] name = (sender as Button).Name.Split('_');

                int a = int.Parse(name[1]);
                int b = int.Parse(name[2]);

                if (!tabla[a, b].bomba())
                {
                    revealText(a, b);
                }
                else
                {
                    Vesztes();
                }
            }
            else
            {
                string[] name = (sender as Button).Name.Split('_');

                int a = int.Parse(name[1]);
                int b = int.Parse(name[2]);

                if (tabla[a,b].getButtonText().CompareTo("") == 0 && int.Parse(txt_marad.Text) > 0)
                {
                    tabla[a, b].setText("P");
                    int db = int.Parse(txt_marad.Text);
                    db--;
                    txt_marad.Text = db.ToString();
                    if (tabla[a,b].bomba())
                    {
                        talalt++;
                    }
                }
                else if(tabla[a, b].getButtonText().CompareTo("P") == 0)
                {
                    tabla[a, b].setText("");
                    int db = int.Parse(txt_marad.Text);
                    db++;
                    txt_marad.Text = db.ToString();
                    if (tabla[a, b].bomba())
                    {
                        talalt--;
                    }
                }

                if (talalt == bombadb)
                {
                    Nyertes();
                }
            }
        }

        private void Nyertes()
        {
            p_game.Controls.Clear();
            Label s = new Label();
            s.Location = new Point(p_game.Width / 2, p_game.Height / 2);
            s.Visible = true;
            s.Size = new Size(p_game.Width, p_game.Height);
            s.Text = "Ön Nyert! :)";
            p_game.Controls.Add(s);
        }

        private void Vesztes()
        {
            p_game.Controls.Clear();
            Label s = new Label();
            s.Location = new Point(p_game.Width / 2, p_game.Height / 2);
            s.Visible = true;
            s.Size = new Size(p_game.Width, p_game.Height);
            s.Text = "Ön vesztett! :(";
            p_game.Controls.Add(s);
        }

        private void Epites()
        {
            p_game.Controls.Clear();
            talalt = 0;

            tabla = new Negyzet[meret, meret];
            Random rnd = new Random();
            double cel = 90;
            int db = 0;

            int width = p_game.Width / meret;
            int height = p_game.Height / meret;

            for (int i = 0; i < meret; i++)
            {
                for (int j = 0; j < meret; j++)
                {
                    Button s = new Button();
                    s.Size = new Size(width, height);
                    s.Location = new Point(i * (width), j * (height));
                    s.Visible = true;
                    p_game.Controls.Add(s);
                    s.Name = "b_" + i + "_" + j;
                    s.MouseDown += new MouseEventHandler(onClick);

                    if (rnd.Next(0,100) > cel && db < bombadb)
                    {
                        db++;
                        tabla[i, j] = new Negyzet(s, "b", 0);
                    }
                    else
                    {
                        tabla[i, j] = new Negyzet(s, "u", 0);
                    }                   
                }
            }

            int a = 0; int b = 0;

            while (db < bombadb)
            {
                int s = rnd.Next(0, 100);
                if (s > cel && !tabla[a,b].bomba())
                {
                    tabla[a, b].changeType("b");
                    db++;
                }

                a++;

                if (a >= meret)
                {
                    a = 0; b++;

                    if (b >= meret)
                    {
                        b = 0;
                    }
                }
            }

            db = 0;

            for (int i = 0; i < meret; i++)
            {
                for (int j = 0; j < meret; j++)
                {
                    if (i - 1 >= 0 && j - 1 >= 0 && tabla[i - 1,j - 1].bomba())
                    {
                        db++;
                    }
                    if (i - 1 >= 0 && tabla[i - 1, j].bomba())
                    {
                        db++;
                    }
                    if (i - 1 >= 0 && j + 1 < meret && tabla[i - 1, j + 1].bomba())
                    {
                        db++;
                    }
                    if (j + 1 < meret && tabla[i, j + 1].bomba())
                    {
                        db++;
                    }
                    if (i + 1 < meret && j + 1 < meret && tabla[i + 1, j + 1].bomba())
                    {
                        db++;
                    }
                    if (i + 1 < meret && tabla[i + 1, j].bomba())
                    {
                        db++;
                    }
                    if (i + 1 < meret && j - 1 >= 0 && tabla[i + 1, j - 1].bomba())
                    {
                        db++;
                    }
                    if (j - 1 >= 0 && tabla[i, j - 1].bomba())
                    {
                        db++;
                    }

                    tabla[i, j].dataGather(db);
                    db = 0;

                    txt_marad.Text = "9";
                }
            }
        }

        private void Btn_kever_Click(object sender, EventArgs e)
        {
            Epites();
        }
    }

    struct Negyzet
    {
        private Button gomb;
        private string type;
        private int data;

        public Negyzet(Button b, string t, int d)
        {
            gomb = b;
            type = t;
            data = d;
        }

        public string getText()
        {
            if (data == 0) return "0";
            else return data.ToString();
        }

        public string getButtonText()
        {
            return gomb.Text;
        }

        public bool ures()
        {
            return data == 0;
        }

        public bool bomba()
        {
            return String.Compare(type, "b", StringComparison.Ordinal) == 0;
        }

        public void changeType(String t)
        {
            type = t;
        }

        public void dataGather(int a)
        {
            data = a;
        }
        public void setText(String s)
        {
            gomb.Text = s;
        }
    }
}
