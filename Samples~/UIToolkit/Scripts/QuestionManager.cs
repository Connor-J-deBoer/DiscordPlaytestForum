// Copyright © Connor deBoer 2024, All Rights Reserved

using DiscordPlaytestForm;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class QuestionManager
{
    public UnityEvent<bool> QuestionAnswered = new UnityEvent<bool>();

    private QuestionList _questions;
    public QuestionManager(QuestionList questions) => _questions = questions;

    /// <summary>
    /// Properly creates a list item when it enters the scroll view
    /// </summary>
    /// <param name="item">the item, as defined by the makeItem</param>
    /// <param name="index">The index in the list</param>
    /// <param name="questionList">The listView, so we can pass it to our handle answer function</param>
    public void BindListItems(VisualElement item, int index, ListView questionList)
    {
        // Set the label of the question
        Label label = item.Q<Label>("Question");
        Question question = _questions.Questions[index];
        string questionString = (question.Required) ? $"{question.QuestionText} (Required)" : question.QuestionText;
        label.text = questionString;

        // Remove any answers already in the UXML
        VisualElement buttonContainer = item.Q<VisualElement>("ButtonContainer");
        buttonContainer.Clear();

        // Create the correct amount answers, make them look right, and set up the value change call back
        // so we can handle an answer
        for (int j = 0; j < question.AnswersText.Length; ++j)
        {
            Answer answer = question.AnswersText[j];
            Toggle answerToggle = new Toggle(answer.Name);
            answerToggle.value = answer.Value;
            answerToggle.style.color = Color.white;

            answerToggle.RegisterValueChangedCallback(evnt => HandleAnswer(evnt.newValue, question, answer, questionList));
            buttonContainer.Add(answerToggle);
        }
    }

    /// <summary>
    /// Sets the values to match the ui
    /// </summary>
    /// <param name="newValue">the new value in the ui</param>
    /// <param name="question">the questions it's answering</param>
    /// <param name="answer">the answer chosen</param>
    /// <param name="list">the listview the question is in</param>
    private void HandleAnswer(bool newValue, Question question, Answer answer, ListView list)
    {
        answer.Value = newValue;

        if (newValue && !question.AllowMoreThanOneAnswer)
            PreventMutipleChoices(question.AnswersText, answer);

        QuestionAnswered.Invoke(_questions.AllRequiredQuestionsAnswered());
        list.Rebuild();
    }

    /// <summary>
    /// Prevents more than one answer per question from being selected
    /// </summary>
    /// <param name="allAnswers">all answers to the question</param>
    /// <param name="answer">the chosen answer</param>
    private void PreventMutipleChoices(Answer[] allAnswers, Answer answer)
    {
        for (int i = 0; i < allAnswers.Length; ++i)
        {
            if (allAnswers[i].Value && allAnswers[i] != answer)
            {
                allAnswers[i].Value = false;
            }
        }
    }
}