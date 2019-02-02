using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static bool Playing;
    public static GameObject Instance;

    void Awake()
    {
        Instance = GameObject.Find("Music");

        DontDestroyOnLoad(gameObject);

        if (Playing)
            Destroy(gameObject);

        Playing = true;
    }
}
