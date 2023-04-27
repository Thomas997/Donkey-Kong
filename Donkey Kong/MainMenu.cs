using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace Donkey_Kong
{
    public partial class MainMenu : Form
    {
        private List<Label> menuItems;
        private int selectedMenuItemIndex;
        private PictureBox marioPictureBox;
        string path = Application.StartupPath;
        SoundPlayer MainTheme = new SoundPlayer(Application.StartupPath + @"\DKtheme.wav");
        SoundPlayer button = new SoundPlayer(Application.StartupPath + @"\button.wav");
        public MainMenu()
        {
            InitializeComponent();

            //Main menu muziek afspelen
            MainTheme.Play();

            // Initialiseer het menu lijstje
            //Hulp van ChatGPT
            menuItems = new List<Label>
            {
                lblNewGame,
                lblLeaderboard,
                lblControls
            };

            // Zet het geselecteerde menu op 0
            selectedMenuItemIndex = 0;
            menuItems[selectedMenuItemIndex].ForeColor = Color.Red;

            // Create and position the Mario picture box
            marioPictureBox = new PictureBox();
            marioPictureBox.Image = Properties.Resources.MarioPointer;
            marioPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
            int itemTop = menuItems[selectedMenuItemIndex].Top;
            int itemLeft = menuItems[selectedMenuItemIndex].Left;
            marioPictureBox.Location = new Point(itemLeft - marioPictureBox.Width - 5, itemTop - 15);


            // Mario pointer toevoegen
            panel1.Controls.Add(marioPictureBox);

            // Key down -> eventhandler
            this.KeyDown += MainMenu_KeyDown;
        }

        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // Naar boven
                    if (selectedMenuItemIndex > 0)
                    {
                        menuItems[selectedMenuItemIndex].ForeColor = Color.White;
                        selectedMenuItemIndex--;
                        menuItems[selectedMenuItemIndex].ForeColor = Color.Red;
                    }
                    break;
                case Keys.Down:
                    // Naar onder
                    if (selectedMenuItemIndex < menuItems.Count - 1)
                    {
                        menuItems[selectedMenuItemIndex].ForeColor = Color.White;
                        selectedMenuItemIndex++;
                        menuItems[selectedMenuItemIndex].ForeColor = Color.Red;
                    }
                    break;
                case Keys.Space:
                case Keys.Enter:
                    // Enter geselecteerde index
                    switch (selectedMenuItemIndex)
                    {
                        case 0:
                            // Start spel
                            MainTheme.Stop();
                            button.Play();
                            Task.Delay(5000);
                            StartGame();
                            break;
                        case 1:
                            // Leaderboard laten zien
                            button.Play();
                            Task.Delay(5000);
                            ShowLeaderboard();
                            break;
                        case 2:
                            // Control animatie
                            button.Play();
                            Task.Delay(5000);
                            ShowControls();
                            break;
                    }
                    break;
            }

            // Mario repositioning
            int itemTop = menuItems[selectedMenuItemIndex].Top;
            int itemLeft = menuItems[selectedMenuItemIndex].Left;
            marioPictureBox.Location = new Point(itemLeft - marioPictureBox.Width - 5, itemTop - 15);
        }

        private void StartGame()
        {
            menuItems[selectedMenuItemIndex].ForeColor = Color.Yellow;
            //Verander startactie
            //Credit: verschillende sites geholpen
            Platform Platform = new Platform();
            //Nieuwe tab openen en deze dicht doen
            this.Hide();
            Platform.ShowDialog();
            this.Close();
        }

        private void ShowLeaderboard()
        {
            menuItems[selectedMenuItemIndex].ForeColor = Color.Yellow;
            // TODO: Implementeer leaderboard logica
        }

        private void ShowControls()
        {
            menuItems[selectedMenuItemIndex].ForeColor = Color.Yellow;
            // TODO: Implementeer control window
            ShowControls ShowControls = new ShowControls();
            this.Hide();
            ShowControls.ShowDialog();
            this.Close();
        }
    }
}
