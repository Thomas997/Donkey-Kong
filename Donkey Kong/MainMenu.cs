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
        private PictureBox pbxMarioPointer;
        string path = Application.StartupPath;
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

            // Mario pointer's picturebox maken
            pbxMarioPointer = new PictureBox();
            pbxMarioPointer.Image = Properties.Resources.MarioPointer;
            pbxMarioPointer.SizeMode = PictureBoxSizeMode.AutoSize;
            int itemTop = menuItems[selectedMenuItemIndex].Top;
            int itemLeft = menuItems[selectedMenuItemIndex].Left;
            pbxMarioPointer.Location = new Point(itemLeft - pbxMarioPointer.Width - 5, itemTop - 15);

            
            // Mario pointer toevoegen
            panel1.Controls.Add(pbxMarioPointer);

            // Key down -> eventhandler
            this.KeyDown += MainMenu_KeyDown;
        }


        //Main menu controls
        // ChatGPT
        private void MainMenu_KeyDown(object sender, KeyEventArgs e)
        {
            select.PlayOnce();
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
                            button.PlayOnce();
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
                            button.PlayOnce();
                            Task.Delay(5000);
                            ShowLeaderboard();
                            break;
                        case 2:
                            // Control animatie
                            button.PlayOnce();
                            Task.Delay(5000);
                            ShowControls();
                            break;
                    }
                    break;
            }

            // Mario repositioning
            int itemTop = menuItems[selectedMenuItemIndex].Top;
            int itemLeft = menuItems[selectedMenuItemIndex].Left;
            pbxMarioPointer.Location = new Point(itemLeft - pbxMarioPointer.Width - 5, itemTop - 15);
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

            string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{Application.StartupPath}\\Database.accdb';Persist Security Info=False;";
            var dbHelper = new DatabaseHelper(connectionString);
            var leaderboardData = dbHelper.GetTopHighScores(10); // Change the limit as per your requirement

            using (var form = new Form())
            {
                form.BackColor = Color.Black;
                form.ForeColor = Color.White;
                form.Font = new Font("Kongtext", 12, FontStyle.Regular);
                form.Width = 700;
                form.Height = 800;
                form.Text = "Leaderboard";
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.StartPosition = FormStartPosition.CenterScreen;

                var highScores = dbHelper.GetTopHighScores(10);

                var dataGridView = new DataGridView()
                {
                    Left = 15,
                    Top = 15,
                    Width = 550,
                    Height = 400,
                    BackgroundColor = Color.Black,
                    ForeColor = Color.White,
                    Font = new Font("Kongtext", 12, FontStyle.Regular),
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    RowHeadersVisible = false,
                    ColumnHeadersVisible = true,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
                };
                // Set the background color and text color for columns
                dataGridView.DefaultCellStyle.BackColor = Color.Black;
                dataGridView.DefaultCellStyle.ForeColor = Color.White;
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.DefaultCellStyle.BackColor = Color.Black;
                    column.DefaultCellStyle.ForeColor = Color.White;
                }
                // Define the columns for the DataGridView
                dataGridView.Columns.Add("PlayerName", "Player Name");
                dataGridView.Columns.Add("Score", "Score");
                dataGridView.Columns.Add("Date", "Date");

                // Sort the high scores based on the score
                var sortedHighScores = highScores.OrderByDescending(score => score.Score).ToList();

                // Populate the DataGridView with the sorted high scores
                foreach (var score in sortedHighScores)
                {
                    dataGridView.Rows.Add(score.PlayerName, score.Score, score.Date.ToString("dd-MM-yyyy"));
                }

                // Assuming you have three columns: PlayerName, Score, and Date
                dataGridView.Columns[0].Width = 250;  // Set the width of the second column
                dataGridView.Columns[1].Width = 100;  // Set the width of the third column
                dataGridView.Columns[2].Width = 220;  // Set the width of the third column

                form.Controls.Add(dataGridView);
                form.ShowDialog();
            }
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
