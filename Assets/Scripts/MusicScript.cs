using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static bool Playing;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Playing)
            Destroy(gameObject);

        Playing = true;
    }
}
