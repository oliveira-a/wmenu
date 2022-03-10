// ****************************************************************************
// <copyright file="MainForm.cs">
//    Copyright © Andre Brasil 2022 <brasil.a@pm.me>
// </copyright>
// ****************************************************************************

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
        private readonly Color _backColour = Color.FromArgb(68, 68, 68);
        private readonly Color _foregroundColour = Color.White;
        private readonly Font _font = new Font("Lucida Console", 10f);
        private SortedSet<App> _apps = new SortedSet<App>();
        private KeyHandler _ghk;

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
            this.Location = new Point(0, 0);
            this.Padding = Padding.Empty;
            this.BackColor = _backColour;

            inputTxtBox.Width = width / 7;
            inputTxtBox.Height = height;
            inputTxtBox.Location = GetNewLocation(inputTxtBox);
            inputTxtBox.Padding = Padding.Empty;
            inputTxtBox.Margin = Padding.Empty;
            inputTxtBox.BackColor = _backColour;
            inputTxtBox.ForeColor = _foregroundColour;
            inputTxtBox.Font = _font;
            inputTxtBox.BorderStyle = BorderStyle.None;
            AdjustTextBoxHeight(ref inputTxtBox);

            lblPrograms.Text = string.Empty;
            lblPrograms.ForeColor = _foregroundColour;
            lblPrograms.Padding = Padding.Empty;
            lblPrograms.Margin = Padding.Empty;
            lblPrograms.Location = GetNewLocation(lblPrograms);
            lblPrograms.Font = _font;
            lblPrograms.Width = width;
            lblPrograms.Height = height;

            // Create a new KeyHandler to open wmenu with a hotkey
            _ghk = new KeyHandler(Keys.Oem3, this);
            if (!_ghk.Register())
            {
                throw new Exception("There was a problem registering the hotkey.");
            }

            SetupIconTray();
            LoadPrograms();
            DisplayBestMatch();
        }

        private void SetupIconTray()
        {
            var menuItems = new MenuItem[]
            {
                new MenuItem("Launch app", delegate(object s, EventArgs e) { this.Show(); }),
                new MenuItem("Quit wmenu", delegate(object s, EventArgs e) { this.Close(); }),
            };
            trayIcon.Text = "wmenu";
            trayIcon.ContextMenu = new ContextMenu(menuItems);
        }

        private Point GetNewLocation(Control c)
        {
            return Screen.FromControl(this).Bounds.Height <= 992 ? new Point(c.Location.X, this.Location.Y - 1) : c.Location;
        }

        private void AdjustTextBoxHeight(ref TextBox c)
        {
            if (Screen.FromControl(this).Bounds.Height > 992)
            {
                c.Location = new Point(c.Location.X, (Screen.FromControl(this).Bounds.Height / 50) / 4);
            }
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
                        if (!string.IsNullOrEmpty(path))
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
                MinimizeToTray();
                return;
            }

            MinimizeToTray();
        }

        private void inputTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) MinimizeToTray();
            if (e.KeyCode == Keys.Enter) RunProgram();
        }

        private bool CheckProgramAlreadyRunning()
        {
            return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            MinimizeToTray();
        }

        private void MinimizeToTray()
        {
            /* Make sure input box is clear when hidding. */
            inputTxtBox.Clear();
            this.Hide();
        }
        
        /*
         * Listenning on windows messaging system.
         * See https://docs.microsoft.com/en-us/windows/win32/winmsg/about-messages-and-message-queues
        */
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == KeyHandler.WM_HOTKEY_MS_ID)
            {
                if (!this.Visible) { this.Show(); }
            }
            base.WndProc(ref m);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            MinimizeToTray();
        }
    }
}
