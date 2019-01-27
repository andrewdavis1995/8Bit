using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCreationScript : MonoBehaviour
{
    public Transform[] Transforms;

    public void Create(ObjectType item, Vector3 position, int xAdd = 0)
    {
        var prefab = GetPrefab(item);
        Create(prefab, position);
    }

    public void Create(Transform prefab, Vector3 position, int xAdd = 0)
    {
        var obj = Instantiate(prefab, position, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-15f, 15f) + xAdd, 200, -10f));
    }

    private Transform GetPrefab(ObjectType obj)
    {
        Transform transform = null;
        switch (obj)
        {
            case ObjectType.Battery:
                transform = Transforms[0];
                break;
            case ObjectType.Stone:
                transform = Transforms[1];
                break;
            case ObjectType.Tank:
                transform = Transforms[2];
                break;
            case ObjectType.Wood:
                transform = Transforms[3];
                break;
        }
        return transform;
    }

}
