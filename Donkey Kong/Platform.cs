using NAudio.Gui;
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

        int barrelSpeed = 5;


        public Platform()
        {
            InitializeComponent();
            AudioPlayer GameMusic = new AudioPlayer(Donkey_Kong.Properties.Resources.Game);
            GameMusic.Play();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Dit is de timer hier gebeurd alles met beweging
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score;

            // is voor naar links en rechts te kunnen gaan
            Player.Top += jumpSpeed;

            if (goLeft == true)
            {
                Player.Left -= playerSpeed;
            }

            if (goRight == true)
            {
                Player.Left += playerSpeed;
            }


            // Dit zorgt er voor dat de game weet hoe hoog je springt en het checked of de force 0 is dus dat je mag springen
            if (jumping == true && force < 0)
            {
                jumping = false;
            }


            // Als het springen begonnen is dan moeten de variabelen naar benden zodat het ook stopt anders mag het gewoon standaard zodat je kunt springen
            if (jumping == true)
            {
                jumpSpeed = -7; //old variable -8
                force -= 1;
            }
            else
            {
                jumpSpeed = 9; //old variable 10
            }


            foreach (Control x in this.Controls)
            {

                // Here we use the tag of the platforms to identify them as solid objects
                // Credit: https://youtu.be/rQBHwdEEL9I
                if ((string)x.Tag == "platform" && x is PictureBox)
                {

                    checkCollisionPlatform((PictureBox)x);

                    // Dit is voor het gelitch bij de speler in te perken als dit er niet is dan kan de speler sprite door het platform glitchen
                    x.BringToFront();
                }

                if ((string)x.Tag == "ladder" && x is PictureBox)
                {
                    checkCollisionladder((PictureBox)x);

                }

                // Here we use the tag of the platforms to identify them as solid objects
                // Credit: gemaakt door chatgtp maar zelf moet aanpassen en uitzoeken hoe het werkt
                if ((string)x.Tag == "sidewall" && x is PictureBox)
                {
                    // Check for collision with the wall
                    checkSidewallCollision((PictureBox)x);

                    // Check for collision with the top or bottom of the wall
                    checkCollisionTopBottomWall((PictureBox)x);
                }

                // Dit is de code voor de enemy het zegt dat als de speler collide met de enemy dan eindigt het spel (timer af)
                // Credit: https://youtu.be/rQBHwdEEL9I
                if ((string)x.Tag == "Enemy")
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        GameTimer.Stop();
                        isGameOver = true;
                        txtScore.Text = "Score: " + score + Environment.NewLine + "KO";
                    }
                }
            }
        }


        // Check for collision with the wall
        // Credit: gemaakt door chatgtp maar zelf moet aanpassen en uitzoeken hoe het werkt
        private void checkSidewallCollision(PictureBox sidewall)
        {
            if (Player.Bounds.IntersectsWith(sidewall.Bounds))
            {
                // If the player is moving to the right, move them back to the left of the wall
                if (goRight)
                {
                    Player.Left = sidewall.Left - Player.Width;
                }

                // If the player is moving to the left, move them back to the right of the wall
                if (goLeft)
                {
                    Player.Left = sidewall.Right;
                }
            }
        }


        // Check for collision with the top or bottom of the wall
        // Credit: gemaakt door chatgtp maar zelf moet aanpassen en uitzoeken hoe het werkt
        private void checkCollisionTopBottomWall(PictureBox sidewall)
        {
            if (Player.Bounds.IntersectsWith(sidewall.Bounds))
            {
                // If the player is moving down, move them back up to the top of the wall
                if (force > 0)
                {
                    Player.Top = sidewall.Top - Player.Height;
                }

                // If the player is moving up, move them back down to the bottom of the wall
                else if (force < 0)
                {
                    Player.Top = sidewall.Bottom;
                }
            }
        }


        // collision met de onderkant van het platform
        // Credit:
        private void checkCollisionPlatform(PictureBox platform)
        {
            if (Player.Bounds.IntersectsWith(platform.Bounds) && Player.Bottom <= platform.Bottom && Player.Bottom >= platform.Top)
            {
                force = 8;
                Player.Top = platform.Top - Player.Height;
            }

        }


        private void checkCollisionladder(PictureBox ladder)
        {
            if (Player.Bounds.IntersectsWith(ladder.Bounds))
            {
                force = 8;
                jumping = true;
            }
        }


        // Dit is als er een toets is in gedrukt
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            // Dit is de code voor de linker toets ingedrukt
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }

            // Dit is de code voor de rechter toets ingedrukt
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }


            // Dit is de code voor de spaciebalk en als je springt
            if (e.KeyCode == Keys.Space && jumping == false)
            {
                jumping = true;
            }

        }


        // Dit is als er een toets is los gelaten
        // Credit: https://youtu.be/rQBHwdEEL9I
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
            if (jumping == true)
            {
                jumping = false;
            }

            // Als er op enter wordt ge drukt herstart de game
            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                RestartGame();
            }
        }


        // Deze functie herstart het spel en reset de variabelen
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void RestartGame()
        {
            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;
            score = 0;

            txtScore.Text = "Score: " + score;

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                {
                    x.Visible = true;
                }
            }

            // Reset the position of player, platform and enemies

            Player.Left = 335;
            Player.Top = 614;

            Barrelone.Left = 311;

            GameTimer.Start();
        }
        
    }
}
