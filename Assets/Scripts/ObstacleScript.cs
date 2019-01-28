using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScript : MonoBehaviour
{
    public float Damage;
    public float Health;

    public void Damaged(float damage)
    {
        Health -= damage;
    }
}
