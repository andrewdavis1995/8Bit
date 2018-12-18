using UnityEngine;

public class PatrolScript : MonoBehaviour
{
    public float MinX = 0;
    public float MaxX = 0;

    public PersonInteractionScript InteractionScript;

    private bool _movingLeft = false;
    public SpriteRenderer _renderer;

    public float Speed = 1;


    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!InteractionScript.IsInteracting())
        {
            if (_movingLeft)
            {
                _renderer.flipX = true;
                transform.Translate(-Speed * new Vector3(1, 0, 0) * Time.deltaTime);
                if (transform.position.x <= MinX)
                {
                    _movingLeft = false;
                }
            }
            else
            {
                _renderer.flipX = false;
                transform.Translate(Speed * new Vector3(1, 0, 0) * Time.deltaTime);
                if (transform.position.x >= MaxX)
                {
                    _movingLeft = true;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Person")
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
