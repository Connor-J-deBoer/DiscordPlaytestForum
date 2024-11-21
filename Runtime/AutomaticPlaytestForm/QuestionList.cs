// Copyright © Connor deBoer 2024, All Rights Reserved

using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DiscordPlaytestForm
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Question List")]
    public class QuestionList : ScriptableObject
    {
        public string DiscordWebHookURL;
        public Question[] Questions;

        /// <summary>
        /// Sets all answers false
        /// </summary>
        public void CleanQuestions()
        {
            foreach (Question question in Questions)
            {
                foreach (Answer answer in question.AnswersText)
                {
                    answer.Value = false;
                }
            }
        }
        
        /// <summary>
        /// Checks to see that all questions have been answered
        /// </summary>
        /// <returns></returns>
        public bool AllRequiredQuestionsAnswered()
        {
            return !Questions.Any(question => !question.Filled() && question.Required);
        }

        /// <summary>
        /// Posts all Questions to discord
        /// </summary>
        /// <returns>True if sent, false if failed</returns>
        public async Task<bool> SendQuestionsToDiscord()
        {
            SendToDiscord send = new SendToDiscord();
            return await send.PostToDiscord(Questions, DiscordWebHookURL);
        }

        public override string ToString()
        {
            string message = "";
            foreach (Question question in Questions)
            {
                message += $"{question}\n";
            }
            return message;
        }
    }
}