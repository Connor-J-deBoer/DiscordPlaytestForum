// Copyright © Connor deBoer 2024, All Rights Reserved

using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace DiscordPlaytestForm
{
    internal class SendToDiscord
    {
        // This guy is sorta like axios, she allows us to send our http requests
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// Sends our questions to discord
        /// </summary>
        /// <param name="allQuestions">An array of questions</param>
        /// <param name="webHook">The web hook to send them to</param>
        /// <returns>true if sent was successful, false if failed</returns>
        public async Task<bool> PostToDiscord(Question[] allQuestions, string webHook)
        {
            try
            {
                var payload = new
                {
                    content = await DecodeAnswer(allQuestions)
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                var request = new HttpRequestMessage(HttpMethod.Post, webHook)
                {
                    Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json")
                };

                await _client.SendAsync(request);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This guy turns our questions into a properly formated discord message and adds the playername/id (that's why it's async)
        /// </summary>
        /// <param name="allQuestions">An array of questions</param>
        /// <returns>A properly formated string to be used for discord</returns>
        private async Task<string> DecodeAnswer(Question[] allQuestions)
        {
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            string playerName = AuthenticationService.Instance.PlayerName;
            string playerId = AuthenticationService.Instance.PlayerId;
            string playerIdentification = string.IsNullOrEmpty(playerName) ? playerId : playerName;
            
            string output = $"**Playtest Response from {playerIdentification}:**\n\n";
            foreach (Question question in allQuestions)
            {
                output += $"- {question.QuestionText}:\n";

                if (question.AnswersText.Where(ans => ans.Value).Count() == 0)
                {
                    output += "\tNone\n";
                }

                foreach (Answer answer in question.AnswersText)
                {
                    if (!answer.Value)
                        continue;

                    output += $"\t{answer.Name}\n";
                }

                output += "\n" ;
            }
            return output;
        }
    }
}