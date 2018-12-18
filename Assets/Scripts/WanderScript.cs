using System;
using System.Collections;
using UnityEngine;

public class WanderScript : MonoBehaviour
{
    public PersonInteractionScript InteractionScript;

    public float MinX = 0;
    public float MaxX = 0;

    private float _destinationX = 0;
    private bool _onTheMove = false;

    public SpriteRenderer _renderer;

    public float Speed = 1;


    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        NewDestination();
    }

    private void NewDestination()
    {
        _destinationX = UnityEngine.Random.Range(MinX, MaxX);
        _onTheMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        DoMovement();
    }

    private void DoMovement()
    {
        if (_onTheMove && !InteractionScript.IsInteracting())
        {
            bool flip = false;
            float direction = .8f;

            if (transform.position.x > _destinationX)
            {
                flip = true;
                direction = -.8f;
            }

            if (Mathf.Abs(transform.position.x - _destinationX) < 0.5f)
            {
                _onTheMove = false;
                StartCoroutine(Wait());
            }

            _renderer.flipX = flip;
            transform.Translate(Speed * new Vector3(direction, 0, 0) * Time.deltaTime);
        }
    }

    private IEnumerator Wait()
    {
        float random = UnityEngine.Random.Range(5f, 25f);
        yield return new WaitForSeconds(random);
        NewDestination();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Person")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
