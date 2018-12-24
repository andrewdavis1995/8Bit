using UnityEngine;

public class CollectableObject : MonoBehaviour
{
    public string ItemName;
    public ObjectType ObjectType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }


}

public enum ObjectType
{
    Battery,
    Stone,
    Wood
}
