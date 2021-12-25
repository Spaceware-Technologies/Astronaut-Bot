using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;

namespace Astronaut
{



    public enum RpcTarget
    {
        All,
        Others,
        MasterClient,
        AllBuffered,
        OthersBuffered,
        AllViaServer,
        AllBufferedViaServer
    }

    public class AvatarInformation
    {

        public string name { get; set; }
        public string description { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string assetUrl { get; set; }
        public object[] tags { get; set; }
        public string releaseStatus { get; set; }
        public int version { get; set; }
        public string unityPackageUrl { get; set; }
        public object unityPackageUrlObject { get; set; }
        public string unityVersion { get; set; }
        public int assetVersion { get; set; }
        public string platform { get; set; }
        public bool featured { get; set; }
        public bool imported { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string id { get; set; }
        public Unitypackage[] unityPackages { get; set; }

        public class Unitypackage
        {
            public string id { get; set; }
            public string assetUrl { get; set; }
            public object assetUrlObject { get; set; }
            public string unityVersion { get; set; }
            public long unitySortNumber { get; set; }
            public int assetVersion { get; set; }
            public string platform { get; set; }
            public DateTime created_at { get; set; }
        }

    }

    public class LoginResponse
    {
        public string id { get; set; }
        public string username { get; set; }
        public string displayName { get; set; }
        public string bio { get; set; }
        public object[] bioLinks { get; set; }
        public object[] pastDisplayNames { get; set; }
        public bool hasEmail { get; set; }
        public bool hasPendingEmail { get; set; }
        public string email { get; set; }
        public string obfuscatedEmail { get; set; }
        public string obfuscatedPendingEmail { get; set; }
        public bool emailVerified { get; set; }
        public bool hasBirthday { get; set; }
        public bool unsubscribe { get; set; }
        public object[] friends { get; set; }
        public object[] friendGroupNames { get; set; }
        public string currentAvatarImageUrl { get; set; }
        public string currentAvatarThumbnailImageUrl { get; set; }
        public string currentAvatar { get; set; }
        public string currentAvatarAssetUrl { get; set; }
        public object accountDeletionDate { get; set; }
        public int acceptedTOSVersion { get; set; }
        public string steamId { get; set; }
        public object steamDetails { get; set; }
        public string oculusId { get; set; }
        public bool hasLoggedInFromClient { get; set; }
        public string homeLocation { get; set; }
        public bool twoFactorAuthEnabled { get; set; }
        public object feature { get; set; }
        public string status { get; set; }
        public string statusDescription { get; set; }
        public string state { get; set; }
        public object[] tags { get; set; }
        public string developerType { get; set; }
        public DateTime last_login { get; set; }
        public string last_platform { get; set; }
        public bool allowAvatarCopying { get; set; }
        public bool isFriend { get; set; }
        public string friendKey { get; set; }
        public object[] onlineFriends { get; set; }
        public object[] activeFriends { get; set; }
        public object[] offlineFriends { get; set; }
    }

    public class WorldInformation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool featured { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public int capacity { get; set; }
        public string[] tags { get; set; }
        public string releaseStatus { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string assetUrl { get; set; }
        public string pluginUrl { get; set; }
        public string unityPackageUrl { get; set; }
        public string _namespace { get; set; }
        public int version { get; set; }
        public string organization { get; set; }
        public object previewYoutubeId { get; set; }
        public int favorites { get; set; }
        public int visits { get; set; }
        public int popularity { get; set; }
        public int heat { get; set; }
        public int publicOccupants { get; set; }
        public int privateOccupants { get; set; }
        public int occupants { get; set; }
        public object[][] instances { get; set; }
    }


    public static class PhotonExtensions
    {
        public static string GetUsername(this Player player)
        {
            if (player.CustomProperties.ContainsKey("user"))
                if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                    return (string)dict["username"];
            return "";
        }
        public static string GetUserID(this Player player)
        {
            if (player.CustomProperties.ContainsKey("user"))
                if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                    return (string)dict["id"];
            return "";
        }

        public static string GetDisplayName(this Player player)
        {
            if (player.CustomProperties.ContainsKey("user"))
                if (player.CustomProperties["user"] is Dictionary<string, object> dict)
                    return (string)dict["displayName"];
            return "";
        }
        public static string GetSteamID(this Player player)
        {
            if (player.CustomProperties.ContainsKey("steamUserID"))
                if ((string)player.CustomProperties["steamUserID"] != "0")
                    return (string)player.CustomProperties["steamUserID"];
            return "No Steam";
        }
        public static void EventSpammer(this int count, int amount, Action action, int? sleep = null)
        {
            for (int ii = 0; ii < count; ii++)
            {
                for (int i = 0; i < amount; i++)
                    action();
                if (sleep != null)
                    Thread.Sleep(sleep.Value);
                else
                    Thread.Sleep(25);
            }
        }

        public static string GetAvatarID(this Player player)
        {
            var avatar = player.CustomProperties["avatarDict"] as Dictionary<string, object>;
            var avtrID = avatar["id"];
            return (string)avtrID;
        }


        public static Player GetPlayer(this PhotonClient client, string userid)
        {
            foreach (var player in client.CurrentRoom.Players.Values)
            {
                var user = player.CustomProperties["user"] as Dictionary<string, object>;
                if (user["id"].ToString() == userid) return player;
            }

            return null;
        }



        public static RaiseEventOptions SetPlayerAsTarget(Player ply)
        {
            return new RaiseEventOptions
            {
                TargetActors = new int[] { ply.ActorNumber }
            };
        }



        //Change aviii go brr
        public static void ChangeAvatar(this PhotonClient photonClient, HttpClient client, string avatarID)
        {
            var response = client.PutAsync($"https://www.vrchat.com/api/1/avatars/{avatarID}/select?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat", null);
            photonClient.OpRaiseEvent(40, (short)1, new RaiseEventOptions() { Receivers = ReceiverGroup.Others }, default(SendOptions));
        }

        ////Change aviii go brr
        //public static void ChangeAvatar(this PhotonClient photonClient, string avatarID)
        //{
        //    HttpClientHandler handler = new HttpClientHandler();
        //    handler.CookieContainer = new CookieContainer();
        //    Uri target = new Uri("https://www.vrchat.com");
        //    handler.CookieContainer.Add(new Cookie("auth", Program.bot.Token) { Domain = target.Host });
        //    HttpClient client = new HttpClient(handler);
        //    var response = client.PutAsync($"https://www.vrchat.com/api/1/avatars/{avatarID}/select?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat", null);
        //    photonClient.OpRaiseEvent(40, (short)1, new RaiseEventOptions() { Receivers = ReceiverGroup.Others }, default(SendOptions));
        //}
    }

}