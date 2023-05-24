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
using Donkey_Kong;
using System.Runtime.Versioning;

namespace Donkey_Kong
{
    public partial class Platform : Form
    {
        // Variabelen
        bool goLeft, goRight, jumping, isGameOver, usingLadder, isPaused = false;
        int jumpSpeed, speedLadderUp, force, playerSpeed = 7, barrelSpeed = 0;

        private string playerName; // Declareer name als een veld in deze class

        public Platform(AudioPlayer MainTheme, string name)
        {
            InitializeComponent();
            playerName = name;
            AudioPlayer GameMusic = new AudioPlayer(Donkey_Kong.Properties.Resources.Game);
            GameMusic.Play();
            MainTheme.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // geen clipping met barrelremoval picturebox
            pbxBarrelRemoval.SendToBack();
        }

        #region Barrel timer
        //dit is de timer van de barrels te laten spawnen
        private void BarrelTimer_Tick(object sender, EventArgs e)
        {
            // Create a random number generator
            Random random = new Random();

            // Generate a random index to select the interval from the available options
            int index = random.Next(0, 3);

            // Define the available interval options
            int[] intervals = { 1000, 2000, 3000 };

            // Retrieve the randomly selected interval
            int interval = intervals[index];

            // Update the timer interval
            BarrelTimer.Interval = interval;

            // hier maken we de barrel aan in de code zelf niet meer een picture box in de form geplaatst en we her gebruiken al de variabelen van de oude barrel
            PictureBox barrel = new PictureBox();

            barrel.BackColor = System.Drawing.Color.Brown;
            barrel.Location = new System.Drawing.Point(330, 206);
            barrel.Name = "Barrel";
            barrel.Size = new System.Drawing.Size(30, 31);
            barrel.TabIndex = 9;
            barrel.TabStop = false;
            barrel.Tag = "right";
            barrel.BackgroundImage = Properties.Resources.Barrel;
            barrel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            barrel.BackColor = Color.Black;
            this.Controls.Add(barrel);
        }
        #endregion

        #region Main game timer
        // Dit is de timer hier gebeurd alles met beweging
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            
            //txtHighscore.Text = "" + score; //moet nog naar highsscore veranderd worden

            // deze foreach zorgt er voor dat we meerdere barrels gaan kunnen gebruiken
            foreach(Control c in this.Controls)
            {
                // hier zetten we de picturebox gelijk aan de barrel
                if(c is PictureBox && c.Name.StartsWith("Barrel"))
                {
                    // de tag is al right dus gaat de barrel naar rechts gaan bewegen tot het de rightsidewall raakt en dan gaat de tag naar links
                    if((string)c.Tag == "right")
                    {
                        c.Left += barrelSpeed;
                        c.Top += 10;
                        if(c.Left > pbxRightSidewall.Left - 30)
                        {
                            c.Tag = "left";
                        }
                    }
                    // als barrel links is geworden dan moet die ook naar links tot die de leftsidewall aan raakt en dan verandert de tag weer naar right
                    if ((string)c.Tag == "left")
                    {
                        c.Left -= barrelSpeed;
                        c.Top += 10;
                        if (c.Left < pbxLeftSidewall.Left + pbxLeftSidewall.Width)
                        {
                            c.Tag = "right";
                        }
                    }

                    // zegt dat als de speler collide met de enemy dan eindigt het spel (timers af)
                    // Credit: https://youtu.be/rQBHwdEEL9I + chatgtp
                    PictureBox barrel = (PictureBox)c;

                    IfPlayerTouchBarrelStopGame((PictureBox)barrel);

                    RemoveBarrelAtEndOfBarrelTrack((PictureBox)barrel);
                }
            }

            // is voor naar links te gaan
            if (goLeft == true)
            {
                pbxPlayer.Left -= playerSpeed;
                //pbxPlayer.BackgroundImage = Properties.Resources.Barrel; nog aanpassen naar juiste image van mario
            }

            // is voor naar rechts te gaan
            if (goRight == true)
            {
                pbxPlayer.Left += playerSpeed;
                //pbxPlayer.BackgroundImage = Properties.Resources.Barrel; nog aanpassen naar juiste image van mario
            }

            // als usingladder false is dan kan je springen anders is usingladder true en ben je een ladder aan het gebruiken
            if (!usingLadder)
            {
                // Dankzij dit kun je omhoog gaan als je springt
                pbxPlayer.Top += jumpSpeed;

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
                pbxPlayer.Top += speedLadderUp;
                speedLadderUp = -4;

                if (!IsPlayerOnLadder())
                {
                    usingLadder = false;
                }
            }

            foreach (Control x in this.Controls)
            {
                // Hier gebruiken we de tag van de platformen om ze te identificeren als vaste objecten
                // Credit: https://youtu.be/rQBHwdEEL9I
                if ((string)x.Tag == "platform" && x is PictureBox)
                {
                    IfSidewallCollisionSetPlayerRightOnPlatform((PictureBox)x);

                    // Dit is voor het gelitch bij de speler in te perken als dit er niet is dan kan de speler sprite door het platform glitchen
                    x.BringToFront();
                }

                if ((string)x.Tag == "ladder" && x is PictureBox)
                {
                    // Dit is tegen clipping  van de ladders
                    x.SendToBack();
                }

                // Hier gebruiken we de tag van de sidewall om ze te identificeren als vaste objecten
                // Credit: gemaakt door chatgtp maar zelf moet aanpassen en uitzoeken hoe het werkt
                if ((string)x.Tag == "sidewall" && x is PictureBox)
                {
                    // Check for collision with the wall
                    IfSidewallCollisionStopPlayer((PictureBox)x);
                }

                // Hier gebruiken we de tag van End om ze te identificeren als vaste objecten
                if ((string)x.Tag == "End" && x is PictureBox)
                {
                    IfPlayerTouchEndStopGameAndRestart((PictureBox)x, playerName);
                }
            }
        }
        #endregion

        // Define a class-level variable for the score
        private int score = 0;

        // Method to update the score
        private void UpdateScore(ref int score)
        {
            score += 50;
            txtScore.Text = score.ToString();
        }

        #region Collisions
        // code voor end of the game
        private void IfPlayerTouchEndStopGameAndRestart(PictureBox FinalDestination, string playerName)
        {
            if (pbxPlayer.Bounds.IntersectsWith(FinalDestination.Bounds))
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd");

                // Pass the score by reference to the UpdateScore method
                UpdateScore(ref score);

                // Update the score in the database
                DatabaseHelper.AddOrUpdateHighScore(playerName, score, date);

                // Restart the game
                RestartGame();
            }
        }

        // Stopt de timers en herstart het spel
        // Credit: chatgtp + Thomas
        private void IfPlayerTouchBarrelStopGame(PictureBox barrel)
        {
            if (pbxPlayer.Bounds.IntersectsWith(barrel.Bounds))
            {
                // Behandel de botsing (speler raakt de barrel)
                GameTimer.Stop();
                BarrelTimer.Stop();
                isGameOver = true;
                //txtScore.Text = score.ToString();
            }
        }

        // Verwijdert de barrels die pbxBarrelRemoval aanraken 
        private void RemoveBarrelAtEndOfBarrelTrack(PictureBox barrel)
        {
            if (pbxBarrelRemoval.Bounds.IntersectsWith(barrel.Bounds))
            {
                this.Controls.Remove(barrel);

                //halt de barrel complete weg zodat er geen overload kan zijn (is optioneel omdat een barrel niet veel vraagt)
                barrel.Dispose();
            }
        }

        // deze code zorgt er voor dat als je de bounds van een ladder aanraakt dat usingladder true is zodat je kunt klimmen en niet meer kunt springen
        private bool IsPlayerOnLadder()
        {
            foreach(Control x in this.Controls)
            {
                if(x is PictureBox && (String)x.Tag == "ladder")
                {
                    if (pbxPlayer.Bounds.IntersectsWith(x.Bounds))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check for collision with the wall
        // Credit: gemaakt door chatgtp maar zelf moet aanpassen en uitzoeken hoe het werkt
        /// <summary>
        /// Als de speler de sidewall raakt stopt de speler tegen de sidewall en gaat het terug op het platform voor de sidewall.
        /// </summary>
        /// <param name="sidewall">dit is de picturebox van de sidewall</param>
        private void IfSidewallCollisionStopPlayer(PictureBox sidewall)
        {
            if (pbxPlayer.Bounds.IntersectsWith(sidewall.Bounds))
            {
                // Als de speler naar rechts beweegt, beweeg hem dan terug naar links van de muur als hij deze muur raakt
                if (goRight)
                {
                    pbxPlayer.Left = sidewall.Left - pbxPlayer.Width;
                }

                // Als de speler naar links beweegt, beweeg hem dan terug naar rechts van de muur als hij deze muur raakt
                if (goLeft)
                {
                    pbxPlayer.Left = sidewall.Right;
                }
            }
        }

        // collision met de onderkant van het platform
        // Credit: Zelf gemaakt maar het werkt door hulp van leerkracht
        private void IfSidewallCollisionSetPlayerRightOnPlatform(PictureBox platform)
        {
            // Deze code zorgt er voor dat de game weet of je een platform raakt en zet het de variabelen en de player correct
            if (pbxPlayer.Bounds.IntersectsWith(platform.Bounds) && pbxPlayer.Bottom <= platform.Bottom && pbxPlayer.Bottom >= platform.Top)
            {
                force = 8;
                pbxPlayer.Top = platform.Top - pbxPlayer.Height;
            }

            // Deze code is voor de enemy zijn bounds van de platformen
            foreach (Control c in this.Controls)
            {
                if(c is PictureBox && c.Name.StartsWith("Barrel"))
                {
                    if (c.Bounds.IntersectsWith(platform.Bounds))
                    {
                        c.Top = platform.Top - c.Height;
                    }
                }
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

            // Springen wordt op true gezet als spaciebalk wordt ingedrukt
            if (e.KeyCode == Keys.Space && jumping == false)
            {
                jumping = true;
            }

            // Toggle pause state when "P" key is pressed
            if (e.KeyCode == Keys.P)
            {
                isPaused = !isPaused;

                // Pause or resume the timers based on the pause state
                if (isPaused)
                {
                    GameTimer.Stop();
                    BarrelTimer.Stop();
                }
                else
                {
                    GameTimer.Start();
                    BarrelTimer.Start();
                }
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

            // Als er op enter wordt ge drukt herstart de game
            if (e.KeyCode == Keys.Enter && isGameOver == true)
            {
                RestartGame();
            }
        }
        #endregion

        #region Restart events
        // Deze functie herstart het spel en reset de variabelen
        // Credit: https://youtu.be/rQBHwdEEL9I
        private void RestartGame()
        {
            // zet alle controls af zodat je niets meer kunt doen terwijl je herstart
            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Visible == false)
                {
                    x.Visible = true;
                }
            }

            for(int i = this.Controls.Count - 1; i >= 0; i--)
            {
                Control x = this.Controls[i];
                if (x is PictureBox && x.Name.StartsWith("Barrel"))
                {
                    this.Controls.RemoveAt(i);
                }
            }

            // Reset the position of player, platform and enemies
            pbxPlayer.Left = 256;
            pbxPlayer.Top = 875;

            BarrelTimer.Start();
            GameTimer.Start();
        }
        #endregion

        #region Database
        //Sla de high score op
        //Full credit: ChatGPT
        public static void SaveHighScore(string name, int score)
        {
            // Haal de spelernaam op
            string playerName = name;
            int playerScore = score;

            // Sla de high score op in de database
            string formattedDate = DateTime.Now.ToString("dd-MM-yyyy");
            DatabaseHelper.AddOrUpdateHighScore(playerName, playerScore, formattedDate);
        }
        #endregion
    }
}
