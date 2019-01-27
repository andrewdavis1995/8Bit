using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Classes;
using UnityEngine;

public class PersonInteractionScript : MonoBehaviour
{
    public Person person;
    private SpriteRenderer _renderer;
    private bool _isInteracting = false;
    public StateMachine Status = new StateMachine();
    public KnowledgeCategory[] Knowledge;
    public KnowledgeCategory[] KnowledgeRefuse;
    private KnowledgeCategory _currentQuestionType;

    public static bool ConversationActive = false;

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsInteracting()
    {
        return _isInteracting;
    }

    public void ConversationOver()
    {
        _isInteracting = false;
        ConversationActive = false;
        Status.CurrentState = ProcessState.Inactive;
    }

    public void Converse(Transform playerPosition)
    {
        _isInteracting = true;
        ConversationActive = true;

        // check not above anger threshold
        if (person.Anger < 40)
        {
            UIScript.Instance().GetComponent<UIScript>().StartConversation(this);
        }
        else
        {
            UIScript.Instance().GetComponent<UIScript>().BlockConversation(this);
        }

        if(playerPosition.position.x < transform.position.x)
        {
            _renderer.flipX = true;
        }
        else
        {
            _renderer.flipX = false;
        }
    }

    internal void GetOptions(ref Command[] commands, ref string[] options)
    {
        switch (Status.CurrentState)
        {
            case ProcessState.MenuOptions:
                commands = new Command[] { Command.GoToQuestions, Command.GoToCompliments, Command.GoToJokes, Command.Exit };
                options = new string[] { "Ask Question", "Pay Compliment", "Tell Joke", "Goodbye" };
                break;
            case ProcessState.BrowsingQuestions:
                GetQuestions(ref commands, ref options);
                break;
            case ProcessState.SelectingCompliment:
                commands = new Command[] { Command.ComplimentSelected, Command.ComplimentSelected, Command.BackToMenu };
                options = new string[] { "Compliment Hair", "Compliment Personality", "Cancel" };
                break;
            case ProcessState.SelectingJoke:
                commands = new Command[] { Command.JokeSelected, Command.JokeSelected, Command.JokeSelected, Command.BackToMenu };
                options = new string[] { "Scottish Joke", "Disney Joke", "Music Joke", "Cancel" };
                break;

            case ProcessState.AskingQuestion:
                commands = new Command[] { Command.FinishedSpeaking };
                break;
            case ProcessState.TellingJoke:
                commands = new Command[] { Command.FinishedSpeaking };
                break;
            case ProcessState.TellingCompliment:
                commands = new Command[] { Command.FinishedSpeaking };
                break;

            case ProcessState.Listening:
                commands = new Command[] { Command.DoneListening };
                break;

            default:
                //ProcessState.Inactive:
                //ProcessState.Ended:
                commands = null;
                break;
        }
    }

    private void GetQuestions(ref Command[] commands, ref string[] options)
    {
        var questions = new List<string> { "Ask age", "Ask how to build a ladder", "Ask name", "Ask where to buy weapons" };
        var comms = new List<Command> { Command.QuestionSelected, Command.QuestionSelected, Command.QuestionSelected, Command.QuestionSelected };

        var extras = LocationSpecificData.GetLocationSpecificQuestions();
        for (int i = 0; i < extras.Count; i++)
        {
            comms.Add(Command.QuestionSelected);
        }

        questions.AddRange(extras);

        questions.Add("Cancel");
        comms.Add(Command.BackToMenu);

        commands = comms.ToArray();
        options = questions.ToArray();
    }

    internal void SetQuestionType(KnowledgeCategory knowledgeCat)
    {
        _currentQuestionType = knowledgeCat;
    }

    internal string CheckQuestionAnswer()
    {
        // TODO: different response, based on personality

        if (Knowledge.Any(k => k == _currentQuestionType))
        {
            return GetAnswer();
        }
        if (KnowledgeRefuse.Any(k => k == _currentQuestionType))
        {
            return "I am not going to tell YOU!";
        }
        return "I don't know!";
    }

    private string GetAnswer()
    {
        switch(_currentQuestionType)
        {
            case KnowledgeCategory.HowOldAreYou:
                return person.Age.ToString();
            case KnowledgeCategory.HowToMakeALadder:
                return "You'll need to combine 2 bits of wood, and a piece of string";
            case KnowledgeCategory.WhatIsYourName:
                return person.Name;
            case KnowledgeCategory.WhereToBuyWeapons:
                return "Try the Blacksmith's shop - just east of here";

                // trials
            case KnowledgeCategory.IsThisADemoLevel:
                return LocationSpecificData.CurrentLocation == Location.Trials ? "It is indeed!" : "Absolutely not";

                // Caredall
            case KnowledgeCategory.WhenWasThisPlaceBuilt:
                return "1923";
        }


        return "I don't know, sorry";
    }
}