
using Astronaut.API.Utils;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;


namespace Astronaut
{
    public class PhotonClient : LoadBalancingClient, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks, IPhotonPeerListener
    {
        public static string _botName { get; set; }
        public static USpeakLite _uspeak { get; set; }
        public string userID = BotHandler.Userid;
        public string AvatarID = BotHandler.Avatarid;
        public string posz = BotHandler.Distance;

        public static void Debuglog(string line)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now.ToString("hh:mm tt")} [{_botName}] {line}");
        }

        public PhotonClient(string BotName, string auth, string userid, string region, string FakeMacId)
        {
            _botName = BotName;
            _uspeak = new USpeakLite();

            Console.Title = "Astronaut 1.0 | Team GangMonkey | Time to virtually shit on some Client Kids!";
            Console.ForegroundColor = ConsoleColor.Green;
            AppId = "bf0942f7-9935-4192-b359-f092fa85bef1";
            AppVersion = BotHandler.photon_server;
            NameServerHost = "ns.exitgames.com";
            System.Timers.Timer PhotonLoop = new System.Timers.Timer(50) { Enabled = true, AutoReset = true, };
            PhotonLoop.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e) { Service(); };
            CustomTypes.Register(this);
            AuthValues = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
            AuthValues.AddAuthParameter("token", auth);
            AuthValues.AddAuthParameter("user", userid);
            AuthValues.AddAuthParameter("hwid", FakeMacId);
            AuthValues.AddAuthParameter("platform", "android");
            AddCallbackTarget(this);
            this.EventReceived += CustomOnEvent;
            if (!ConnectToRegionMaster(region)) Debuglog($"Failed to connect to Photon {region}");
        }
        #region authentication events
        public void OnCustomAuthenticationFailed(string debugMessage) { throw new NotImplementedException(); }
        public void OnCustomAuthenticationResponse(Dictionary<string, object> data) { throw new NotImplementedException(); }
        #endregion


        public void OnConnected() { if (IsConnected) { Debuglog($"{_botName} Connected!"); } else { Debuglog($"{_botName} IsNotConnected!"); } }
        public void OnCreatedRoom() { Debuglog($"{_botName} Created!"); }
        public void OnCreateRoomFailed(short returnCode, string message) { Debuglog($"OnCreateRoomFailed!/nReturnCode: {returnCode}\nReason: {message}"); }
        public void OnDisconnected(DisconnectCause cause) { Debuglog($"{_botName} Disconnected!\nReson:{cause}"); }
        public Photon.Realtime.RaiseEventOptions UnreliableEventOptions = new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others, CachingOption = Photon.Realtime.EventCaching.DoNotCache };
        public ExitGames.Client.Photon.SendOptions UnreliableOptions = new ExitGames.Client.Photon.SendOptions { DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Unreliable };
        public Photon.Realtime.RaiseEventOptions reliableEventOptions = new Photon.Realtime.RaiseEventOptions { Receivers = Photon.Realtime.ReceiverGroup.Others, CachingOption = Photon.Realtime.EventCaching.DoNotCache };
        public ExitGames.Client.Photon.SendOptions reliableOptions = new ExitGames.Client.Photon.SendOptions { DeliveryMode = ExitGames.Client.Photon.DeliveryMode.Reliable };
        private void OnResponseReceived(OperationResponse opResponse) { }//{Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine($"Response received: {opResponse}");}
        private void OnStateChanged(ClientState before, ClientState now) { }//{ Console.ForegroundColor= ConsoleColor.Yellow;Console.WriteLine($"Changed state from {before} to {now}"); }
        public void OnJoinRandomFailed(short returnCode, string message) { Debuglog($"Player Failed To Join Random!\nReturnCode: {returnCode}\nReason: {message}"); }
        public void OnJoinRoomFailed(short returnCode, string message) { Debuglog($"Player Failed To Join Room!\nReturnCode: {returnCode}\nReason: {message}"); }
        public void OnPlayerLeftRoom(Player otherPlayer) { Debuglog($"{otherPlayer.GetDisplayName()} has left room"); }
        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }//  Debuglog($"{targetPlayer.GetDisplayName()} Properties Updated: {changedProps}");  }
        public void OnFriendListUpdate(List<FriendInfo> friendList) { throw new NotImplementedException(); }
        public void OnRegionListReceived(RegionHandler regionHandler) { }
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
        public void OnLeftRoom() { }

        public static byte[] Event8Full(int[] actrs)
        {
            byte[] data = new byte[actrs.Length * 6];
            for (int i = 0; i < actrs.Length; i++)
            {
                int actr = actrs[i];
                int position = i * 6;
                byte[] actrId = BitConverter.GetBytes(actr);
                Buffer.BlockCopy(actrId, 0, data, position, 4);
                data[position + 4] = 0x00;
                data[position + 5] = 0xFF;
            }
            return data;
        }

        public void OnJoinedRoom()
        {

            Debuglog($"{_botName} Has Joined Room!");
            Debuglog($"WorldID: {CurrentRoom.Name}");
            Debuglog($"Players: {CurrentRoom.PlayerCount}");

            string[][] SendToJoin = Serialization.FromByteArray<string[][]>(Convert.FromBase64String("AAEAAAD/////AQAAAAAAAAAHAQAAAAEBAAAAAgAAAAYJAgAAAAkDAAAAEQIAAAAAAAAAEQMAAAAAAAAACw=="));
            byte[] data = System.Convert.FromBase64String("AgAAABF2h/EcVT0A+FPbmFO9I7AWur7zjv6RZI4ts1RTtRat/Z4aT7g/uJfPUyFublthDawYJ9rXWhJS9DsotoVuSXNJo2c1Lh1VPQD4WoAOR1gmaao0s/7p6KFGbYnNIt83DAyickqUt9t8yfRuFgxRTZVJhO5yrSe2Arcguh4cqD6hivaRQ/kfHlU9APhbS91axtN8BXfsn5xy9P/M4ZPZCNaD/iN1uibQ2ZgRCxV8KccTAGvqMqaBYBG+G7XNOg4HZpfbRYmWNho=");
            OpRaiseEvent(33, new Dictionary<byte, object> { { 0, (byte)20 }, { 3, SendToJoin }, }, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others, }, new SendOptions() { DeliveryMode = DeliveryMode.Reliable, Reliability = true, Channel = 0, });
            int[] viewids = new int[4] { int.Parse(LocalPlayer.ActorNumber + "00001"), int.Parse(LocalPlayer.ActorNumber + "00002"), int.Parse(LocalPlayer.ActorNumber + "00003"), int.Parse(LocalPlayer.ActorNumber + "00004") };
            int Player = int.Parse(LocalPlayer.ActorNumber + "00001");
            byte[] PlayerData = BitConverter.GetBytes(Player);
            Buffer.BlockCopy(PlayerData, 0, data, 0, 4);
            var raiseEventOptions = new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others };
            var sendOptions = new SendOptions() { Channel = 0, Encrypt = true, DeliveryMode = DeliveryMode.Reliable, Reliability = true };
            var actors = new int[] { int.Parse($"{PhotonExtensions.GetPlayer(this, userID).ActorNumber}00001") };
            Hashtable InstantiateHashtable = new Hashtable
            {
                [(byte)0] = "VRCPlayer",
                [(byte)1] = new Vector3(0f, 0f, 0f),//pos
                [(byte)2] = new Quaternion(0f, 0f, 0f, 1f),//rot
                [(byte)4] = viewids,
                [(byte)6] = LoadBalancingPeer.ServerTimeInMilliSeconds,
                [(byte)7] = viewids[0]
            };

            OpRaiseEvent(202, InstantiateHashtable, RaiseEventOptions.Default, SendOptions.SendReliable);
            OpRaiseEvent(7, data, new RaiseEventOptions() { CachingOption = EventCaching.DoNotCache, Receivers = ReceiverGroup.Others }, SendOptions.SendReliable);
            OpRaiseEvent(8, Event8Full(actors), raiseEventOptions, sendOptions);
        }

        private void CustomOnEvent(EventData eventData)
        {
            byte code = eventData.Code;
            switch (code)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    if (eventData.Sender != PhotonExtensions.GetPlayer(this, userID).ActorNumber) break;
                    var movementdata = (byte[])eventData.CustomData;
                    byte[] viewIDOut = BitConverter.GetBytes(int.Parse($"{LocalPlayer.ActorNumber}00001"));
                    var ping = BitConverter.GetBytes(1337);
                    var targetvec = BufferRW.ReadVector3(movementdata, 48);
                    var offset = BufferRW.Vector3ToBytes(new Vector3(targetvec.X, targetvec.Y, targetvec.Z + Convert.ToSingle(posz)));
                    Buffer.BlockCopy(viewIDOut, 0, movementdata, 0, 4);
                    Buffer.BlockCopy(offset, 0, movementdata, 48, offset.Length);
                    Buffer.BlockCopy(ping, 0, movementdata, 68, 4);
                    OpRaiseEvent(7, movementdata, UnreliableEventOptions, UnreliableOptions);
                    PhotonExtensions.ChangeAvatar(this, Bot.Client, AvatarID);
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 33:
                    break;
                case 201:   //Unreliable PhotonView
                    break;
                case 206:   //Reliable PhotonView
                    break;
                case 210:   //Item Ownership
                    break;
                case 209:   //Item Ownership
                    break;

            }

        }

        public void OnConnectedToMaster()
        {
            Debuglog($"{_botName} Connected to master!");
            LocalPlayer.SetCustomProperties(new Hashtable()
            {

                { "inVRMode", true },
                { "showSocialRank", true },
                { "steamUserID", "0" },
                { "modTag", null},
                { "isInvisible", false},
                { "avatarEyeHeight", 1000},
            });
            var targett = PhotonExtensions.GetPlayer(this, userID);

        }


        public void OnMasterClientSwitched(Player newMasterClient) { Debuglog($"{newMasterClient.GetDisplayName()} Switched."); }
        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            string info = string.Empty;
            info += $"Room: {CurrentRoom.Name}\n";
            info += $"Player Count: {CurrentRoom.PlayerCount}\n";
            info += $"Players in Instance:\n";
            var tag = newPlayer.CustomProperties["modTag"];
            var user = newPlayer.CustomProperties["user"] as Dictionary<string, object>;
            var avatar = newPlayer.CustomProperties["avatarDict"] as Dictionary<string, object>;
            var avtrID = avatar["id"];
            var userID = user["id"];
            var username = user["username"];
            var displayname = user["displayName"];
            var AvatarCopying = user["allowAvatarCopying"];
            info += "================================================\n";
            info += $"Player ActorID: {newPlayer.ActorNumber}\n";
            info += $"Player UserID: {userID}\n";
            info += $"Player Username: {username}\n";
            info += $"Player DisplayName: {displayname}\n";
            info += $"Player AvatarID: {avtrID}\n";
            info += $"Player Is Master: {newPlayer.IsMasterClient}\n";
            info += $"Player Is Moderator: {(tag.ToString() == string.Empty ? "False" : "True")}\n";
            info += $"Player Has Cloning Enabled: {AvatarCopying}\n";
            info += $"Player SteamID: {(newPlayer.CustomProperties["steamUserID"] == null ? "0" : newPlayer.CustomProperties["steamUserID"].ToString())}\n";
            info += "================================================\n";
            Debuglog(info);
        }
    }
}





       
   
