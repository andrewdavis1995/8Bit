using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Transform Target;
    public float MinX;
    public float MaxX;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(Target.position.x, Target.position.y+.7f, -100);
    }

    // Update is called once per frame
    void Update()
    {
        var x = transform.position.x;
        if (Target.position.x > MinX && Target.position.x < MaxX)
        {
            x = Target.position.x;
        }
        transform.position = new Vector3(x, Target.position.y+.7f, -100);
    }
}
