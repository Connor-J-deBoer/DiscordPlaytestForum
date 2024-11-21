// Copyright © Connor deBoer 2024, All Rights Reserved

using DiscordPlaytestForm;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentManager : MonoBehaviour
{
    [SerializeField] private GameObject _questionUI;
    [SerializeField] private GameObject _answerUI;
    [SerializeField] private GameObject _submitButton;
    [SerializeField] private TextMeshProUGUI _response;
    [SerializeField] private QuestionList _questions;

    private void Start()
    {
        // sets all answers false
        _questions.CleanQuestions();

        // create all the questions in the ui using prefabs
        foreach (Question question in _questions.Questions)
        {
            GameObject questionUI = Instantiate(_questionUI, transform);
            string questionString = (question.Required) ? $"{question.QuestionText} (Required)" : question.QuestionText;
            questionUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = questionString;

            Transform answerContainer = questionUI.transform.GetChild(2);
            foreach (Answer answer in question.AnswersText)
            {
                GameObject answerUI = Instantiate(_answerUI, answerContainer);
                answerUI.GetComponentInChildren<Text>().text = answer.Name;
                Toggle answerToggle = answerUI.GetComponent<Toggle>();
                answerToggle.name = answer.Name;
                answerToggle.isOn = answer.Value;
                answerToggle.onValueChanged.AddListener(evnt => HandleAnswer(evnt, question, answer));
            }
        }
    }

    /// <summary>
    /// Sets the values to match the ui
    /// </summary>
    /// <param name="newValue">the new value in the ui</param>
    /// <param name="question">the questions it's answering</param>
    /// <param name="answer">the answer chosen</param>
    /// <param name="list">the listview the question is in</param>
    private void HandleAnswer(bool newValue, Question question, Answer answer)
    {
        answer.Value = newValue;

        if (newValue && !question.AllowMoreThanOneAnswer)
            PreventMutipleChoices(question.AnswersText, answer);

        _submitButton.SetActive(_questions.AllRequiredQuestionsAnswered());
    }

    /// <summary>
    /// Prevents more than one answer per question from being selected
    /// </summary>
    /// <param name="allAnswers">all answers to the question</param>
    /// <param name="answer">the chosen answer</param>
    private void PreventMutipleChoices(Answer[] allAnswers, Answer answer)
    {
        Transform answersUI = FindObjectsOfType<Toggle>().Where(tog => tog.name == answer.Name).First().transform.parent;
        for (int i = 0; i < allAnswers.Length; ++i)
        {
            if (allAnswers[i].Value && allAnswers[i] != answer)
            {
                allAnswers[i].Value = false;
                answersUI.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }
    }

    /// <summary>
    /// Sends the questions to discord and shows the response
    /// </summary>
    public async void HandleSubmitButton()
    {
        bool worked = await _questions.SendQuestionsToDiscord();
        transform.parent.parent.gameObject.SetActive(false);

        _response.gameObject.SetActive(true);
        _response.text = worked ? "Success" : "Failed";
        _response.color = worked ? Color.green : Color.red;
    }
}
