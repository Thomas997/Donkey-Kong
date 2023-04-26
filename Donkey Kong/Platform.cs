using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Donkey_Kong
{
    public partial class Platform : Form
    {
        // Variabelen
        bool goLeft, goRight, jumping, isGameOver;

        int jumpSpeed;
        int force;
        int score = 0;
        int playerSpeed = 7;

        int horizontalSpeed = 5;
        int verticalSpeed = 3;

        int barrelSpeed = 5;


        public Platform()
        {
            InitializeComponent();
        }

        // Dit is de timer
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score;

            // zwaartekracht
            Player.Top += jumpSpeed;

            if(goLeft == true)
            {
                Player.Left -= playerSpeed;
            }
            
            if(goRight == true)
            {
                Player.Left += playerSpeed;
            }

            // Dit zorgt er voor dat de game weet hoe hoog je springt en het checked of de force 0 is dus dat je mag springen
            if(jumping == true && force < 0)
            {
                jumping = false;
            }

            // Als het springen begonnen is dan moeten de variabelen naar benden zodat het ook stopt anders mag het gewoon standaard zodat je kunt springen
            if(jumping == true)
            {
                jumpSpeed = -8;
                force -= 1;
            }
            else
            {
                jumpSpeed = 10;
            }

            foreach(Control x in this.Controls)
            {
                if(x is PictureBox)
                {
                    // Hier gebruiken we de tag van de platformen zodat de game dit als een vast object ziet waar je kunt op staan
                    if ((string)x.Tag == "platform")
                    {

                        if(Player.Bounds.IntersectsWith(x.Bounds))
                        {
                            force = 8;
                            Player.Top = x.Top - Player.Height;


                        }

                        // Dit is voor het gelitch bij de speler in te perken als dit er niet is dan kan de speler sprite door het platform glitchen
                        x.BringToFront();
                    }


                    



                    // Dit is de code voor de enemy het zegt dat als de speler collide met de enemy dan eindigt het spel (timer af)
                    if ((string)x.Tag == "Enemy")
                    {
                        if(Player.Bounds.IntersectsWith(x.Bounds))
                        {
                            GameTimer.Stop();
                            isGameOver = true;
                            txtScore.Text = "Score: " + score + Environment.NewLine + "KO";
                        }
                    }

                }
            }



        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Dit is als er een toets is in gedrukt
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            // Dit is de code voor de linker toets ingedrukt
            if(e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            
            // Dit is de code voor de rechter toets ingedrukt
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }

            // Dit is de code voor de spaciebalk en als je springt
            if(e.KeyCode == Keys.Space && jumping == false)
            {
                jumping = true;
            }
        }

        // Dit is als er een toets is los gelaten
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            // Dit is de code voor de linker toets als losgelaten
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            // Dit is de code voor de rechter toets als losgelaten
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            // Dit is de code voor eindigen van het springen als dat al gebeurt is 
            if(jumping == true)
            {
                jumping = false;
            }

            // Als er op enter wordt ge drukt herstart de game
            if(e.KeyCode == Keys.Enter && isGameOver == true)
            {
                RestartGame();
            }
        }

        // Deze functie herstart het spel en reset de variabelen
        private void RestartGame()
        {
            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;
            score = 0;

            txtScore.Text = "Score: " + score;

            foreach(Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                {
                    x.Visible = true;
                }
            }

            // Reset the position of player, platform and enemies

            Player.Left = 99;
            Player.Top = 688;

            Barrelone.Left = 160;

            GameTimer.Start();
        }
    }
}
