using UnityEngine;

public class ItemDropScript : MonoBehaviour
{

    Quaternion _rotation;

    // Use this for initialization
    void Start()
    {
        _rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = _rotation;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            Destroy(GetComponent<Rigidbody2D>());
            transform.Translate(new Vector3(0, 0.02f, 0));
        }
    }
}
