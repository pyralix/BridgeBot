using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Web.Configuration;

namespace BridgeBot.Dialogs
{

        [Serializable]
    public class RootDialog : IDialog<object>
    {
        public object Client { get; set; }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            
            //Get Username
            var userName = "UnknownUser: ";
            if (activity.From.Name != null)
            {
                userName = activity.From.Name + ": ";
            }
            else
            {
                activity.From.Name = userName;
            }
            //Get ChannelID
            var channelID = "UnknownChannelID";
            if (activity.ChannelId != null)
            {
                channelID = activity.ChannelId;
            }
            else
            {
                activity.ChannelId = channelID;
            }

            //Use this to confirm activity
            //await context.PostAsync("Received...");

            //Remove incarnations of the bot's own name. You will need to replace these with the names of your bots as they occur within your unique environment.
            string body = "\"" + userName + activity.Text + "\"";
            body = body.Replace("<at>TestBot</at>", "").Replace("TestBot", "").Replace("@jerrythebot","");

            if (channelID.Equals("msteams"))
            // Send message to Slack
            {
                try
                {
                    Uri requestUri = new Uri(WebConfigurationManager.AppSettings["SlackURI"]); //replace your Url with a configurable from a .json
                    var values = new Dictionary<string, string>
                    {
                        {"text", body}
                    };
                    var json = JsonConvert.SerializeObject(values);
                    var objClint = new System.Net.Http.HttpClient();
                    System.Net.Http.HttpResponseMessage respon = await objClint.PostAsync(requestUri, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                }
                catch (HttpRequestException hre)
                {
                    await context.PostAsync("Error:" + hre.Message);
                }
            }
            else {
            // Send message to Teams
            try
            {
                Uri requestUri = new Uri(WebConfigurationManager.AppSettings["TeamsURI"]); //replace your Url with a configurable from a .json
                var values = new Dictionary<string, string>
                    {
                        {"text", body}
                    };
                var json = JsonConvert.SerializeObject(values);
                var objClint = new System.Net.Http.HttpClient();
                System.Net.Http.HttpResponseMessage respon = await objClint.PostAsync(requestUri, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            }
            catch (HttpRequestException hre)
            {
                await context.PostAsync("Error:" + hre.Message);
            }
            }
            //

        }
    }
}