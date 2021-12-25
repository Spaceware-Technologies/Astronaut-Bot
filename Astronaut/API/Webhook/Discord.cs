//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;

//namespace Astronaut.API.Webhook
//{
//    class Discord
//    {

//        public static byte[] Post(string uri, NameValueCollection pairs)
//        {
//            byte[] numArray;
//            using (WebClient webClient = new WebClient())
//            {
//                numArray = webClient.UploadValues(uri, pairs);
//            }
//            return numArray;
//        }

//        public static void sendWebHook(string URL, string msg, string username)
//        {
//            Post(URL, new NameValueCollection()
//            {
//                { "username", username },
//                { "content", "```" + msg + "```" }
//            });
//        }
//    }
//}
