using UnityEngine;

public class ItemDropScript : MonoBehaviour
{
    BoxCollider2D _boxCollider;
    BoxCollider2D _playerCollider;
    Quaternion _rotation;

    // Use this for initialization
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();

        _rotation = transform.rotation;
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerCollider = player.GetComponentInChildren<BoxCollider2D>();
        Physics2D.IgnoreCollision(_playerCollider, _boxCollider);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Ground"))
        {
            _boxCollider.isTrigger = true;
            Destroy(GetComponent<Rigidbody2D>());
            //transform.Translate(new Vector3(0, 0.01f, 0));
            Physics2D.IgnoreCollision(_playerCollider, _boxCollider, false);
        }
    }
}
