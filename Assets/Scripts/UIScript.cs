using Assets.Classes;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject PlayerConversation;
    public PersonInteractionScript PersonActive;

    public Sprite[] Collectables;

    public PlayerScript Player;

    public Transform SpeechOptionPrefab;
    public Transform SpeechWindowContainer;
    public Transform ConversationWindow;
    public Text ConversationText;
    public Transform InventoryPopup;

    private static UIScript _instance;
    
    private int _conversationIndex = 0;
    string[] currentOptions = new string[] { };
    Command[] currentCommands = new Command[] { };

    private int _inventoryIndex = 0;
    private const int INVENTORY_ITEMS = 9;



    public static UIScript Instance() { return _instance; }

    public UIScript() { _instance = this; }

    private void Update()
    {
        if (PersonInteractionScript.ConversationActive)
        {
            HandleConversationInput();
        }
        else if (InventoryOpen())
        {
            HandleInventoryInput();
        }
    }

    private void HandleConversationInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RemoveMenuOptions();
            PersonActive.Status.MoveNext(currentCommands[_conversationIndex]);
            PersonActive.GetOptions(ref currentCommands, ref currentOptions);

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
        else if (Input.GetKeyDown(KeyCode.UpArrow) && PersonActive.Status.AllowsUpDown())
        {
            ChangeOption(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && PersonActive.Status.AllowsUpDown())
        {
            ChangeOption(1);
        }
    }

    private void HandleInventoryInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            InventoryIndexChanged(-3);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            InventoryIndexChanged(3);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InventoryIndexChanged(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InventoryIndexChanged(-1);
        }
    }

    public void InventoryIndexChanged(int changed)
    {
        // reset previously selected one
        var item = InventoryPopup.GetChild(0).GetChild(_inventoryIndex);
        var image = item.GetComponentInChildren<Image>();
        image.color = new Color(1, 1, 1);
        if (_inventoryIndex >= Player.GetCollectedItemsGrouped().Count())
        {
            item.GetComponentsInChildren<Image>()[1].color = new Color(0, 0, 0, 0);
            image.color = new Color(0.2f, 0.2f, 0.2f);
        }
        else
        {
            item.GetComponentsInChildren<Image>()[1].color = new Color(1,1,1,1);
        }

        _inventoryIndex += changed;

        if (_inventoryIndex < 0) _inventoryIndex += INVENTORY_ITEMS;
        if (_inventoryIndex >= INVENTORY_ITEMS) _inventoryIndex -= INVENTORY_ITEMS;

        // update new one
        var item2 = InventoryPopup.GetChild(0).GetChild(_inventoryIndex);
        var image2 = item2.GetComponentInChildren<Image>();
        image2.color = new Color(0.4f, 0.6f, 0.8f);

        var txtDescription = InventoryPopup.GetChild(1).GetComponent<Text>();
        var strOutput = " --- ";
        if (_inventoryIndex < Player.GetCollectedItemsGrouped().Count)
        {
            var items = Player.GetCollectedItemsGrouped()[_inventoryIndex];
            strOutput = items.First().ItemName;
        }
        txtDescription.text = strOutput;
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
        PersonActive.GetOptions(ref currentCommands, ref currentOptions);
        currentCommands = new Command[] { Command.Exit };
        RepositionWindows(false);
        ShowResponse();
    }

    void ChangeOption(int direction)
    {
        SpeechWindowContainer.GetChild(_conversationIndex).GetComponentInChildren<Text>().color = new Color(.5f, .5f, .5f);
        _conversationIndex += direction;

        // wrap around
        if (_conversationIndex < 0) _conversationIndex = SpeechWindowContainer.childCount-1;
        if (_conversationIndex >= SpeechWindowContainer.childCount) _conversationIndex = 0;

        SpeechWindowContainer.GetChild(_conversationIndex).GetComponentInChildren<Text>().color = new Color(1, 1, 1);
    }

    internal void StartConversation(PersonInteractionScript personInteractionScript)
    {
        PersonActive = personInteractionScript;
        PersonActive.Status.CurrentState = ProcessState.Inactive;
        PersonActive.Status.MoveNext(Command.StartConversation);
        PersonActive.GetOptions(ref currentCommands, ref currentOptions);
        ShowMenu();
    }

    public bool ToggleInventory(Transform player)
    {
        var playerScript = player.gameObject.GetComponent<PlayerScript>();

        var grouped = playerScript.GetCollectedItemsGrouped();

        for (int i = 0; i < grouped.Count && i < InventoryPopup.GetChild(0).childCount; i++)
        {
            var display = InventoryPopup.GetChild(0).GetChild(i);
            display.GetComponentInChildren<Text>().text = grouped[i].Count().ToString();
            display.GetComponentInChildren<Image>().color = new Color(1, 1, 1);
            display.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
            display.GetComponentsInChildren<Image>()[1].sprite = GetCollectableImage(grouped[i].Key);
        }

        for (int i = grouped.Count; i < InventoryPopup.GetChild(0).childCount; i++)
        {
            var display = InventoryPopup.GetChild(0).GetChild(i);
            display.GetComponentInChildren<Image>().color = new Color(0.2f, 0.2f, 0.2f);
            display.GetComponentsInChildren<Image>()[1].color = new Color(0, 0, 0, 0);
        }

        InventoryPopup.gameObject.SetActive(!InventoryPopup.gameObject.activeInHierarchy);
        var popupPos = player.position;
        popupPos.x -= .75f;
        popupPos.y += 1.5f;

        InventoryIndexChanged(0);

        var screenPos = Camera.main.WorldToScreenPoint(popupPos);
        InventoryPopup.position = screenPos;
        return InventoryPopup.gameObject.activeInHierarchy;
    }

    public bool InventoryOpen()
    {
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

        RepositionWindows(true);
        PlayerConversation.gameObject.SetActive(true);
        ConversationWindow.gameObject.SetActive(false);
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
        ConversationText.text = GetResponse();
        _conversationIndex = 0;
        RepositionWindows(false);
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(true);
    }

    private string GetResponse()
    {
        switch (PersonActive.Status.PreviousState)
        {
            case ProcessState.AskingQuestion:
                return GetQuestionAnswer();
            case ProcessState.TellingCompliment:
                return GetComplimentResponse();
            case ProcessState.TellingJoke:
                return GetJokeResponse();
        }

        return "I don't know what to say";
    }

    private string GetComplimentResponse()
    {
        // TODO: factor in feelings/personality
        return "That's so nice!";
    }

    private string GetJokeResponse()
    {
        return "Hahahaha that's hilarious!";
    }

    private string GetQuestionAnswer()
    {
        var response = PersonActive.CheckQuestionAnswer();
        return response;
    }

    private void ShowTalking()
    {
        ConversationText.text = GetSpeechOutput();
        _conversationIndex = 0;
        RepositionWindows(true);
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(true);
    }

    private string GetSpeechOutput()
    {
        switch (PersonActive.Status.PreviousState)
        {
            case ProcessState.BrowsingQuestions:
                // who started that fire?
                {
                    var knowledgeCat = KnowledgeCategory.HowOldAreYou;
                    var output = ConversationFetcher.GetQuestion(currentOptions[_conversationIndex].Trim(), ref knowledgeCat);
                    PersonActive.SetQuestionType(knowledgeCat);
                    return output;
                }
            case ProcessState.SelectingCompliment:
                return ConversationFetcher.GetCompliment(currentOptions[_conversationIndex].Replace("Compliment", "").Trim());
            case ProcessState.SelectingJoke:
                return ConversationFetcher.GetJoke(currentOptions[_conversationIndex].Replace("Joke", "").Trim());
        }

        return "Erm... Words...?";
    }

    private void Finished()
    {
        _conversationIndex = 0;
        PlayerConversation.gameObject.SetActive(false);
        ConversationWindow.gameObject.SetActive(false);
        PersonActive.ConversationOver();
    }

    private void RepositionWindows(bool usePlayerPosition)
    {
        var popupPos = PersonActive.transform.position;
        popupPos.x -= .75f;
        popupPos.y += 1.5f;

        var popupPos2 = Player.transform.position;
        popupPos2.x -= .75f;
        popupPos2.y += 1.5f;

        var screenPos = Camera.main.WorldToScreenPoint(popupPos);
        var screenPos2 = Camera.main.WorldToScreenPoint(popupPos2);

        if (usePlayerPosition)
            ConversationWindow.position = screenPos2;
        else
        ConversationWindow.position = screenPos;
            PlayerConversation.transform.position = screenPos;
    }

    Sprite GetCollectableImage(ObjectType obj)
    {
        switch (obj)
        {
            case ObjectType.Wood: return Collectables[0];
            case ObjectType.Battery: return Collectables[1];
            case ObjectType.Stone: return Collectables[2];
            case ObjectType.Tank: return Collectables[3];
        }
        return Collectables[0];
    }

}
