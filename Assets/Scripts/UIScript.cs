using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public GameObject PlayerConversation;
    public static bool ConversationMenuActive = false;
    public PersonInteractionScript PersonActive;

    public Transform SpeechOptionPrefab;
    public Transform SpeechWindowContainer;
    public Transform ConversationWindow;
    public Text ConversationText;
    public Transform InventoryPopup;

    private static UIScript _instance;

    private bool _messageDisplayed = false;

    private int _conversationIndex = 0;
    string[] currentOptions = new string[] { "Do you study?", "Wanna hear a joke?", "Look at these rocks", "Gloagburn sucks", "Goodbye" };

    public static UIScript Instance() { return _instance; }

    public UIScript() { _instance = this; }

    private void Update()
    {
        if (ConversationMenuActive)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                // remove existing
                for (var v = 0; v < SpeechWindowContainer.childCount; v++)
                {
                    Destroy(SpeechWindowContainer.GetChild(v).gameObject);
                }

                if (currentOptions[_conversationIndex] == "Goodbye")
                {
                    ConversationMenuActive = false;
                    PlayerConversation.SetActive(false);
                    PersonActive.ConversationOver();
                }
                else if (!_messageDisplayed)
                {
                    PlayerConversation.SetActive(false);
                    ConversationWindow.gameObject.SetActive(true);
                    ConversationText.text = GetResponse();
                    _messageDisplayed = true;
                }
                else
                {
                    _messageDisplayed = false;
                    ShowMenu();
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

    public string GetResponse()
    {
        return "I have no response";
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
        // remove existing
        for (var v = 0; v < SpeechWindowContainer.childCount; v++)
        {
            Destroy(SpeechWindowContainer.GetChild(v).gameObject);
        }

        foreach (var v in currentOptions)
        {
            var speech = Instantiate(SpeechOptionPrefab, new Vector3(0, 0, 0), Quaternion.identity, SpeechWindowContainer);
            speech.GetComponentInChildren<Text>().text = v;
        }

        // select first option
        if (currentOptions.Length > 0)
        {
            SpeechWindowContainer.GetChild(0).GetComponentInChildren<Text>().color = new Color(1, 1, 1);
        }

        ConversationMenuActive = true;


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
