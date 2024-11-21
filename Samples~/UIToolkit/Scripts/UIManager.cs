// Copyright © Connor deBoer 2024, All Rights Reserved

using DiscordPlaytestForm;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset _questionUXML;
    [SerializeField] private QuestionList _questions;

    private VisualElement _root;
    private QuestionManager _questionManager;
    private Button _submit;

    private void Start()
    {
        // Get the root of the document so we can control the UI
        _root = GetComponent<UIDocument>().rootVisualElement;
            
        // hide the scroller because it's ugly
        _root.Q<ScrollView>().verticalScrollerVisibility = ScrollerVisibility.Hidden;

        // Set up the forum to work properly
        _questions.CleanQuestions();
        HandleSubmitButton();

        _questionManager = new QuestionManager(_questions);
        _questionManager.QuestionAnswered.AddListener(allAnswered =>
        {
            _submit.style.visibility = allAnswered ? Visibility.Visible : Visibility.Hidden;
        });

        // Define the list view so that everything displays properly
        ListView questionList = _root.Q<ListView>();
        Func<VisualElement> makeItem = () => _questionUXML.Instantiate();
        Action<VisualElement, int> bindItem = (item, index) => _questionManager.BindListItems(item, index, questionList);

        questionList.itemsSource = _questions.Questions;
        questionList.makeItem = makeItem;
        questionList.bindItem = bindItem;
    }

    /// <summary>
    /// Finds and sets up the Submit button in the document so that it's only visible if the questions have been 
    /// answered and so that when clicked it sends them to discord and shows the response
    /// </summary>
    private void HandleSubmitButton()
    {
        _submit = _root.Q<Button>("submit");
        _submit.clicked += async () =>
        {
            bool worked = await _questions.SendQuestionsToDiscord();
            _root.Q<VisualElement>("Survey").style.visibility = Visibility.Hidden;
            _root.Q<VisualElement>("AfterSurvey").style.visibility = Visibility.Visible;

            Label message = _root.Q<Label>("Message");
            message.text = (worked) ? "Success" : "Failed";
            message.style.color = (worked) ? Color.green : Color.red;
        };

        _submit.style.visibility = _questions.AllRequiredQuestionsAnswered() ? Visibility.Visible : Visibility.Hidden;
    }
}