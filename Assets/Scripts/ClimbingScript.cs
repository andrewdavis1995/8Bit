using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingScript : MonoBehaviour
{
    private bool _climbing = false;
    public PlayerScript PlayerScript;

    ClimbableObject _climbingObject;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckClimb();
    }

    public bool IsClimbing() { return _climbing; }
    public void IsClimbing(bool state) { _climbing = state; }

    private void CheckClimb()
    {
        if (_climbingObject != null && PlayerScript.IsAlive())
        {
            if (Input.GetKey(KeyCode.UpArrow) && transform.position.y < _climbingObject.TopPosition)
            {
                PlayerScript.StopMomentum(true);
                _climbing = true;
                PlayerScript.Climb(); 
                transform.Translate(new Vector3(0, 2 * Time.deltaTime, 0));
                transform.position = new Vector3(_climbingObject.CentralXposition, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                if (transform.position.y > _climbingObject.BottomYPosition)
                {
                    PlayerScript.StopMomentum(true);
                    _climbing = true;
                    PlayerScript.Climb();
                    transform.Translate(new Vector3(0, -2 * Time.deltaTime, 0));
                    transform.position = new Vector3(_climbingObject.CentralXposition, transform.position.y, transform.position.z);
                }
            }
        }
    }
    
    internal void ClimbZoneEntered(Collider2D collision)
    {
        _climbingObject = collision.GetComponentInChildren<ClimbableObject>();
    }

    internal void StopClimbing()
    {
        _climbing = false;
        _climbingObject = null;
    }
}
