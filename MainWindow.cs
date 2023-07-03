using ReverseWordle.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.DataFormats;


namespace ReverseWordle
{
    public partial class MainWindow : Form
    {

        protected string guess;
        protected int guesses;
        protected List<string> greyLetters = new List<string>();
        protected List<string> yellowLetters = new List<string>();
        protected List<int> yellowPlaces = new List<int>();
        protected string[] greenLetters = new string[5];
        protected List<string> words = new List<string>();
        public int streak;

        public MainWindow(int streak = 0)
        {
            InitializeComponent();
            this.streak = streak;
            Startup();
        }

        private void colorChange(object sender, EventArgs e)
        {
            if (sender.Equals(bg1a)) changer(bg1a, txtChar1a);
            else if (sender.Equals(bg1b)) changer(bg1b, txtChar1b);
            else if (sender.Equals(bg1c)) changer(bg1c, txtChar1c);
            else if (sender.Equals(bg1d)) changer(bg1d, txtChar1d);
            else if (sender.Equals(bg1e)) changer(bg1e, txtChar1e);

            if (sender.Equals(bg2a)) changer(bg2a, txtChar2a);
            else if (sender.Equals(bg2b)) changer(bg2b, txtChar2b);
            else if (sender.Equals(bg2c)) changer(bg2c, txtChar2c);
            else if (sender.Equals(bg2d)) changer(bg2d, txtChar2d);
            else if (sender.Equals(bg2e)) changer(bg2e, txtChar2e);

            if (sender.Equals(bg3a)) changer(bg3a, txtChar3a);
            else if (sender.Equals(bg3b)) changer(bg3b, txtChar3b);
            else if (sender.Equals(bg3c)) changer(bg3c, txtChar3c);
            else if (sender.Equals(bg3d)) changer(bg3d, txtChar3d);
            else if (sender.Equals(bg3e)) changer(bg3e, txtChar3e);

            if (sender.Equals(bg4a)) changer(bg4a, txtChar4a);
            else if (sender.Equals(bg4b)) changer(bg4b, txtChar4b);
            else if (sender.Equals(bg4c)) changer(bg4c, txtChar4c);
            else if (sender.Equals(bg4d)) changer(bg4d, txtChar4d);
            else if (sender.Equals(bg4e)) changer(bg4e, txtChar4e);

            if (sender.Equals(bg5a)) changer(bg5a, txtChar5a);
            else if (sender.Equals(bg5b)) changer(bg5b, txtChar5b);
            else if (sender.Equals(bg5c)) changer(bg5c, txtChar5c);
            else if (sender.Equals(bg5d)) changer(bg5d, txtChar5d);
            else if (sender.Equals(bg5e)) changer(bg5e, txtChar5e);

            if (sender.Equals(bg6a)) changer(bg6a, txtChar6a);
            else if (sender.Equals(bg6b)) changer(bg6b, txtChar6b);
            else if (sender.Equals(bg6c)) changer(bg6c, txtChar6c);
            else if (sender.Equals(bg6d)) changer(bg6d, txtChar6d);
            else if (sender.Equals(bg6e)) changer(bg6e, txtChar6e);

        }

        public async void btnGuess_Click(object sender, EventArgs e)
        {
            if (guesses == 0) standardGameOver();
            else if (guesses > 0)
            {
                if (!checkEarlyWin())
                {
                    await Task.Run(() => { updateLists(); });
                    await Task.Run(() => { words = pruneDict(); });
                    newGuess();
                    guesses--;
                }
            }
        }

        private void btnNewGame_Click(object sender, EventArgs e)
        {
            MainWindow mainWindow = new MainWindow(streak);
            this.Hide();
            mainWindow.FormClosed += (s, args) => this.Close();
            mainWindow.Show();
        }

        private List<string> pruneDict()
        {
            List<string> toRemove = new List<string>();

            // Removes words containing grey letters
            for (int j = 0; j < greyLetters.Count; j++)
            {
                string letter = greyLetters[j];
                for (int i = words.Count - 1; i >= 0; i--)
                {
                    string word = words[i];
                    if (greenLetters.Contains(letter) && !String.IsNullOrEmpty(letter))
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            if (word[k].ToString() == letter && k != Array.IndexOf(greenLetters, letter))
                            {
                                toRemove.Add(word);
                                break;
                            }
                        }
                    }
                    else if (word.Contains(letter))
                    {
                        toRemove.Add(word);
                    }
                }
            }

            foreach (string word in toRemove) words.Remove(word);
            toRemove.Clear();

            // Removes words where the yellow letters are in incorrect places
            for (int i = words.Count - 1; i >= 0; i--)
            {
                string word = words[i];
                for (int j = 0; j < yellowLetters.Count; j++)
                {
                    if (!word.Contains(yellowLetters[j]))
                    {
                        toRemove.Add(word);
                        break; // Because the word will be removed anyway
                    }
                    if (word[yellowPlaces[j]].ToString() == yellowLetters[j])
                    {
                        toRemove.Add(word);
                        break; // Because the word will be removed anyway
                    }
                }
            }

            foreach (string word in toRemove) words.Remove(word);
            toRemove.Clear();

            // Remove words without found green letters
            for (int i = words.Count - 1; i >= 0; i--)
            {
                string word = words[i];
                for (int j = 0; j < 5; j++)
                {
                    if (!String.IsNullOrEmpty(greenLetters[j]))
                    {
                        if (!word.Contains(greenLetters[j]))
                        {
                            toRemove.Add(word);
                            break;
                        }
                        if (greenLetters[j] != word[j].ToString())
                        {
                            toRemove.Add(word);
                            break;
                        }
                    }
                }
            }

            foreach (string word in toRemove) words.Remove(word);
            toRemove.Clear();
            return words;
        }

        protected void newGuess()
        {
            // Choose new word
            Random random = new Random();
            try
            {
                int i = random.Next(0, (words.Count) - 1);
                guess = words[i];

                // Split guess into letters
                switch (guesses)
                {
                    case 6:
                        txtChar1a.Text = guess[0].ToString();
                        txtChar1b.Text = guess[1].ToString();
                        txtChar1c.Text = guess[2].ToString();
                        txtChar1d.Text = guess[3].ToString();
                        txtChar1e.Text = guess[4].ToString();
                        break;
                    case 5:
                        txtChar2a.Text = guess[0].ToString();
                        txtChar2b.Text = guess[1].ToString();
                        txtChar2c.Text = guess[2].ToString();
                        txtChar2d.Text = guess[3].ToString();
                        txtChar2e.Text = guess[4].ToString();
                        break;
                    case 4:
                        txtChar3a.Text = guess[0].ToString();
                        txtChar3b.Text = guess[1].ToString();
                        txtChar3c.Text = guess[2].ToString();
                        txtChar3d.Text = guess[3].ToString();
                        txtChar3e.Text = guess[4].ToString();
                        break;
                    case 3:
                        txtChar4a.Text = guess[0].ToString();
                        txtChar4b.Text = guess[1].ToString();
                        txtChar4c.Text = guess[2].ToString();
                        txtChar4d.Text = guess[3].ToString();
                        txtChar4e.Text = guess[4].ToString();
                        break;
                    case 2:
                        txtChar5a.Text = guess[0].ToString();
                        txtChar5b.Text = guess[1].ToString();
                        txtChar5c.Text = guess[2].ToString();
                        txtChar5d.Text = guess[3].ToString();
                        txtChar5e.Text = guess[4].ToString();
                        break;
                    case 1:
                        txtChar6a.Text = guess[0].ToString();
                        txtChar6b.Text = guess[1].ToString();
                        txtChar6c.Text = guess[2].ToString();
                        txtChar6d.Text = guess[3].ToString();
                        txtChar6e.Text = guess[4].ToString();
                        break;
                }

                if (guesses == 1 && words.Count == 1)
                {
                    bg6a.BackgroundImage = Properties.Resources.green;
                    bg6b.BackgroundImage = Properties.Resources.green;
                    bg6c.BackgroundImage = Properties.Resources.green;
                    bg6d.BackgroundImage = Properties.Resources.green;
                    bg6e.BackgroundImage = Properties.Resources.green;

                    txtChar6a.BackgroundImage = Properties.Resources.green;
                    txtChar6b.BackgroundImage = Properties.Resources.green;
                    txtChar6c.BackgroundImage = Properties.Resources.green;
                    txtChar6d.BackgroundImage = Properties.Resources.green;
                    txtChar6e.BackgroundImage = Properties.Resources.green;

                    txtChar6a.Enabled = false;
                    txtChar6b.Enabled = false;
                    txtChar6c.Enabled = false;
                    txtChar6d.Enabled = false;
                    txtChar6e.Enabled = false;

                    txtChar6a.Enabled = true;
                    txtChar6b.Enabled = true;
                    txtChar6c.Enabled = true;
                    txtChar6d.Enabled = true;
                    txtChar6e.Enabled = true;
                    standardGameOver();
                }
                else
                {
                    updateColors();
                    txtInstructions.Text = words.Count.ToString() + " possible words remaining";
                }
            }
            catch
            {
                btnGuess.Enabled = false;
                txtInstructions.Text = "There are no possible remaining words.";
            }
        }

        private void updateLists()
        {
            yellowLetters.Clear();
            yellowPlaces.Clear();
            greyLetters.Clear();
            // Updates Grey Letters
            switch (guesses)
            {
                case 5:
                    if (ImageEquals(bg1a.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar1a.Text);
                    if (ImageEquals(bg1b.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar1b.Text);
                    if (ImageEquals(bg1c.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar1c.Text);
                    if (ImageEquals(bg1d.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar1d.Text);
                    if (ImageEquals(bg1e.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar1e.Text);
                    break;
                case 4:
                    if (ImageEquals(bg2a.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar2a.Text);
                    if (ImageEquals(bg2b.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar2b.Text);
                    if (ImageEquals(bg2c.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar2c.Text);
                    if (ImageEquals(bg2d.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar2d.Text);
                    if (ImageEquals(bg2e.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar2e.Text);
                    break;
                case 3:
                    if (ImageEquals(bg3a.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar3a.Text);
                    if (ImageEquals(bg3b.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar3b.Text);
                    if (ImageEquals(bg3c.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar3c.Text);
                    if (ImageEquals(bg3d.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar3d.Text);
                    if (ImageEquals(bg3e.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar3e.Text);
                    break;
                case 2:
                    if (ImageEquals(bg4a.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar4a.Text);
                    if (ImageEquals(bg4b.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar4b.Text);
                    if (ImageEquals(bg4c.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar4c.Text);
                    if (ImageEquals(bg4d.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar4d.Text);
                    if (ImageEquals(bg4e.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar4e.Text);
                    break;
                case 1:
                    if (ImageEquals(bg5a.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar5a.Text);
                    if (ImageEquals(bg5b.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar5b.Text);
                    if (ImageEquals(bg5c.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar5c.Text);
                    if (ImageEquals(bg5d.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar5d.Text);
                    if (ImageEquals(bg5e.BackgroundImage, Properties.Resources.grey)) greyLetters.Add(txtChar5e.Text);
                    break;
            }

            // Updates Yellow Letters
            switch (guesses)
            {
                case 5:
                    if (ImageEquals(bg1a.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar1a.Text);
                        yellowPlaces.Add(0);
                    }
                    if (ImageEquals(bg1b.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar1b.Text);
                        yellowPlaces.Add(1);
                    }
                    if (ImageEquals(bg1c.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar1c.Text);
                        yellowPlaces.Add(2);
                    }
                    if (ImageEquals(bg1d.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar1d.Text);
                        yellowPlaces.Add(3);
                    }
                    if (ImageEquals(bg1e.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar1e.Text);
                        yellowPlaces.Add(4);
                    }
                    break;
                case 4:
                    if (ImageEquals(bg2a.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar2a.Text);
                        yellowPlaces.Add(0);
                    }
                    if (ImageEquals(bg2b.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar2b.Text);
                        yellowPlaces.Add(1);
                    }
                    if (ImageEquals(bg2c.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar2c.Text);
                        yellowPlaces.Add(2);
                    }
                    if (ImageEquals(bg2d.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar2d.Text);
                        yellowPlaces.Add(3);
                    }
                    if (ImageEquals(bg2e.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar2e.Text);
                        yellowPlaces.Add(4);
                    }
                    break;
                case 3:
                    if (ImageEquals(bg3a.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar3a.Text);
                        yellowPlaces.Add(0);
                    }
                    if (ImageEquals(bg3b.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar3b.Text);
                        yellowPlaces.Add(1);
                    }
                    if (ImageEquals(bg3c.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar3c.Text);
                        yellowPlaces.Add(2);
                    }
                    if (ImageEquals(bg3d.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar3d.Text);
                        yellowPlaces.Add(3);
                    }
                    if (ImageEquals(bg3e.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar3e.Text);
                        yellowPlaces.Add(4);
                    }
                    break;
                case 2:
                    if (ImageEquals(bg4a.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar4a.Text);
                        yellowPlaces.Add(0);
                    }
                    if (ImageEquals(bg4b.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar4b.Text);
                        yellowPlaces.Add(1);
                    }
                    if (ImageEquals(bg4c.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar4c.Text);
                        yellowPlaces.Add(2);
                    }
                    if (ImageEquals(bg4d.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar4d.Text);
                        yellowPlaces.Add(3);
                    }
                    if (ImageEquals(bg4e.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar4e.Text);
                        yellowPlaces.Add(4);
                    }
                    break;
                case 1:
                    if (ImageEquals(bg5a.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar5a.Text);
                        yellowPlaces.Add(0);
                    }
                    if (ImageEquals(bg5b.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar5b.Text);
                        yellowPlaces.Add(1);
                    }
                    if (ImageEquals(bg5c.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar5c.Text);
                        yellowPlaces.Add(2);
                    }
                    if (ImageEquals(bg5d.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar5d.Text);
                        yellowPlaces.Add(3);
                    }
                    if (ImageEquals(bg5e.BackgroundImage, Properties.Resources.yello))
                    {
                        yellowLetters.Add(txtChar5e.Text);
                        yellowPlaces.Add(4);
                    }
                    break;
            }

            // Updates Green Letters
            switch (guesses)
            {
                case 5:
                    if (ImageEquals(bg1a.BackgroundImage, Properties.Resources.green)) greenLetters[0] = guess[0].ToString();
                    if (ImageEquals(bg1b.BackgroundImage, Properties.Resources.green)) greenLetters[1] = guess[1].ToString();
                    if (ImageEquals(bg1c.BackgroundImage, Properties.Resources.green)) greenLetters[2] = guess[2].ToString();
                    if (ImageEquals(bg1d.BackgroundImage, Properties.Resources.green)) greenLetters[3] = guess[3].ToString();
                    if (ImageEquals(bg1e.BackgroundImage, Properties.Resources.green)) greenLetters[4] = guess[4].ToString();
                    break;
                case 4:
                    if (ImageEquals(bg2a.BackgroundImage, Properties.Resources.green)) greenLetters[0] = guess[0].ToString();
                    if (ImageEquals(bg2b.BackgroundImage, Properties.Resources.green)) greenLetters[1] = guess[1].ToString();
                    if (ImageEquals(bg2c.BackgroundImage, Properties.Resources.green)) greenLetters[2] = guess[2].ToString();
                    if (ImageEquals(bg2d.BackgroundImage, Properties.Resources.green)) greenLetters[3] = guess[3].ToString();
                    if (ImageEquals(bg2e.BackgroundImage, Properties.Resources.green)) greenLetters[4] = guess[4].ToString();
                    break;
                case 3:
                    if (ImageEquals(bg3a.BackgroundImage, Properties.Resources.green)) greenLetters[0] = guess[0].ToString();
                    if (ImageEquals(bg3b.BackgroundImage, Properties.Resources.green)) greenLetters[1] = guess[1].ToString();
                    if (ImageEquals(bg3c.BackgroundImage, Properties.Resources.green)) greenLetters[2] = guess[2].ToString();
                    if (ImageEquals(bg3d.BackgroundImage, Properties.Resources.green)) greenLetters[3] = guess[3].ToString();
                    if (ImageEquals(bg3e.BackgroundImage, Properties.Resources.green)) greenLetters[4] = guess[4].ToString();
                    break;
                case 2:
                    if (ImageEquals(bg4a.BackgroundImage, Properties.Resources.green)) greenLetters[0] = guess[0].ToString();
                    if (ImageEquals(bg4b.BackgroundImage, Properties.Resources.green)) greenLetters[1] = guess[1].ToString();
                    if (ImageEquals(bg4c.BackgroundImage, Properties.Resources.green)) greenLetters[2] = guess[2].ToString();
                    if (ImageEquals(bg4d.BackgroundImage, Properties.Resources.green)) greenLetters[3] = guess[3].ToString();
                    if (ImageEquals(bg4e.BackgroundImage, Properties.Resources.green)) greenLetters[4] = guess[4].ToString();
                    break;
                case 1:
                    if (ImageEquals(bg5a.BackgroundImage, Properties.Resources.green)) greenLetters[0] = guess[0].ToString();
                    if (ImageEquals(bg5b.BackgroundImage, Properties.Resources.green)) greenLetters[1] = guess[1].ToString();
                    if (ImageEquals(bg5c.BackgroundImage, Properties.Resources.green)) greenLetters[2] = guess[2].ToString();
                    if (ImageEquals(bg5d.BackgroundImage, Properties.Resources.green)) greenLetters[3] = guess[3].ToString();
                    if (ImageEquals(bg5e.BackgroundImage, Properties.Resources.green)) greenLetters[4] = guess[4].ToString();
                    break;
            }
        }

        protected void updateColors()
        {
            // Color bottom squares grey
            switch (guesses)
            {
                case 1:
                    bg6a.BackgroundImage = Properties.Resources.grey;
                    bg6b.BackgroundImage = Properties.Resources.grey;
                    bg6c.BackgroundImage = Properties.Resources.grey;
                    bg6d.BackgroundImage = Properties.Resources.grey;
                    bg6e.BackgroundImage = Properties.Resources.grey;

                    bg5a.Enabled = false;
                    bg5b.Enabled = false;
                    bg5c.Enabled = false;
                    bg5d.Enabled = false;
                    bg5e.Enabled = false;

                    txtChar6a.BackgroundImage = Properties.Resources.grey;
                    txtChar6b.BackgroundImage = Properties.Resources.grey;
                    txtChar6c.BackgroundImage = Properties.Resources.grey;
                    txtChar6d.BackgroundImage = Properties.Resources.grey;
                    txtChar6e.BackgroundImage = Properties.Resources.grey;

                    txtChar5a.Enabled = false;
                    txtChar5b.Enabled = false;
                    txtChar5c.Enabled = false;
                    txtChar5d.Enabled = false;
                    txtChar5e.Enabled = false;
                    break;
                case 2:
                    bg5a.BackgroundImage = Properties.Resources.grey;
                    bg5b.BackgroundImage = Properties.Resources.grey;
                    bg5c.BackgroundImage = Properties.Resources.grey;
                    bg5d.BackgroundImage = Properties.Resources.grey;
                    bg5e.BackgroundImage = Properties.Resources.grey;

                    bg4a.Enabled = false;
                    bg4b.Enabled = false;
                    bg4c.Enabled = false;
                    bg4d.Enabled = false;
                    bg4e.Enabled = false;

                    txtChar5a.BackgroundImage = Properties.Resources.grey;
                    txtChar5b.BackgroundImage = Properties.Resources.grey;
                    txtChar5c.BackgroundImage = Properties.Resources.grey;
                    txtChar5d.BackgroundImage = Properties.Resources.grey;
                    txtChar5e.BackgroundImage = Properties.Resources.grey;

                    txtChar4a.Enabled = false;
                    txtChar4b.Enabled = false;
                    txtChar4c.Enabled = false;
                    txtChar4d.Enabled = false;
                    txtChar4e.Enabled = false;
                    break;
                case 3:
                    bg4a.BackgroundImage = Properties.Resources.grey;
                    bg4b.BackgroundImage = Properties.Resources.grey;
                    bg4c.BackgroundImage = Properties.Resources.grey;
                    bg4d.BackgroundImage = Properties.Resources.grey;
                    bg4e.BackgroundImage = Properties.Resources.grey;

                    bg3a.Enabled = false;
                    bg3b.Enabled = false;
                    bg3c.Enabled = false;
                    bg3d.Enabled = false;
                    bg3e.Enabled = false;

                    txtChar4a.BackgroundImage = Properties.Resources.grey;
                    txtChar4b.BackgroundImage = Properties.Resources.grey;
                    txtChar4c.BackgroundImage = Properties.Resources.grey;
                    txtChar4d.BackgroundImage = Properties.Resources.grey;
                    txtChar4e.BackgroundImage = Properties.Resources.grey;

                    txtChar3a.Enabled = false;
                    txtChar3b.Enabled = false;
                    txtChar3c.Enabled = false;
                    txtChar3d.Enabled = false;
                    txtChar3e.Enabled = false;
                    break;
                case 4:
                    bg3a.BackgroundImage = Properties.Resources.grey;
                    bg3b.BackgroundImage = Properties.Resources.grey;
                    bg3c.BackgroundImage = Properties.Resources.grey;
                    bg3d.BackgroundImage = Properties.Resources.grey;
                    bg3e.BackgroundImage = Properties.Resources.grey;

                    bg2a.Enabled = false;
                    bg2b.Enabled = false;
                    bg2c.Enabled = false;
                    bg2d.Enabled = false;
                    bg2e.Enabled = false;

                    txtChar3a.BackgroundImage = Properties.Resources.grey;
                    txtChar3b.BackgroundImage = Properties.Resources.grey;
                    txtChar3c.BackgroundImage = Properties.Resources.grey;
                    txtChar3d.BackgroundImage = Properties.Resources.grey;
                    txtChar3e.BackgroundImage = Properties.Resources.grey;

                    txtChar2a.Enabled = false;
                    txtChar2b.Enabled = false;
                    txtChar2c.Enabled = false;
                    txtChar2d.Enabled = false;
                    txtChar2e.Enabled = false;
                    break;
                case 5:
                    bg2a.BackgroundImage = Properties.Resources.grey;
                    bg2b.BackgroundImage = Properties.Resources.grey;
                    bg2c.BackgroundImage = Properties.Resources.grey;
                    bg2d.BackgroundImage = Properties.Resources.grey;
                    bg2e.BackgroundImage = Properties.Resources.grey;

                    txtChar2a.BackgroundImage = Properties.Resources.grey;
                    txtChar2b.BackgroundImage = Properties.Resources.grey;
                    txtChar2c.BackgroundImage = Properties.Resources.grey;
                    txtChar2d.BackgroundImage = Properties.Resources.grey;
                    txtChar2e.BackgroundImage = Properties.Resources.grey;

                    bg1a.Enabled = false;
                    bg1b.Enabled = false;
                    bg1c.Enabled = false;
                    bg1d.Enabled = false;
                    bg1e.Enabled = false;

                    txtChar1a.Enabled = false;
                    txtChar1b.Enabled = false;
                    txtChar1c.Enabled = false;
                    txtChar1d.Enabled = false;
                    txtChar1e.Enabled = false;
                    break;
                case 6:
                    bg1a.BackgroundImage = Properties.Resources.grey;
                    bg1b.BackgroundImage = Properties.Resources.grey;
                    bg1c.BackgroundImage = Properties.Resources.grey;
                    bg1d.BackgroundImage = Properties.Resources.grey;
                    bg1e.BackgroundImage = Properties.Resources.grey;

                    txtChar1a.BackgroundImage = Properties.Resources.grey;
                    txtChar1b.BackgroundImage = Properties.Resources.grey;
                    txtChar1c.BackgroundImage = Properties.Resources.grey;
                    txtChar1d.BackgroundImage = Properties.Resources.grey;
                    txtChar1e.BackgroundImage = Properties.Resources.grey;
                    break;

            }
        }

        internal void Startup()
        {
            txtInstructions.Text = "Loading... Please Wait";
            string dictionary = Properties.Resources.dictionary;
            string[] allWords = dictionary.Split(',');

            foreach (string word in allWords)
            {
                if (word.Length == 5 && !word.Contains(" ")) words.Add(word);
            }
            txtInstructions.Text = words.Count.ToString() + " possible words remaining";
            txtStreak.Text = streak.ToString();
            guesses = 6;
            newGuess();
            guesses--;
        }

        internal void standardGameOver()
        {
            btnGuess.Enabled = false;
            if (ImageEquals(bg6a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg6b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg6c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg6d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg6e.BackgroundImage, Properties.Resources.green))
            {
                txtInstructions.Text = "Yay! I got it!";
                bg6a.Enabled = false;
                bg6b.Enabled = false;
                bg6c.Enabled = false;
                bg6d.Enabled = false;
                bg6e.Enabled = false;
                txtChar6a.Enabled = false;
                txtChar6b.Enabled = false;
                txtChar6c.Enabled = false;
                txtChar6d.Enabled = false;
                txtChar6e.Enabled = false;
                streak++;
                txtStreak.Text = streak.ToString();
            }
            else
            {
                txtInstructions.Text = "I lost! There were " + words.Count.ToString() + " possible words.";
                streak = 0;
            }
        }

        internal bool checkEarlyWin()
        {
            bool win = false;
            switch (guesses)
            {
                case 5:
                    if (ImageEquals(bg1a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg1b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg1c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg1d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg1e.BackgroundImage, Properties.Resources.green))
                        win = true;
                    break;
                case 4:
                    if (ImageEquals(bg2a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg2b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg2c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg2d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg2e.BackgroundImage, Properties.Resources.green))
                        win = true;
                    break;
                case 3:
                    if (ImageEquals(bg3a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg3b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg3c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg3d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg3e.BackgroundImage, Properties.Resources.green))
                        win = true;
                    break;
                case 2:
                    if (ImageEquals(bg4a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg4b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg4c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg4d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg4e.BackgroundImage, Properties.Resources.green))
                        win = true;
                    break;
                case 1:
                    if (ImageEquals(bg5a.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg5b.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg5c.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg5d.BackgroundImage, Properties.Resources.green) &&
                ImageEquals(bg5e.BackgroundImage, Properties.Resources.green))
                        win = true;
                    break;
            }

            if (win)
            {
                txtInstructions.Text = "Yay! I win! " +
                    "There were " + (words.Count - 1).ToString() + " possible words.";
                btnGuess.Enabled = false;
                streak++;
                txtStreak.Text = streak.ToString();
            }

            return win;
        }

        private void changer(PictureBox bg, Label txt)
        {
            if (ImageEquals(bg.BackgroundImage, Properties.Resources.grey))
            {
                bg.BackgroundImage = Properties.Resources.yello;
                txt.BackgroundImage = Properties.Resources.yello;
            }
            else if (ImageEquals(bg.BackgroundImage, Properties.Resources.yello))
            {
                bg.BackgroundImage = Properties.Resources.green;
                txt.BackgroundImage = Properties.Resources.green;
            }
            else if (ImageEquals(bg.BackgroundImage, Properties.Resources.green))
            {
                bg.BackgroundImage = Properties.Resources.grey;
                txt.BackgroundImage = Properties.Resources.grey;
            }
        }

        private bool ImageEquals(Image image1, Image image2)
        {
            using (MemoryStream ms1 = new MemoryStream())
            using (MemoryStream ms2 = new MemoryStream())
                if (image1 != null)
                {
                    image1.Save(ms1, image1.RawFormat);
                    image2.Save(ms2, image2.RawFormat);

                    byte[] imageBytes1 = ms1.ToArray();
                    byte[] imageBytes2 = ms2.ToArray();

                    return imageBytes1.SequenceEqual(imageBytes2);
                }
                else return false;
        }


    }


}
