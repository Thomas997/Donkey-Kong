using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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

        public MainMenu()
        {
            InitializeComponent();

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


            // Add the Mario picture box to the panel
            panel1.Controls.Add(marioPictureBox);

            // Hook up the key down event handler
            this.KeyDown += MainMenu_KeyDown;
        }

        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // Move the selection up
                    if (selectedMenuItemIndex > 0)
                    {
                        menuItems[selectedMenuItemIndex].ForeColor = Color.White;
                        selectedMenuItemIndex--;
                        menuItems[selectedMenuItemIndex].ForeColor = Color.Red;
                    }
                    break;
                case Keys.Down:
                    // Move the selection down
                    if (selectedMenuItemIndex < menuItems.Count - 1)
                    {
                        menuItems[selectedMenuItemIndex].ForeColor = Color.White;
                        selectedMenuItemIndex++;
                        menuItems[selectedMenuItemIndex].ForeColor = Color.Red;
                    }
                    break;
                case Keys.Space:
                case Keys.Enter:
                    // Perform the action for the selected menu item
                    switch (selectedMenuItemIndex)
                    {
                        case 0:
                            // Start the game
                            StartGame();
                            break;
                        case 1:
                            // Show the leaderboard
                            ShowLeaderboard();
                            break;
                        case 2:
                            // Show the controls
                            ShowControls();
                            break;
                    }
                    break;
            }

            // Reposition the Mario picture box next to the selected menu item
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
            this.Hide();
            Platform.ShowDialog();

            Platform.Left = this.Left;
            Platform.Top = this.Top;
            Platform.Size = this.Size;

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

            ShowControls.Left = this.Left;
            ShowControls.Top = this.Top;
            ShowControls.Size = this.Size;

            this.Close();
        }
    }
}
