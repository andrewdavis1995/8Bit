using System;
using Assets.Classes;

using UnityEngine;
using UnityEngine.UI;

public class PersonInteractionScript : MonoBehaviour
{
    public Person person;
    private SpriteRenderer _renderer;
    private bool _isInteracting = false;
    public StateMachine Status = new StateMachine();

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

    internal string[] GetOptions(ref Command[] commands)
    {
        switch (Status.CurrentState)
        {
            case ProcessState.MenuOptions:
                commands = new Command[] { Command.GoToQuestions, Command.GoToCompliments, Command.GoToJokes, Command.Exit };
                return new string[] { "Ask Question", "Pay Compliment", "Tell Joke", "Goodbye" };
            case ProcessState.BrowsingQuestions:
                commands = new Command[] { Command.QuestionSelected, Command.QuestionSelected, Command.BackToMenu };
                return new string[] { "Question 1", "Question 2", "Cancel" };
            case ProcessState.SelectingCompliment:
                commands = new Command[] { Command.ComplimentSelected, Command.ComplimentSelected, Command.BackToMenu };
                return new string[] { "Compliment 1", "Compliment 2", "Cancel" };
            case ProcessState.SelectingJoke:
                commands = new Command[] { Command.JokeSelected, Command.JokeSelected, Command.BackToMenu };
                return new string[] { "Joke 1", "Joke 2", "Cancel" };

            case ProcessState.AskingQuestion:
                commands = new Command[] { Command.FinishedSpeaking };
                return null;
            case ProcessState.TellingJoke:
                commands = new Command[] { Command.FinishedSpeaking };
                return null;
            case ProcessState.TellingCompliment:
                commands = new Command[] { Command.FinishedSpeaking };
                return null;

            case ProcessState.Listening:
                commands = new Command[] { Command.DoneListening };
                return null;

            default:
                //ProcessState.Inactive:
                //ProcessState.Ended:
                commands = null;
                return null;
        }
    }
}