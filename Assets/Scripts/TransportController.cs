using UnityEngine;
using UnityEngine.SceneManagement;

public class TransportController : MonoBehaviour
{
    public int SceneIndex;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Collided()
    {
        if (SceneIndex > -1)
        {
            SceneManager.LoadScene(SceneIndex);
        }
    }
}
