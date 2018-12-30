using System;
using System.Collections.Generic;

public enum ProcessState
{
    Inactive,
    /* Menus */
    MenuOptions, SelectingJoke, SelectingCompliment, BrowsingQuestions,
    /* Selected Option showing */
    TellingJoke, TellingCompliment, AskingQuestion,
    /* Listening to answer from person */
    Listening,
    Ended
}

public enum Command
{
    StartConversation,
    GoToJokes, GoToCompliments, GoToQuestions,
    JokeSelected, ComplimentSelected, QuestionSelected,
    FinishedSpeaking,
    DoneListening,
    BackToMenu,
    Blocked,
    Exit
}
public class StateMachine
{
    class StateTransition
    {
        readonly ProcessState CurrentState;
        readonly Command Command;
        public StateTransition(ProcessState currentState, Command command)
        {
            CurrentState = currentState;
            Command = command;
        }
        public override int GetHashCode()
        {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            StateTransition other = obj as StateTransition;
            return other != null && CurrentState == other.CurrentState && Command == other.Command;
        }
    }

    Dictionary<StateTransition, ProcessState> transitions;

    public ProcessState CurrentState { get; set; }
    public ProcessState PreviousState { get; set; }

    public StateMachine()
    {
        CurrentState = ProcessState.Inactive;
        PreviousState = ProcessState.Inactive;

        transitions = new Dictionary<StateTransition, ProcessState>
        {
                                        // current state                       // transition                           // next state
            { new StateTransition(      ProcessState.Inactive,                 Command.StartConversation),             ProcessState.MenuOptions },
            { new StateTransition(      ProcessState.Inactive,                 Command.Blocked),                       ProcessState.Listening },

            { new StateTransition(      ProcessState.MenuOptions,              Command.GoToCompliments),               ProcessState.SelectingCompliment },
            { new StateTransition(      ProcessState.MenuOptions,              Command.GoToJokes),                     ProcessState.SelectingJoke },
            { new StateTransition(      ProcessState.MenuOptions,              Command.GoToQuestions),                 ProcessState.BrowsingQuestions },
            { new StateTransition(      ProcessState.MenuOptions,              Command.Exit),                          ProcessState.Ended },

            { new StateTransition(      ProcessState.SelectingJoke,            Command.BackToMenu),                    ProcessState.MenuOptions },
            { new StateTransition(      ProcessState.SelectingJoke,            Command.JokeSelected),                  ProcessState.TellingJoke },

            { new StateTransition(      ProcessState.SelectingCompliment,      Command.BackToMenu),                    ProcessState.MenuOptions },
            { new StateTransition(      ProcessState.SelectingCompliment,      Command.ComplimentSelected),            ProcessState.TellingCompliment },

            { new StateTransition(      ProcessState.BrowsingQuestions,        Command.BackToMenu),                    ProcessState.MenuOptions },
            { new StateTransition(      ProcessState.BrowsingQuestions,        Command.QuestionSelected),              ProcessState.AskingQuestion },

            { new StateTransition(      ProcessState.TellingJoke,              Command.FinishedSpeaking),              ProcessState.Listening },

            { new StateTransition(      ProcessState.TellingCompliment,        Command.FinishedSpeaking),              ProcessState.Listening },

            { new StateTransition(      ProcessState.AskingQuestion,           Command.FinishedSpeaking),              ProcessState.Listening },

            { new StateTransition(      ProcessState.Listening,                Command.DoneListening),                 ProcessState.MenuOptions },
            { new StateTransition(      ProcessState.Listening,                Command.Exit),                          ProcessState.Ended },
        };
    }

    internal bool AllowsUpDown()
    {
        return CurrentState == ProcessState.MenuOptions
            || CurrentState == ProcessState.BrowsingQuestions
            || CurrentState == ProcessState.SelectingCompliment
            || CurrentState == ProcessState.SelectingJoke;
    }

    public ProcessState GetNext(Command command)
    {
        StateTransition transition = new StateTransition(CurrentState, command);
        ProcessState nextState;
        if (!transitions.TryGetValue(transition, out nextState))
            throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
        return nextState;
    }

    public ProcessState MoveNext(Command command)
    {
        PreviousState = CurrentState;
        CurrentState = GetNext(command);
        return CurrentState;
    }
    public ProcessState MoveNext(string commandStr)
    {
        Command command = GetCommandFromString(commandStr);
        PreviousState = CurrentState;
        CurrentState = GetNext(command);
        return CurrentState;
    }

    private Command GetCommandFromString(string commandStr)
    {
        // all options
        commandStr = commandStr.ToLower();
        if(commandStr == "Goodbye")
        {
            return Command.Exit;
        }


        return Command.BackToMenu;
    }
}