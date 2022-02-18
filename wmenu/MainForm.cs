using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace wmenu
{
    public partial class MainForm : Form
    {
        private readonly Color _backColour = Color.FromArgb(68,68,68);
        private readonly Color _foregroundColour = Color.White;
        private readonly Font _font = new Font("Arial", 10f);
        private SortedSet<App> _apps = new SortedSet<App>();

        public MainForm()
        {
            if (CheckProgramAlreadyRunning())
            {
                this.Close();
            }
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int width = Screen.FromControl(this).Bounds.Width;
            int height = Screen.FromControl(this).Bounds.Height / 50;

            this.Width = width;
            this.Height = height;
            this.Location = new Point(0,0);
            this.Padding = Padding.Empty;
            this.BackColor = _backColour;

            inputTxtBox.Width = width / 7;
            inputTxtBox.Height = height;
            inputTxtBox.Location = new Point(0, height/4);
            inputTxtBox.Padding = Padding.Empty;
            inputTxtBox.Margin = Padding.Empty;
            inputTxtBox.BackColor = _backColour;
            inputTxtBox.ForeColor = _foregroundColour;
            inputTxtBox.Font = _font;
            inputTxtBox.BorderStyle = BorderStyle.None;

            lblPrograms.Text = string.Empty;
            lblPrograms.ForeColor = _foregroundColour;
            lblPrograms.Font = _font;
            lblPrograms.Width = width;
            lblPrograms.Height = height;

            LoadPrograms();
            DisplayBestMatch();
        }

        private void DisplayBestMatch(string input = "")
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input))
            {
                foreach (var p in _apps)
                    sb.Append(p.name + " ");
            } else
            {
                _apps.Where(x => x.name.ToLower().StartsWith(input.ToLower()) || x.name.ToLower() == input.ToLower())
                    .ToList()
                    .ForEach(i => sb.Append(i.name + " "));
            }

            // Find the best matches according to the input
            lblPrograms.Text = sb.ToString();
        }

        private void inputTxtBox_TextChanged(object sender, EventArgs e)
        {
            DisplayBestMatch(inputTxtBox.Text);
        }

        private void LoadPrograms()
        {
            string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
            string appPathsReg = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(appPathsReg))
            {
                foreach (string skName in key.GetSubKeyNames())
                {
                    using (RegistryKey sk = key.OpenSubKey(skName))
                    {
                        string path = ((string)sk.GetValue(string.Empty))?.Trim();
                        if(!string.IsNullOrEmpty(path))
                        {
                            _apps.Add(new App()
                            {
                                name = Path.ChangeExtension(skName, string.Empty).TrimEnd('.'),
                                path = path
                            });
                        }
                    }
                }
            }

            // Loading programs from user's path as well.
            foreach (var path in paths)
            {
                try
                {
                    string[] programs = Directory.GetFiles(path, "*.exe");
                    foreach (var p in programs)
                    {
                        _apps.Add(new App()
                        {
                            name = Path.ChangeExtension(Path.GetFileName(p), string.Empty).TrimEnd('.'),
                            path = p,
                        });
                    }
                } catch
                {
                    continue;
                }
            }

        }

        private void RunProgram()
        {
            if (string.IsNullOrEmpty(inputTxtBox.Text)) return;

            string[] items = lblPrograms.Text.Split(' ');
            App programToOpen = _apps.FirstOrDefault(x => x.name == items[0]);

            if (programToOpen == null) return;

            // Some programs might not run
            try
            {
                Process.Start(programToOpen.path);
            } catch
            {
                this.Close();
            }

            this.Close();
        }

        private void inputTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
            if (e.KeyCode == Keys.Enter) RunProgram();
        }

        private bool CheckProgramAlreadyRunning()
        {
            return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1; 
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
