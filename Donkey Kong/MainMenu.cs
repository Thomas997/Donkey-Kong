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
using System.Runtime.InteropServices;

namespace Donkey_Kong
{
    public partial class MainMenu : Form
    {
        //variabele aanmaken
        private List<Label> menuItems;
        private int selectedMenuItemIndex;
        private PictureBox marioPictureBox;
        string path = Application.StartupPath;
        bool isThemePlaying = false;
        //AudioPlayers (van een class) aanmaken
        //chatGPT
        AudioPlayer button = new AudioPlayer(Donkey_Kong.Properties.Resources.button);
        AudioPlayer MainTheme = new AudioPlayer(Donkey_Kong.Properties.Resources.DKtheme);
        AudioPlayer select = new AudioPlayer(Donkey_Kong.Properties.Resources.blipSelect);

        public MainMenu(bool isThemePlaying)
        {
            InitializeComponent();
            if (isThemePlaying == false)
            {
                MainTheme.Play();
                isThemePlaying = true;
            }
            //main theme initialiseren van AudioPlayer en zorgen dat alle instanties van MainTheme eerst weg zijn
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


        //Main menu controls
        // ChatGPT
        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            select.Play();
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
                            // Vraag de gebruiker voor een username
                            //Credit: ChatGPT
                            string name = "Anonymous";
                            using (var form = new Form())
                            {
                                form.BackColor = Color.Black;
                                form.ForeColor = Color.White;
                                form.Font = new Font("Kongtext", 12, FontStyle.Regular);
                                form.ClientSize = new Size(300, 150);
                                form.Text = "Enter your name";
                                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                                form.MaximizeBox = false;
                                form.MinimizeBox = false;
                                form.StartPosition = FormStartPosition.CenterScreen;

                                var textBox = new TextBox()
                                {
                                    Left = 15,
                                    Top = 15,
                                    Width = 250,
                                    BackColor = Color.Black,
                                    ForeColor = Color.White,
                                    Font = new Font("Kongtext", 26),
                                    BorderStyle = BorderStyle.None
                                };
                                textBox.TextAlign = HorizontalAlignment.Center;
                                var button = new Button() { Text = "SUBMIT", Left = 42, Width = 200, Height = 50, Top = 75, DialogResult = DialogResult.OK };

                                button.Click += (sender2, e2) => { form.Close(); };

                                form.Controls.Add(textBox);
                                form.Controls.Add(button);
                                form.AcceptButton = button;

                                if (form.ShowDialog() == DialogResult.OK)
                                {
                                    name = textBox.Text;
                                }
                            }
                            StartGame(name);
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

        private void StartGame(string name)
        {
            menuItems[selectedMenuItemIndex].ForeColor = Color.Yellow;
            //Verander startactie
            //Credit: verschillende sites geholpen
            Platform Platform = new Platform(MainTheme, name); 

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
            // Formulier verbergen en weer laten zien wanneer DialogResult = Ok bij Showcontrols
            //Credit: meester Feijen
            ShowControls ShowControls = new ShowControls();
            this.Hide();
            if (ShowControls.ShowDialog() == DialogResult.OK)
            {
                this.Show();
            }
        }
    }
}
