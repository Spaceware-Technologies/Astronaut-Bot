using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Astronaut
{
    public class Bot : LoadBalancingClient
    {
        public static HttpClient Client = new HttpClient(); //Client = new HttpClient;
        public List<PhotonClient> photonclients = new List<PhotonClient>();
        public string Auth { get; set; }
        public string userid { get; set; }
        public string FakeMacId { get; set; }
        public string username { get; set; }
        public string CurrentWorldInstance { get; private set; }

        public string Token {  get; private set; }
      
        public void BotLogin(string usernamepassword, string WorldRegion)
        {
            byte[] bytes = new byte[20];
            new Random(Environment.TickCount).NextBytes(bytes);
            FakeMacId = string.Join("", bytes.Select(x => x.ToString("x2")));
            Task.Run(SetUpClientAsync(usernamepassword).GetAwaiter().GetResult).Wait();
            photonclients.Add(new PhotonClient(username, Auth, userid, WorldRegion, FakeMacId));
            System.Timers.Timer VisitPing = new System.Timers.Timer(30000)
            {
                AutoReset = true,
                Enabled = true
            };
            VisitPing.Elapsed += async delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                if (string.IsNullOrEmpty(CurrentWorldInstance)) return;
                HttpRequestMessage visitPayload = new HttpRequestMessage(HttpMethod.Put, "https://api.vrchat.cloud/api/1/visits?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
                string visitBody = JsonConvert.SerializeObject(new { userId = userid, worldId = CurrentWorldInstance });
                visitPayload.Content = new StringContent(visitBody, Encoding.UTF8, "application/json");
                HttpResponseMessage visitResp = await Client.SendAsync(visitPayload);
                string visitRespBody = await visitResp.Content.ReadAsStringAsync();
                JObject visitObjResp = JObject.Parse(visitRespBody);
                if (visitObjResp.ContainsKey("success"))
                {
                   // Console.WriteLine(visitObjResp["success"]?["message"]);
                }
            };
        }
        public void JoinRoom(string worldinstance, int cap = 0)
        {
            CurrentWorldInstance = worldinstance;
            string worldid = worldinstance.Split(':')[0];
            if (photonclients.Any(x => x.InRoom))
            {
                IEnumerable<PhotonClient> inroombots = photonclients.Where(x => x.InRoom);
                foreach (var bot in inroombots)
                {
                    bot.OpLeaveRoom(false);
                }
                Thread.Sleep(3000);
                JoinRoom(worldinstance, cap);
                return;
            }
            if (cap == 0)
            {
                cap = int.Parse(Task.Run(async () => await Get($"worlds/{worldid}")).Result["capacity"].ToString());
            }
            Task.Run(async () =>
            {
                HttpRequestMessage joinWorldPayload = new HttpRequestMessage(HttpMethod.Put, "https://api.vrchat.cloud/api/1/joins?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
                string joinWorldBody = JsonConvert.SerializeObject(new { userId = userid, worldId = CurrentWorldInstance });
                joinWorldPayload.Content = new StringContent(joinWorldBody, Encoding.UTF8, "application/json");
                HttpResponseMessage joinWorldResp = await Client.SendAsync(joinWorldPayload);
                string joinWorldRespBody = await joinWorldResp.Content.ReadAsStringAsync();
            }).Wait();
            string token = Task.Run(async () => await Get("instances/" + CurrentWorldInstance + "/join")).Result["token"].ToString();
            Token = token;
            if (token == null || cap == 0)
            {            
                return;
            }
            EnterRoomParams enterRoomParams = new EnterRoomParams
            {
                CreateIfNotExists = true,
                RejoinOnly = false,
                RoomName = worldinstance,
                RoomOptions = new RoomOptions
                {
                    IsOpen = true,
                    IsVisible = true,
                    MaxPlayers = Convert.ToByte(cap * 2),
                    CustomRoomProperties = new Hashtable
                    {
                      
                        { (byte)3, 1},
                        { (byte)2, token}
                    },
                    EmptyRoomTtl = 0,
                    DeleteNullProperties = true,
                    PublishUserId = false
                    
        }
            };
            photonclients.First().OpJoinRoom(enterRoomParams);
            BotHandler.IsEventNineCrash = true;

       
        }



        public async Task<JObject> Get(string endpoint)
        {
            string hasQuery = endpoint.IndexOf('?') != -1 ? "&" : "?";
            string body = await Client.GetStringAsync($"https://api.vrchat.cloud/api/1/{endpoint}{hasQuery}apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
            //Console.WriteLine(JObject.Parse(body).ToString());
            return JObject.Parse(body);
        }
        private async Task SetUpClientAsync(string usernamepassword)
        {         
            Client = new HttpClient(new HttpClientHandler { UseCookies = false });
            Client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            Client.DefaultRequestHeaders.Add("X-MacAddress", FakeMacId);
            Client.DefaultRequestHeaders.Add("X-Client-Version", BotHandler.x_client_version);
            Client.DefaultRequestHeaders.Add("X-Platform", "standalonewindows");
            Client.DefaultRequestHeaders.Add("Origin", "vrchat.com");
            Client.DefaultRequestHeaders.Add("Host", "api.vrchat.cloud");
            Client.DefaultRequestHeaders.Add("Connection", "Keep-Alive, TE");
            Client.DefaultRequestHeaders.Add("TE", "identity");
            Client.DefaultRequestHeaders.Add("User-Agent", "VRC.Core.BestHTTP");

            HttpRequestMessage loginPayLoad = new HttpRequestMessage(HttpMethod.Get, "https://api.vrchat.cloud/api/1/auth/user?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
            loginPayLoad.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(usernamepassword)));
            HttpResponseMessage loginResp = await Client.SendAsync(loginPayLoad);
            string loginBody = await loginResp.Content.ReadAsStringAsync();
            Client.DefaultRequestHeaders.Add("cookie", loginResp.Headers.GetValues("set-cookie"));
            Auth = loginResp.Headers.GetValues("set-cookie").First().Split('=')[1].Split(';')[0];
            userid = JObject.Parse(loginBody)["id"].ToString();
            username = JObject.Parse(loginBody)["username"].ToString();
        }


    }
}
