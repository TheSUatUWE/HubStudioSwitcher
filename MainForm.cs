using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HubStudioSwitcher
{
    public partial class MainForm : Form
    {
        private StudioOption[] studios;
        private Button[] studioButtons;
        int numberOfStudios;
        public MainForm()
        {
            InitializeComponent();
            SetWindowParameters();
            DisplaySwitchOptions(ref studios, ref studioButtons, ref numberOfStudios);
            ResetButtonStyles(studioButtons, numberOfStudios);
        }

        private void SetWindowParameters()
        {
            int x, y, w, h;
            var appSettings = ConfigurationManager.AppSettings;

            try {
                x = int.Parse(appSettings["windowX"]);
                y = int.Parse(appSettings["windowY"]);
                w = int.Parse(appSettings["windowW"]);
                h = int.Parse(appSettings["windowH"]);
            }
            catch (Exception e)
            {
                // Set some variables just in case.
                x = 0;
                y = 0;
                w = 800;
                h = 250;
            }

            this.Location = new Point(x, y);
            this.Size = new Size(w, h);

            this.TopMost = true;
            
        }

        private void DisplaySwitchOptions(ref StudioOption[] studios, ref Button[] studioButtons, ref int numberOfOptions)
        {
            int maxOptions = 9;
            bool breaker = false;
            var appSettings = ConfigurationManager.AppSettings;
            int x, y, w, h;
            int buttonW, buttonH;

            studios = new StudioOption[maxOptions];
            studioButtons = new Button[maxOptions];
            numberOfOptions = 0;

            do
            {
                string setting = "option" + (numberOfOptions + 1).ToString();
                try
                {
                    string name = appSettings[setting + "name"];
                    string cmd = appSettings[setting + "cmd"];

                    if (name != null && cmd != null)
                    {
                        studios[numberOfOptions] = new StudioOption(name, cmd);
                        numberOfOptions++;
                        if (numberOfOptions >= maxOptions) breaker = true;
                    }
                    else breaker = true;

                }
                catch (Exception e)
                {
                    breaker = true;
                }
            } while (breaker == false);

            // If we have no options, why bother?
            if (numberOfOptions <= 0) return;

            // Work out some positioning. Padding of 10px
            x = 10;
            y = 10;
            w = Size.Width - 20;
            h = Size.Height - 20;

            buttonW = w / numberOfOptions;
            buttonH = h;


            for(int i = 0; i < numberOfOptions; i++)
            {
                studioButtons[i] = new Button();
                studioButtons[i].Name = "switchButton" + i;
                studioButtons[i].Size = new Size(buttonW, buttonH);
                studioButtons[i].Location = new Point(x + (buttonW * i), y);
                studioButtons[i].Text = studios[i].GetName();
                studioButtons[i].Font = new Font(new FontFamily("Arial"), 22);
                studioButtons[i].TextAlign = ContentAlignment.MiddleCenter;
                studioButtons[i].Tag = i;
                studioButtons[i].MouseUp += new MouseEventHandler(buttonSwitchSendCommand);

                this.Controls.Add(studioButtons[i]);
            }
            return;
        }

        private void ResetButtonStyles(Button[] studioButtons, int numberOfStudios)
        {
            for(int i = 0; i < numberOfStudios; i++)
            {
                studioButtons[i].BackColor = Color.LightGray;
            }
        }

        private void buttonSwitchSendCommand(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            ResetButtonStyles(this.studioButtons, this.numberOfStudios);
            button.BackColor = Color.LightGreen;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }
    }


}
