using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCreationScript : MonoBehaviour
{
    public Transform[] Transforms;

    public void Create(ObjectType item, Vector3 position, int xAdd = 0)
    {
        var prefab = GetPrefab(item);
        if (prefab != null)
        {
            Create(prefab, position, item);
        }
        else
        {
            Debug.LogAssertion("Prefab is null and cannot be created");
        }
    }

    public void Create(Transform prefab, Vector3 position, ObjectType item = ObjectType.None, int xAdd = 0)
    {
        var obj = Instantiate(prefab, new Vector3(position.x, position.y, 2), Quaternion.identity);

        if (item == ObjectType.Ladder)
        {
            obj.GetComponent<ClimbableObject>().CentralXposition = obj.transform.position.x + 0.02f;
            obj.GetComponent<ClimbableObject>().BottomYPosition = obj.transform.position.y -1f;
            obj.transform.Translate(new Vector3(0, 4, 0));
        }
        else
        {
            obj.GetComponent<Rigidbody2D>().AddForce(new Vector3(Random.Range(-15f, 15f) + xAdd, 200, -10f));
        }
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
            case ObjectType.Ladder:
                transform = Transforms[4];
                break;
        }
        return transform;
    }

}
