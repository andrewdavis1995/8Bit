using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    Animator _animator;
    SpriteRenderer _renderer;
    Rigidbody2D _rigidBody;
    Quaternion _rotation;
    public AudioSource Sounds;
    public ClimbingScript ClimbingScript;

    private List<CollectableObject> _collectedObjects = new List<CollectableObject>();
    private List<GameObject> _itemsInBounds = new List<GameObject>();

    bool _onGround = false;
    bool _inventoryOpen = false;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _rigidBody = GetComponentInChildren<Rigidbody2D>();
        _rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (UIScript.ConversationMenuActive) return;

        if (Input.GetKeyDown(KeyCode.I) && _onGround)
        {
            var uiController = GameObject.Find("UI Controller").GetComponent<UIScript>();
            _inventoryOpen = uiController.ToggleInventory(transform);
        }

        if (!_inventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PickUp();
            }
            if (Input.GetKeyDown(KeyCode.T) && _onGround)
            {
                Talk();
            }

            CheckMovement();
            transform.rotation = _rotation;
        }
    }

    private void Talk()
    {
        // anyone to talk to?
        for (int i = 0; i < _itemsInBounds.Count; i++)
        {
            if (_itemsInBounds[i].tag == "Person")
            {
                _animator.ResetTrigger("Run");
                _animator.SetTrigger("Stop");
                _itemsInBounds[i].GetComponent<PersonInteractionScript>().Converse(transform);
            }
        }
    }

    private void PickUp()
    {
        // collect items
        for (int i = 0; i < _itemsInBounds.Count; i++)
        {
            var renderer = _itemsInBounds[i].GetComponent<SpriteRenderer>();
            if (_itemsInBounds[i].tag == "Collectable" && renderer.enabled)
            {
                _collectedObjects.Add(_itemsInBounds[i].GetComponent<CollectableObject>());
                 renderer.enabled = false;
                _itemsInBounds.RemoveAt(i);
            }
        }
    }

    internal void Climb()
    {
        _animator.ResetTrigger("Jump");
        _animator.ResetTrigger("Run");
        _animator.SetTrigger("Climb");
        _rigidBody.isKinematic = true;
    }

    internal void StopMomentum()
    {
        _animator.ResetTrigger("Jump");
        _animator.ResetTrigger("Run");
        _animator.SetTrigger("Climb");
        _rigidBody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Person")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }

        if ((collision.transform.tag == "Ground" || collision.transform.tag == "Platform")&& collision.relativeVelocity.y > -1.5f)
        {
            _onGround = true;
            if (collision.transform.tag == "Platform")
            {
                transform.parent = collision.transform;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                _animator.SetTrigger("Run");
            }
            else
            {
                _animator.SetTrigger("Stop");
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Platform")
        {
            transform.parent = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _itemsInBounds.Add(collision.gameObject);
        if (collision.tag == "Climbable")
        {
            ClimbingScript.ClimbZoneEntered(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _itemsInBounds.Remove(collision.gameObject);
        if (collision.tag == "Climbable")
        {
            _rigidBody.isKinematic = false;
            ClimbingScript.StopClimbing();
        }
    }

    private void CheckMovement()
    {
        if (!ClimbingScript.IsClimbing())
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if (_onGround)
                    _animator.SetTrigger("Run");
                _renderer.flipX = false;
                transform.Translate(new Vector3(4 * Time.deltaTime, 0, 0));
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (_onGround)
                    _animator.SetTrigger("Run");
                _renderer.flipX = true;
                transform.Translate(new Vector3(-4 * Time.deltaTime, 0, 0));
            }
            else
            {
                if (_onGround)
                    _animator.SetTrigger("Stop");
            }
        }

        if(ClimbingScript.IsClimbing())
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Space))
            {
                _animator.ResetTrigger("Climb");
                _animator.ResetTrigger("Stop");
                _animator.ResetTrigger("Run");
                _animator.ResetTrigger("Jump");
                ClimbingScript.IsClimbing(false);
                _rigidBody.isKinematic = false;
                _onGround = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && _onGround)
        {
            _animator.ResetTrigger("Climb");
            _animator.ResetTrigger("Stop");
            _animator.ResetTrigger("Run");
            _animator.SetTrigger("Jump");
            _onGround = false;
            _rigidBody.AddForce(new Vector3(0, 10000, 0));
            Sounds.Play();
        }
    }

}
