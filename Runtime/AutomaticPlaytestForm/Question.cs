// Copyright © Connor deBoer 2024, All Rights Reserved

using System.Linq;

namespace DiscordPlaytestForm
{
    [System.Serializable]
    public class Question
    {
        public string QuestionText;
        public Answer[] AnswersText;
        public bool AllowMoreThanOneAnswer;
        public bool Required;

        /// <summary>
        /// Checks all the answers and checks to see if at least one has been answered
        /// </summary>
        /// <returns>Returns true if even one answer has been selected</returns>
        public bool Filled()
        {
            return AnswersText.Any(ans => ans.Value);
        }

        public override string ToString()
        {
            string message = $"{QuestionText}\n{{\n";
            foreach (Answer answer in AnswersText)
            {
                message += $"    {answer.Name}: {answer.Value},\n";
            }
            message += $"}}\nAllow Multiple Answers: {AllowMoreThanOneAnswer}\nRequired: {Required}";
            return message;
        }
    }
}