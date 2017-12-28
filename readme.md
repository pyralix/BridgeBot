Steps to deploy:

1. Deploy to a Azure Web Services or use ngrok to create a tunnel. You will need to run the bot on a an http/s endpoint. Even though the bot isn't ready, we still need the URL that it will be using for the later steps.

2. Create a Bot in Azure Bot Services under "Bot Channels Registration",
 a. Use your http endpoint during creation
 b. Go to Settings > Manage Microsoft App ID
 c. Copy the application Id and copy a newly generated password
 d. Paste both the ID and the Secret into your web.config and update your site

3. Go to Channels and enable the Teams channel

4. Go to Channels and enable the Slack channel
 a. Follow instructions at https://docs.microsoft.com/en-us/bot-framework/bot-service-channel-connect-slack to configure
 b. In addition to the configuration steps, add an Incoming Webhook to the Bot application
 c. Make sure to add your bot to the channel that it is configured to forward into

5. Go to your Teams channel and configure an Incoming Webhook. Use the same as your bot

6. Configure Webhook URIs in Web.config
 a. Use the bot application webhook url from slack as the value for the SlackURI key
 b. Use the configured Incoming webhook from teams as the value for the TeamsURI key

7. Do a final clean and build. Publish to a folder.

8. Prepare the PublishOutput folder by creating or updating a manifest.json. Instructions at https://docs.microsoft.com/en-us/microsoftteams/platform/publishing/apps-package
 a. Change the id and BotId key values to your Microsoft AppId inside of the manifest.json
 b. Add two images, one in 96px*96px color named color.png and one in 20px*20px white named outline.png

9. Zip the contents of the PublishOutput folder into a file and sideload into teams, following instructions at https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/apps/apps-sideload
