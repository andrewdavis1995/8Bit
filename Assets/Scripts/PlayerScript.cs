using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    Animator _animator;
    SpriteRenderer _renderer;
    Rigidbody2D _rigidBody;
    Quaternion _rotation;
    public AudioSource Sounds;
    public ClimbingScript ClimbingScript;
    bool _isRunning = false;

    private List<CollectableObject> _collectedObjects = new List<CollectableObject>();
    private List<GameObject> _itemsInBounds = new List<GameObject>();

    bool _onGround = false;
    bool _inventoryOpen = false;

    bool _walkOff = false;

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
        if (PersonInteractionScript.ConversationActive && _walkOff) return;

        if (Input.GetButtonDown("Inventory")||(Input.GetKeyDown(KeyCode.I)) && _onGround)
        {
            _inventoryOpen = UIScript.Instance().ToggleInventory(transform);
        }

        if (!_inventoryOpen)
        {
            if (Input.GetButtonDown("Pickup"))
            {
                PickUp();
            }
            if (Input.GetKeyDown(KeyCode.T) && !_isRunning && _onGround)
            {
                Talk();
            }
            if (Input.GetKeyDown(KeyCode.L) && !_isRunning && _onGround)
            {
                Punch();
            }

            CheckMovement();
            transform.rotation = _rotation;
        }
    }

    private void Punch()
    {
        foreach(var item in _itemsInBounds)
        {
            var punchable = item.GetComponentInChildren<PunchableObject>();
            if(punchable != null)
            {
                punchable.Punched();
                break;
            }
        }
    }

    private void Talk()
    {
        var sorted = _itemsInBounds.OrderBy(i => Math.Abs(i.transform.position.x - transform.position.x)).ToList();

        // anyone to talk to?
        for (int i = 0; i < sorted.Count; i++)
        {
            if (sorted[i].tag == "Person")
            {
                _animator.ResetTrigger("Run");
                _animator.SetTrigger("Stop");
                if (sorted[i].transform.position.x < transform.position.x)
                {
                    transform.position = new Vector3(sorted[i].transform.position.x + sorted[i].transform.localScale.x/2 +0.1f, transform.position.y, transform.position.z);
                    _renderer.flipX = true;
                }
                else
                {
                    transform.position = new Vector3(sorted[i].transform.position.x - transform.localScale.x/2 -0.1f, transform.position.y, transform.position.z);
                    _renderer.flipX = false;
                }
                sorted[i].GetComponent<PersonInteractionScript>().Converse(transform);
                break;
            }
        }
    }

    internal void DropItem(CollectableObject obj)
    {
        for(uint i = 0; i < _collectedObjects.Count; i++)
        {
            if(_collectedObjects[(int)i] == obj)
            {
                // instantiate
                var creator = GameObject.Find("CollectableCreator").GetComponent<CollectableCreationScript>();
                creator.Create(_collectedObjects.ElementAt((int)i).ObjectType, transform.position, 4);
                _collectedObjects.RemoveAt((int)i);
                break;
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

        if ((collision.transform.tag.Contains("Ground") || collision.transform.tag == "Platform")&& collision.relativeVelocity.y > -1.5f)
        {
            _onGround = true;
            if (collision.transform.tag == "Platform")
            {
                transform.parent = collision.transform;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || JoystickMovementLeft() || JoystickMovementRight())
            {
                _animator.SetTrigger("Run");
            }
            else
            {
                _animator.SetTrigger("Stop");
            }
        }

        if (collision.gameObject.tag == "TransportWall")
        {
            collision.gameObject.GetComponent<TransportController>().Collided();
        }
    }

    internal List<IGrouping<ObjectType, CollectableObject>> GetCollectedItemsGrouped()
    {
        var grouped = _collectedObjects.GroupBy(i => i.ObjectType);
        return grouped.OrderBy(i => i.ToList().First().ObjectType).ToList();
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
        else if (collision.tag == "Door")
        {
            collision.gameObject.GetComponent<DoorScript>().StartOpen(this);
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
        else if (collision.tag == "Door")
        {
            collision.gameObject.GetComponent<DoorScript>().StartClose();
        }
    }

    bool JoystickMovementLeft()
    {
        return Input.GetAxis("Horizontal") < 0f;
    }

    bool JoystickMovementRight()
    {
        return Input.GetAxis("Horizontal") > 0f;
    }
    bool JoystickMovementUp()
    {
        return Input.GetAxis("Vertical") < 0f;
    }


    private void CheckMovement()
    {
        if (!ClimbingScript.IsClimbing())
        {
            float speed = 4;
            if (JoystickMovementLeft() || JoystickMovementRight()) speed *= Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.RightArrow) || JoystickMovementRight())
            {
                _isRunning = true;
                if (_onGround && _animator.GetBool("Run") == false)
                {
                    _animator.SetTrigger("Run");
                }
                _renderer.flipX = false;

                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || JoystickMovementLeft())
            {
                if (speed > 0) speed *= -1;
                _isRunning = true;
                if (_onGround && _animator.GetBool("Run") == false)
                {
                    _animator.SetTrigger("Run");
                }
                _renderer.flipX = true;
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
            else
            {
                _isRunning = false;
                if (_onGround)
                    _animator.SetTrigger("Stop");
            }
        }

        if(ClimbingScript.IsClimbing())
        {
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Space) || JoystickMovementLeft() || JoystickMovementRight())
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

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump")) && _onGround)
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

    public bool Grounded() { return _onGround; }

    public void WalkThroughDoor(float xPosition, int scene)
    {
        _walkOff = true;
        transform.position = new Vector3(xPosition, transform.position.y, transform.position.z);
        StartCoroutine(Disappear(scene));
    }

    private IEnumerator Disappear(int scene)
    {
        while(_renderer.color.a > 0)
        {
            _renderer.color = new Color(1, 1, 1, _renderer.color.a - 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene(scene);
    }
}
