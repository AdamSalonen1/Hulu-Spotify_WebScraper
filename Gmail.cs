using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HtmlAgilityPack;

namespace Application
{
    internal class Gmail
    {
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "Gmail API Application";
        public Gmail() { }

        public string GetHuluVerificationCode()
        {
            GmailService GmailService = GetService();
            UsersResource.MessagesResource.ListRequest ListRequest = GmailService.Users.Messages.List(ConfigurationManager.AppSettings["HostAddress"]);
            ListRequest.Q = "from:accounts-noreply@messaging.hulu.com is:unread";

            ListMessagesResponse ListResponse = ListRequest.Execute();

            string msgID = ListResponse.Messages[0].Id;

            UsersResource.MessagesResource.GetRequest Message = GmailService.Users.Messages.Get(ConfigurationManager.AppSettings["HostAddress"], msgID);

            Message msgContent = Message.Execute();

            string Base64Test = msgContent.Payload.Body.Data;

            string temp = Base64Decode(Base64Test);

            int codeIndex = temp.IndexOf("</b>");

            string value = temp.Substring(codeIndex - 6, 6);

            //MsgMarkAsRead(ConfigurationManager.AppSettings["HostAddress"], msgID);

            return value;
        }

        public static GmailService GetService()
        {
            UserCredential credential;
            using (FileStream stream = new FileStream(Convert.ToString(ConfigurationManager.AppSettings["ClientInfo"]),
                FileMode.Open, FileAccess.Read))
            {
                String FolderPath = Convert.ToString(ConfigurationManager.AppSettings["CredentialsInfo"]);
                String FilePath = Path.Combine(FolderPath, "APITokenCredentials");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(FilePath, true)).Result;
            }
            // Create Gmail API service.
            GmailService service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
        }

        public static string Base64Decode(string Base64Test)
        {
            string EncodTxt = string.Empty;
            //STEP-1: Replace all special Character of Base64Test
            EncodTxt = Base64Test.Replace("-", "+");
            EncodTxt = EncodTxt.Replace("_", "/");
            EncodTxt = EncodTxt.Replace(" ", "+");
            EncodTxt = EncodTxt.Replace("=", "+");

            //STEP-2: Fixed invalid length of Base64Test
            if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('=', 4 - EncodTxt.Length % 4); }
            else if (EncodTxt.Length % 4 == 0)
            {
                try
                {
                    EncodTxt = EncodTxt.Substring(0, EncodTxt.Length - 1);
                }
                catch
                {
                }
                if (EncodTxt.Length % 4 > 0) { EncodTxt += new string('+', 4 - EncodTxt.Length % 4); }
            }

            //STEP-3: Convert to Byte array
            byte[] ByteArray = Convert.FromBase64String(EncodTxt);

            //STEP-4: Encoding to UTF8 Format
            return Encoding.UTF8.GetString(ByteArray);
        }

        public static void MsgMarkAsRead(string HostEmailAddress, string MsgId)
        {
            //MESSAGE MARKS AS READ AFTER READING MESSAGE
            ModifyMessageRequest mods = new ModifyMessageRequest();
            mods.AddLabelIds = null;
            mods.RemoveLabelIds = new List<string> { "UNREAD" };
            GetService().Users.Messages.Modify(mods, HostEmailAddress, MsgId).Execute();
        }
    }
}
