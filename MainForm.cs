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
using System.Runtime.InteropServices;


namespace HubStudioSwitcher
{

    public partial class MainForm : Form
    {

        private StudioOption[] studios;
        private Button[] studioButtons;
        int numberOfStudios;
        IDRInterface idr;


        // Stay on top of all windows Win32API
        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        public MainForm()
        {
            InitializeComponent();
            SetWindowParameters();
            DisplaySwitchOptions(ref studios, ref studioButtons, ref numberOfStudios);
            ResetButtonStyles(studioButtons, numberOfStudios);

            // Init to the IDR. If IDR connection fails, warning already generated, close out.
            if(!InitIDR())
            {
                this.Close();
            }
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
                // Set some default variables just in case.
                x = 0;
                y = 0;
                w = 800;
                h = 250;
            }

            this.Location = new Point(x, y);
            this.Size = new Size(w, h);

            

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

            // Get the options out of the config file
            do
            {
                string setting = "option" + (numberOfOptions + 1).ToString();
                try
                {
                    string name = appSettings[setting + "Name"];
                    string cmd = appSettings[setting + "Cmd"];

                    // Check not blank
                    if (name != null && cmd != null)
                    {
                        int preset = int.Parse(appSettings[setting + "Preset"]);
                        studios[numberOfOptions] = new StudioOption(name, cmd, preset);
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

            // Generate the buttons
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
                studioButtons[i].ForeColor = Color.White;
            }
        }

        private void SetButtonOnStyle(int buttonId)
        {
            Button button = studioButtons[buttonId];
            button.BackColor = Color.Green;
            button.ForeColor = Color.White;
        }

        private void SetButtonOnStyleByPreset(int preset)
        {
            for(int i = 0; i < numberOfStudios; i++)
            {
                if(studios[i].GetPreset() == preset)
                {
                    SetButtonOnStyle(i);
                    i += numberOfStudios;
                }
            }
        }

        private bool InitIDR()
        {
            idr = new IDRInterface();
            if(idr.Connect())
            {
                String result = idr.SendCommand("GET PRESET");
                int preset = int.Parse(result);
                if(preset > 0)
                {
                    ResetButtonStyles(studioButtons, numberOfStudios);
                    SetButtonOnStyleByPreset(preset);
                }
            }
            else
            {
                MessageBox.Show("Unable to connect to the IDR interface. Please check network is connected, and the IDR is on and connected to network. Switcher will now exit.",
                                "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
            idrCheckTimer.Enabled = true;
            return true;

        }
        private void idrCheckTimer_Tick(object sender, EventArgs e)
        {
            String result = idr.SendCommand("GET PRESET");
            int preset = int.Parse(result);
            if (preset > 0)
            {
                ResetButtonStyles(studioButtons, numberOfStudios);
                SetButtonOnStyleByPreset(preset);
            }
        }

        private void buttonSwitchSendCommand(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            int buttonId = int.Parse(button.Tag.ToString());

            ResetButtonStyles(this.studioButtons, this.numberOfStudios);
            SetButtonOnStyle(buttonId);

            idr.SendCommand(studios[buttonId].GetCommand());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            idr.Disconnect();
            this.Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Force on top of everything!
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        }


    }


}
