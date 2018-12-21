using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject PlayerConversation;
    public PersonInteractionScript PersonActive;

    public Transform SpeechOptionPrefab;
    public Transform SpeechWindowContainer;
    public Transform ConversationWindow;
    public Text ConversationText;
    public Transform InventoryPopup;

    private static UIScript _instance;
    
    private int _conversationIndex = 0;
    string[] currentOptions = new string[] { };
    Command[] currentCommands = new Command[] { };

    public static UIScript Instance() { return _instance; }

    public UIScript() { _instance = this; }

    private void Update()
    {
        if (PersonInteractionScript.ConversationActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RemoveMenuOptions();
                PersonActive.Status.MoveNext(currentCommands[_conversationIndex]);
                currentOptions = PersonActive.GetOptions(ref currentCommands);

                if (PersonActive.Status.CurrentState == ProcessState.Ended)
                {
                    Finished();
                }
                else if (PlayerSpeaking())
                {
                    ShowTalking();
                }
                else if (PersonActive.Status.CurrentState != ProcessState.Listening)
                {
                    ShowMenu();
                }
                else
                {
                    ShowResponse();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                ChangeOption(-1);
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                ChangeOption(1);
            }
        }
    }

    private bool PlayerSpeaking()
    {
        return
           PersonActive.Status.CurrentState == ProcessState.TellingCompliment
        || PersonActive.Status.CurrentState == ProcessState.TellingJoke
        || PersonActive.Status.CurrentState == ProcessState.AskingQuestion;
    }

    internal void BlockConversation(PersonInteractionScript personInteractionScript)
    {
        PersonActive = personInteractionScript;
        PersonActive.Status.MoveNext(Command.Blocked);
        currentOptions = PersonActive.GetOptions(ref currentCommands);
        currentCommands = new Command[] { Command.Exit };
        ShowResponse();
    }

    void ChangeOption(int direction)
    {
        SpeechWindowContainer.GetChild(_conversationIndex).GetComponentInChildren<Text>().color = new Color(.5f, .5f, .5f);
        _conversationIndex += direction;
        if (_conversationIndex < 0) _conversationIndex = 0;
        if (_conversationIndex >= SpeechWindowContainer.childCount) _conversationIndex = SpeechWindowContainer.childCount -1;
        SpeechWindowContainer.GetChild(_conversationIndex).GetComponentInChildren<Text>().color = new Color(1, 1, 1);
    }

    internal void StartConversation(PersonInteractionScript personInteractionScript)
    {
        PersonActive = personInteractionScript;
        PersonActive.Status.MoveNext(Command.StartConversation);
        currentOptions = PersonActive.GetOptions(ref currentCommands);
        ShowMenu();
    }

    public bool ToggleInventory(Transform player)
    {
        InventoryPopup.gameObject.SetActive(!InventoryPopup.gameObject.activeInHierarchy);
        var popupPos = player.position;
        popupPos.x -= .75f;
        popupPos.y += 1.5f;

        var screenPos = Camera.main.WorldToScreenPoint(popupPos);
        InventoryPopup.position = screenPos;
        return InventoryPopup.gameObject.activeInHierarchy;
    }

    private void ShowMenu()
    {
        _conversationIndex = 0;
        RemoveMenuOptions();
        int originalChildCount = SpeechWindowContainer.childCount;

        foreach (var v in currentOptions)
        {
            var speech = Instantiate(SpeechOptionPrefab, new Vector3(0, 0, 0), Quaternion.identity, SpeechWindowContainer);
            speech.GetComponentInChildren<Text>().text = v;
        }

        // select first option
        if (currentOptions.Length > 0)
        {
            int r = SpeechWindowContainer.childCount;
            SpeechWindowContainer.GetChild(originalChildCount).GetComponentInChildren<Text>().color = new Color(1, 1, 1);
        }

        RepositionWindows();
    }

    private void RemoveMenuOptions()
    {
        // remove existing
        for (var v = 0; v < SpeechWindowContainer.childCount; v++)
        {
            Destroy(SpeechWindowContainer.GetChild(v).gameObject);
        }
    }

    private void ShowResponse()
    {
        _conversationIndex = 0;
        ConversationText.text = PersonActive.person.Name + " is talking";
        RepositionWindows();
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(true);
    }

    private void ShowTalking()
    {
        _conversationIndex = 0;
        ConversationText.text = "You are talking";
        RepositionWindows();
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(true);
    }

    private void Finished()
    {
        _conversationIndex = 0;
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(false);
        PersonActive.ConversationOver();
    }

    private void RepositionWindows()
    {
        var popupPos = PersonActive.transform.position;
        popupPos.x -= .75f;
        popupPos.y += 1.5f;

        var screenPos = Camera.main.WorldToScreenPoint(popupPos);
        PlayerConversation.transform.position = screenPos;
        ConversationWindow.position = screenPos;
        PlayerConversation.gameObject.SetActive(true);
        ConversationWindow.gameObject.SetActive(false);
    }

}
