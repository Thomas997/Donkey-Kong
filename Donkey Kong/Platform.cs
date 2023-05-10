using NAudio.Gui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Donkey_Kong
{
    public partial class Platform : Form
    {
        // Variabelen
        bool goLeft, goRight, jumping, isGameOver, usingLadder, enemyGoLeft, enemyGoRight;
        int jumpSpeed, speedLadderUp, force, score = 0, playerSpeed = 7;

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

        #region Main game timer
        // Dit is de timer hier gebeurd alles met beweging
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            score += 1;

            txtScore.Text = "" + score;
            txtHighscore.Text = "" + score; //moet nog naar highsscore veranderd worden


            if (enemyMovement() == true)
            {
                Barrel.Left += barrelSpeed;
                Barrel.Top += 10;
            }
            else if (enemyMovement() == false)
            {
                Barrel.Left -= barrelSpeed;
            }








            // is voor naar links te gaan
            if (goLeft == true)
            {
                Player.Left -= playerSpeed;
            }

            // is voor naar rechts te gaan
            if (goRight == true)
            {
                Player.Left += playerSpeed;
            }

            // als usingladder false is dan kan je springen anders is usingladder true en ben je een ladder aan het gebruiken
            if (!usingLadder)
            {
                // Dankzij dit kun je omhoog gaan als je springt
                Player.Top += jumpSpeed;

                // Dit zorgt er voor dat de game weet hoe hoog je springt en het checked of de force 0 is dus dat je mag springen
                if (jumping == true && force < 0)
                {
                    jumping = false;
                }

                // Als het springen begonnen is dan moeten de variabelen naar benden zodat het ook stopt anders mag het gewoon standaard zodat je kunt springen
                if (jumping == true)
                {
                    jumpSpeed = -9; //old variable -8
                    force -= 1;
                }
                else
                {
                    jumpSpeed = 9; //old variable 10
                }
            }
            else
            {
                // Dankzij dit kun je omhoog gaan als je de ladder gebruikt
                Player.Top += speedLadderUp;
                speedLadderUp = -4;

                if (!IsPlayerOnLadder())
                {
                    usingLadder = false;
                }
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
                    // Dit is tegen clipping  van de ladders
                    x.SendToBack();
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
                        txtScore.Text = "" + score;
                    }
                    //Barrel.Left += barrelSpeed;
                }
            }
        }
        #endregion


        #region Collisions
        // deze code zorgt er voor dat als je de bounds van een ladder aanraakt dat usingladder true is zodat je kunt klimmen en niet meer kunt springen
        private bool IsPlayerOnLadder()
        {
            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && (String)x.Tag == "ladder")
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool enemyMovement()
        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (String)x.Tag == "enemyMovementCheckpoint")
                {
                    if (Barrel.Bounds.IntersectsWith(x.Bounds))
                    {
                        return true;
                    }
                    x.SendToBack();
                }
            }
            return false;
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
        // Credit: Zelf gemaakt maar het werkt door hulp van leerkracht
        private void checkCollisionPlatform(PictureBox platform)
        {
            // Deze code zorgt er voor dat de game weet of je een platform raakt en zet het de variabelen en de player correct
            if (Player.Bounds.IntersectsWith(platform.Bounds) && Player.Bottom <= platform.Bottom && Player.Bottom >= platform.Top)
            {
                force = 8;
                Player.Top = platform.Top - Player.Height;
            }
        }
        #endregion


        #region Keybinds
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

            // Dit is de code voor als je de key up in klikt en als de speler op de ladder is zodat je de variabelen usingladder op true zet zodat het werkt
            usingLadder = (e.KeyCode == Keys.Up && IsPlayerOnLadder());
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

            // Dit is de code voor eindigen van het gebruiken van de ladder als dat al gebeurt is 
            if (usingLadder == true)
            {
                //usingLadder = false;
            }

            // Als er op enter wordt ge drukt herstart de game
            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                RestartGame();
            }
        }
        #endregion

        
        #region restart events
        // Deze functie herstart het spel en reset de variabelen
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void RestartGame()
        {
            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;
            score = 0;

            txtScore.Text = "" + score;
            txtHighscore.Text = "" + score; //moet nog aangepast worden naar highscore variabelen

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                {
                    x.Visible = true;
                }
            }

            // Reset the position of player, platform and enemies
            Player.Left = 256;
            Player.Top = 875;

            Barrel.Left = 330;

            GameTimer.Start();
        }
        #endregion
    }
}
