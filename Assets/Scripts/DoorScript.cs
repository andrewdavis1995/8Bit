using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public Sprite[] Images;
    public SpriteRenderer Renderer;
    public int NextScene;

    private PlayerScript _player;

    bool _bOpen = false;
    bool _bOpening = false;
    bool _bClosing = false;
    uint _uiIndex = 0;


    void Start()
    {
        StartCoroutine(UpdateImage());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) && _bOpen)
        {
            _player.WalkThroughDoor(transform.position.x, NextScene);
        }
    }

    private IEnumerator UpdateImage()
    {
        while (true)
        {
            if (_bOpening)
            {
                _uiIndex++;
                if (_uiIndex == Images.Length - 1)
                {
                    _bOpening = false;
                    _bOpen = true;
                }
            }
            if (_bClosing)
            {
                _bOpen = false;
                _uiIndex--;
                if (_uiIndex == 0)
                {
                    _bClosing = false;
                }
            }

            Renderer.sprite = Images[_uiIndex];
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void StartOpen(PlayerScript player)
    {
        _player = player;
        _bOpening = true;
        _bClosing = false;
    }

    public void StartClose()
    {
        _player = null;
        _bOpening = false;
        _bClosing = true;
    }
}
