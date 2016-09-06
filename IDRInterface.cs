using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HubStudioSwitcher
{
    class IDRInterface
    {
        bool stayConnected = true;
        Int32 port = 23;
        String server = "";
        String password = "";
        TcpClient client;
        NetworkStream stream;
        Byte[] dataIn = new Byte[256];
        Byte[] dataOut = new Byte[256];
        String textIn = String.Empty;
        Int32 bytesIn = 0;

        public IDRInterface()
        {
            var appSettings = ConfigurationManager.AppSettings;

            try {
                server = appSettings["idrServer"];
                port = Int32.Parse(appSettings["idrPort"]);
                password = appSettings["idrPassword"];
            } 
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        public bool Connect()
        {
            client = new TcpClient(server, port);
            stream = client.GetStream();

            stayConnected = true;
            try {
                while (stayConnected)
                {
                    bytesIn = stream.Read(dataIn, 0, dataIn.Length);
                    textIn = System.Text.Encoding.ASCII.GetString(dataIn, 0, bytesIn);
                    textIn = textIn.Replace("\r", string.Empty);
                    textIn = textIn.Replace("\n", string.Empty);
                    textIn = textIn.Replace("\0", string.Empty);
                    Console.WriteLine("{0}", textIn);

                    // Respond based on what we receive from the IDR
                    switch (textIn)
                    {
                        case "Connected to iDR":
                            Console.WriteLine("We're seeing an IDR");
                            break;
                        case "Password: ":
                        case "Connected to iDRPassword":
                            Console.WriteLine("Authenticating...");
                            dataOut = System.Text.Encoding.ASCII.GetBytes(password + "\r");
                            stream.Write(dataOut, 0, dataOut.Length);
                            break;
                        case "iDR4>":
                            // Ready to do some real stuff now!
                            Console.WriteLine("Let's go!");
                            return true;

                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Unable to read settings. Expect things to fail.");
                return false;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return false;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return false;
            }

            return false;
        }

        public void Disconnect()
        {
            try
            {
                stream.Close();
                client.Close();
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Unable to read settings. Expect things to fail.");
                return;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
                return;
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                return;
            }
        }

        public String SendCommand(String command)
        {
            String[] args = command.Split(' ');
            switch (args[0].ToUpper())
            {
                case "GET":
                    switch (args[1].ToUpper())
                    {
                        case "PRESET":
                            Console.WriteLine("Getting current preset");
                            dataOut = System.Text.Encoding.ASCII.GetBytes("GET PRESET\r");
                            stream.Write(dataOut, 0, dataOut.Length);

                            do
                            {
                                bytesIn = stream.Read(dataIn, 0, dataIn.Length);
                                textIn = System.Text.Encoding.ASCII.GetString(dataIn, 0, bytesIn);
                                textIn = textIn.Replace("\0", string.Empty);
                                textIn = textIn.Replace("\n", string.Empty);
                                textIn = textIn.Replace("\r", string.Empty);
                                textIn = textIn.Replace("iDR4>", string.Empty);
                            } while (String.Compare(textIn, 0, "", 0, 2) == 0);
                            Console.WriteLine("Currently on preset {0}", textIn);
                            return textIn;
                            
                    }

                    break;
                case "SET":
                    switch (args[1].ToUpper())
                    {
                        case "PRESET":
                            string preset = (args.Length >= 3) ? args[2] : "1";

                            Console.WriteLine("Setting current preset to {0}", preset);
                            dataOut = System.Text.Encoding.ASCII.GetBytes("SET PRESET " + preset + "\r");
                            stream.Write(dataOut, 0, dataOut.Length);

                            // Verify our actions
                            dataOut = System.Text.Encoding.ASCII.GetBytes("GET PRESET\r");
                            stream.Write(dataOut, 0, dataOut.Length);

                            do
                            {
                                bytesIn = stream.Read(dataIn, 0, dataIn.Length);
                                textIn = System.Text.Encoding.ASCII.GetString(dataIn, 0, bytesIn);
                                textIn = textIn.Replace("\0", string.Empty);
                                textIn = textIn.Replace("iDR4>", string.Empty);
                            } while (String.Compare(textIn, 0, "", 0, 2) == 0);


                            // Did it work?

                            if (string.Compare(preset, textIn) == 0)
                            {
                                Console.WriteLine("Successfully set");
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Not set successfully.");
                            }
                            return textIn;
                    }
                    return "";
            }
            return "";
        }

    }
}
