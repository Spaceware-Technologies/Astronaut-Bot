using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Text.Encodings.Web;
using System.IO;
using System.Threading;
using USpeak;
using System.Diagnostics;

namespace Astronaut
{
    public class BotHandler
    {
        #region Global Variables

        //public

        public static Bot bot = new Bot();

        //Strings
        public static string photon_server { get; set; }
        public static string x_client_version { get; set; }
        public static string userPass { get; set; }
        public static string region { get; set; }
        public static string worldid { get; set; }       
        public static string AstronautLogo { get; set; }
        public static string UID { get; set; }
        public static string POS { get; set; }
        public static string AID { get; set; }
        public static bool CrashNLeave { get; set; }


        //bools
        public static bool IsLogingAvatars { get; set; }
        public static bool IsEventNineCrash { get; set; }

        //Private 

        //strings
        private static string Cmd { get; set; }
        #endregion


        public static void DisplayLogo()
        {
            
            Console.Title = $"Area 51 Photonbot *Astronaut* | OuterSpace {Environment.Version} | Fish, Silly & Josh";
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("========================================================================================================================");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("                        █████╗ ███████╗████████╗██████╗  ██████╗ ███╗   ██╗ █████╗ ██╗   ██╗████████╗                   ");
            Console.WriteLine("                       ██╔══██╗██╔════╝╚══██╔══╝██╔══██╗██╔═══██╗████╗  ██║██╔══██╗██║   ██║╚══██╔══╝                   ");
            Console.WriteLine("                       ███████║███████╗   ██║   ██████╔╝██║   ██║██╔██╗ ██║███████║██║   ██║   ██║                      ");
            Console.WriteLine("                       ██╔══██║╚════██║   ██║   ██╔══██╗██║   ██║██║╚██╗██║██╔══██║██║   ██║   ██║                      ");
            Console.WriteLine("                       ██║  ██║███████║   ██║   ██║  ██║╚██████╔╝██║ ╚████║██║  ██║╚██████╔╝   ██║                      ");
            Console.WriteLine("                        ═╝  ╚═╝╚══════╝   ╚═╝   ╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝╚═╝  ╚═╝ ╚═════╝    ╚═╝                      ");
            Console.WriteLine("                                I OR SOMEONE I'M CHILL WITH IS COOL WITH YOU, Congrats!                                 ");
            Console.WriteLine("                                              Here's A Fucking Cookie *                                                 ");
            Console.WriteLine("                                         The Developers's: Fish, Josh, Silly                                            ");
            Console.WriteLine($"                                          Bot Version {Environment.Version}                                            ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("========================================================================================================================\n");
            Console.ForegroundColor = ConsoleColor.White;

        }

        private static bool Check(string argument)
        {
            string[] arguments = argument.Split('|');
            if (arguments.Length > 1) // In your case if you are expecting 2 or more arguments
            {
                userPass = arguments[0];
                region = arguments[1];
                worldid = arguments[2];
                AID = arguments[3];
                UID = arguments[4];
                POS = arguments[5];
                _BotHandler(arguments[6]);
                Console.WriteLine(arguments[6]);
                return true;
            }
            return false;
        }


        public static void Main(string[] args)
        {
            foreach (var arg in args)
            {
                Check(arg);
                if (!Check(arg))
                {
                    PhotonClient.Debuglog("Something Went Wrong");
                }
            }
        }

        public static string _BotHandler(string command)
        {
            using (var wc = new WebClient())
            {
                JObject config = JObject.Parse(wc.DownloadString("https://api.ripper.store/config"));
                photon_server = config["photon_server"].ToString();
                x_client_version = config["x_client_version"].ToString();

                switch (command)
                {                   
                    case "JoinWorld":
                        bot.BotLogin(userPass, region);
                        DisplayLogo();
                        System.Threading.Thread.Sleep(5000);
                        //  IsEventNineCrash = true;
                        PhotonClient.Debuglog($"Joining World[{region}]: " + worldid);
                        bot.JoinRoom(worldid);                     
                        Console.Read();
                        return $"Joining World: {worldid}";
                    case "AvatarCrash":
                        bot.BotLogin(userPass, region);
                        DisplayLogo();
                        System.Threading.Thread.Sleep(5000);
                        PhotonClient.Debuglog($"Joining World[{region}]: " + worldid);
                        bot.JoinRoom(worldid);
                        CrashNLeave = true;
                        Console.Read();
                        return $"Joining World: {worldid}";
                    case "KillWorld":
                        bot.BotLogin(userPass, region);
                        DisplayLogo();                  
                        System.Threading.Thread.Sleep(5000);
                        PhotonClient.Debuglog($"Joining World[{region}]: " + worldid);
                        bot.JoinRoom(worldid);
                        IsEventNineCrash = true;
                        Console.Read();
                        return $"Joining World: {worldid}";        
                    default:                
                        return "N/A";
                }
            }
        }



    }
}
