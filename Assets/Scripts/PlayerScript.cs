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
    Collider2D _boxCollider;
    Rigidbody2D _rigidBody;
    private int _coins;
    Quaternion _rotation;
    public AudioSource Sounds;
    public ClimbingScript ClimbingScript;

    private List<ObjectType> _collectedObjects = new List<ObjectType>();
    private List<GameObject> _itemsInBounds = new List<GameObject>();
    Collider2D _activeObstacle;

    public Sprite[] PunchImages;

    // state
    bool _onGround = false;
    bool _alive = true;
    bool _punching = false;
    bool _inventoryOpen = false;
    bool _walkOff = false;
    bool _isRunning = false;
    bool _bounceBack = false;
    bool _bounceBackBlock = false;

    public float Health = 100;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _rigidBody = GetComponentInChildren<Rigidbody2D>();
        _boxCollider = GetComponentInChildren<BoxCollider2D>();
        _rotation = transform.rotation;
    }

    internal bool IsAlive()
    {
        return _alive;
    }

    // Update is called once per frame
    void Update()
    {
        if (PersonInteractionScript.ConversationActive || _walkOff || _bounceBack || !_alive) return;

        if (Input.GetButtonDown("Inventory") || (Input.GetKeyDown(KeyCode.I)) && _onGround)
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
            if (Input.GetKeyDown(KeyCode.L) && !_isRunning && _onGround && !_punching)
            {
                Punch();
            }

            CheckMovement();
            transform.rotation = _rotation;
        }
    }

    private void Punch()
    {
        _punching = true;
            bool targetFound = false;

        foreach (var item in _itemsInBounds)
        {
            var person = item.GetComponentInChildren<PersonInteractionScript>();
            if (person != null)
            {
                targetFound = true;
                person.Punched(transform.position.x);
                _itemsInBounds.Remove(item.gameObject);
                // TODO: sound effect
                break;
            }
        }

        if (!targetFound)
        {
            foreach (var item in _itemsInBounds)
            {
                var punchable = item.GetComponentInChildren<PunchableObject>();
                if (punchable != null)
                {
                    punchable.Punched();
                    // TODO: sound effect
                    break;
                }
            }
        }

        StartCoroutine(PunchAnim());
    }

    private IEnumerator PunchAnim()
    {
        _animator.enabled = false;
        _renderer.sprite = PunchImages[0];
        yield return new WaitForSeconds(0.1f);
        _renderer.sprite = PunchImages[1];
        yield return new WaitForSeconds(0.1f);
        _animator.enabled = true;
        _punching = false;
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
                    transform.position = new Vector3(sorted[i].transform.position.x + sorted[i].transform.localScale.x / 2 + 0.1f, transform.position.y, transform.position.z);
                    _renderer.flipX = true;
                }
                else
                {
                    transform.position = new Vector3(sorted[i].transform.position.x - transform.localScale.x / 2 - 0.1f, transform.position.y, transform.position.z);
                    _renderer.flipX = false;
                }
                sorted[i].GetComponent<PersonInteractionScript>().Converse(transform);
                break;
            }
        }
    }

    internal void DropItem(ObjectType obj, bool crafting)
    {
        for (uint i = 0; i < _collectedObjects.Count; i++)
        {
            if (_collectedObjects[(int)i] == obj)
            {
                // instantiate
                if (!crafting)
                {
                    var creator = GameObject.Find("CollectableCreator").GetComponent<CollectableCreationScript>();
                    //creator.Create(_collectedObjects.ElementAt((int)i), new Vector3(transform.position.x, transform.position.y, -10), 4);
                    creator.Create(_collectedObjects.ElementAt((int)i), transform.position, 4);
                }
                _collectedObjects.RemoveAt((int)i);
                break;
            }
        }
    }

    internal void ItemCollected(ObjectType item)
    {
        _collectedObjects.Add(item);
    }

    private void PickUp()
    {
        // collect items
        for (int i = 0; i < _itemsInBounds.Count; i++)
        {
            var renderer = _itemsInBounds[i].GetComponent<SpriteRenderer>();
            if (_itemsInBounds[i].tag == "Collectable" && renderer.enabled)
            {
                _collectedObjects.Add(_itemsInBounds[i].GetComponent<CollectableObject>().ObjectType);
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

    internal void StopMomentum(bool triggers)
    {
        if (triggers)
        {
            _animator.ResetTrigger("Jump");
            _animator.ResetTrigger("Run");
            _animator.SetTrigger("Climb");
        }
        _rigidBody.velocity = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Person")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }

        if ((collision.transform.tag.Contains("Ground") || collision.transform.tag == "Platform") && collision.relativeVelocity.y > -1.5f)
        {
            if (_bounceBack && !_bounceBackBlock && collision.transform.tag.Contains("Ground"))
            {
                Debug.Log("All Better thanks to " + collision.gameObject.name);
                _bounceBack = false;
                _renderer.color = new Color(1, 1, 1);
                Physics2D.IgnoreCollision(_activeObstacle, _boxCollider, false);
            }

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

    internal List<IGrouping<ObjectType, ObjectType>> GetCollectedItemsGrouped()
    {
        var grouped = _collectedObjects.GroupBy(i => i);
        return grouped.OrderBy(i => i.ToList().First()).ToList();
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
        else if (collision.tag == "Obstacle")
        {
            _activeObstacle = collision.GetComponent<Collider2D>();
            Stun(collision.gameObject.GetComponent<ObstacleScript>(), _renderer.flipX ? 1 : -1);
        }
        else if (collision.tag == "Coin")
        {
            CoinCollected(collision.gameObject);
        }
    }

    private void CoinCollected(GameObject gameObject)
    {
        _coins++;
        gameObject.GetComponentInChildren<CoinScript>().Pickup();
        UIScript.Instance().TxtCoins.text = _coins.ToString();
        if (_itemsInBounds.Contains(gameObject))
        {
            _itemsInBounds.Remove(gameObject);
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

        if (ClimbingScript.IsClimbing())
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
            _rigidBody.AddForce(new Vector3(0, 11000, 0));
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
        while (_renderer.color.a > 0)
        {
            _renderer.color = new Color(1, 1, 1, _renderer.color.a - 0.02f);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(MusicScript.Instance);
        MusicScript.Playing = false;
        SceneManager.LoadScene(scene);
    }

    void Stun(ObstacleScript obstacle, int multiplier)
    {
        Health -= obstacle.Damage;
        if (Health > 0)
        {
            StopMomentum(false);
            StartCoroutine(WaitAfterBounceback());
            StartCoroutine(ResetAfterBounceback());
            Physics2D.IgnoreCollision(_activeObstacle, _boxCollider);
            _bounceBack = true;
            _rigidBody.AddForce(new Vector2(multiplier * 3500, 8000));
            _renderer.color = new Color(0.8f, 0.2f, 0.2f);
        }
        else
        {
            Die();
        }

        var healthPerc = Health / 100f;

        UIScript.Instance().HealthBar.fillAmount = healthPerc;
    }

    private void Die()
    {
        _animator.SetTrigger("Dead");
        _alive = false;
        _rigidBody.AddForce(new Vector2(0, 8000));
        UIScript.Instance().DeadScreen.SetActive(true);
    }

    private IEnumerator WaitAfterBounceback()
    {
        // need to wait to avoid hitting floor straight away
        _bounceBackBlock = true;
        yield return new WaitForSeconds(0.1f);
        _bounceBackBlock = false;
    }

    private IEnumerator ResetAfterBounceback()
    {
        // need to wait to avoid hitting floor straight away
        yield return new WaitForSeconds(2f);
        _bounceBackBlock = false;
        _bounceBack = false;
        _renderer.color = new Color(1, 1, 1);
        Physics2D.IgnoreCollision(_activeObstacle, _boxCollider, false);
    }
}
