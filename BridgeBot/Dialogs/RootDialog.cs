using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Text.RegularExpressions;

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
            var body = "\"" + userName + activity.Text + "\"";
            body = Regex.Replace(body, WebConfigurationManager.AppSettings["BotId"], "", RegexOptions.IgnoreCase);
            body = Regex.Replace(body, "@", "", RegexOptions.IgnoreCase);
            body = Regex.Replace(body, "</*at>", "", RegexOptions.IgnoreCase);

            //ServiceUrls, for reference
            //Teams: https://smba.trafficmanager.net/amer-client-ss.msg/
            //Slack: https://slack.botframework.com

            String appSettingsKey;
            // If from Teams, send message to Slack
            if (channelID.Equals("msteams"))
            {
                appSettingsKey = "SlackURI";
            }
            // If from Slack, send message to Teams
            else
            {
                appSettingsKey = "TeamsURI";
            }

            try
            {
                Uri requestUri = new Uri(WebConfigurationManager.AppSettings[appSettingsKey]);
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
    }
}
