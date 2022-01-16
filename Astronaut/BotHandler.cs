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
        public static string AstronautLogo { get; set; }

        //Strings
        public static string Photon_server { get; set; }
        public static string X_client_version { get; set; }

        public static string Userid { get; set; } //the person you would like to atatch to
        public static string Avatarid { get; set; } // the avatar the bot will have.
        public static string UserPass { get; set; } //Bot login Info
        public static string Region { get; set; } //Region of world you are joining EX: USW
        public static string Worldid { get; set; } // the world you wish to join.  
        public static string Distance { get; set; } //how far the bots will be from you.

        //bools
        public static bool CrashNLeave { get; set; }
        public static bool IsLogingAvatars { get; set; }
        public static bool IsEventNineCrash { get; set; }

        //Private strings
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


        public static void Main(string[] args)
        {
            foreach (var arg in args) //for each pram passed in cmd 
            {
                string[] arguments = arg.Split('|');
                if (arguments.Length > 1 && arguments != null)
                {
                    UserPass = arguments[0];
                    Region = arguments[1];
                    Worldid = arguments[2];
                    Avatarid = arguments[3];
                    Userid = arguments[4];
                    Distance = arguments[5];
                    _BotHandler(arguments[6]); //Command you wish to send to the handler.
                    Console.WriteLine($"[{DateTime.Now.ToString("hh:mm tt")} {PhotonClient._botName}] Starting up....\n");
                }
                else
                {
                    Console.WriteLine("Something Went Wrong");
                    Console.Read();
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
