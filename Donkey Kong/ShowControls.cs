using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Media;
using System.Runtime.Versioning;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using NAudio.Wave;
using System.Linq.Expressions;

namespace Donkey_Kong
{
    public partial class ShowControls : Form
    {
        private List<Label> menuItems;
        private int selectedMenuItemIndex;
        private PictureBox marioPictureBox;
        string path = Application.StartupPath;

        //AudioPlayers (van een class) aanmaken
        //chatGPT
        AudioPlayer button = new AudioPlayer(Donkey_Kong.Properties.Resources.button);
        public ShowControls()
        {
            InitializeComponent();

            // Initialiseer het menu lijstje
            //Hulp van ChatGPT
            menuItems = new List<Label>
            {
                lblReturn
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
        //Main menu controls
        // ChatGPT
        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                case Keys.Enter:
                    // Enter geselecteerde index
                    switch (selectedMenuItemIndex)
                    {
                        case 0:
                            // Ga terug naar main menu
                            button.Play();
                            Task.Delay(5000);
                            Return();
                            break;
                    }
                    break;
            }

            // Mario repositioning
            int itemTop = menuItems[selectedMenuItemIndex].Top;
            int itemLeft = menuItems[selectedMenuItemIndex].Left;
            marioPictureBox.Location = new Point(itemLeft - marioPictureBox.Width - 5, itemTop - 15);
        }
        private void Return()
        {
            menuItems[selectedMenuItemIndex].ForeColor = Color.Yellow;
            MainMenu MainMenu = new MainMenu(true);
            this.Hide();
            MainMenu.ShowDialog();
            this.Close();
        }
    }
}
