using System;
using Assets.Classes;

using UnityEngine;
using UnityEngine.UI;

public class PersonInteractionScript : MonoBehaviour
{
    public Person person;
    private SpriteRenderer _renderer;
    private bool _isInteracting = false;

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
    }

    public void Converse(Transform playerPosition)
    {
        var uiController = GameObject.Find("UI Controller");
        _isInteracting = true;
        uiController.GetComponent<UIScript>().StartConversation(this);
        if(playerPosition.position.x < transform.position.x)
        {
            _renderer.flipX = true;
        }
        else
        {
            _renderer.flipX = false;
        }
    }
}