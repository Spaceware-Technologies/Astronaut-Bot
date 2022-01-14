using Newtonsoft.Json.Linq;
using System;
using System.Net;
using Astronaut.API.BotServer;
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
        public static string Cmd { get; set; }
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
            Console.WriteLine($"                                             Bot Version {Environment.Version}                                         ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("                                                                                                                        ");
            Console.WriteLine("========================================================================================================================\n");
            Console.ForegroundColor = ConsoleColor.White;

        }

        private static bool BotController(string argument)
        {
            string[] arguments = argument.Split('|');
            if (arguments.Length > 1) 
            {
               userPass = arguments[0];
                region = arguments[1];
                worldid = arguments[2];
                AID = arguments[3];
                UID = arguments[4];
                POS = arguments[5];
                _BotHandler(arguments[6]);
                Console.WriteLine($"[{DateTime.Now.ToString("hh:mm tt")} [{PhotonClient._botName}] Starting up....\n");
                return true;
            }
            return false;
        }

        public static void Main(string[] args)
        {
           
            foreach (var arg in args)
            { 
                if (BotController(arg))
                {                  
                    BotController(arg);
                    if (!BotController(arg))
                    {
                        PhotonClient.Debuglog("Something Went Wrong");
                    }
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
                var WID = "";
                if (worldid.Contains("~")) { WID = worldid.Substring(0, worldid.IndexOf("~") + 1).Replace("~", ""); } else { WID = worldid; }
                switch (command)
                {                   
                    case "JoinWorld":
                        bot.BotLogin(userPass, region);
                        DisplayLogo();
                        System.Threading.Thread.Sleep(2500);
                        PhotonClient.Debuglog($"Joining World[{region}]: " + WID);
                        bot.JoinRoom(worldid);
                        System.Threading.Thread.Sleep(2500);                    
                        Console.Read();
                        return $"Joining World:" + WID + "\n";                  
                    default:                
                        return "N/A";
                }
            }
        }
    }
}
