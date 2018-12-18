using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Directions
{
    Left, 
    Right, 
    Up, 
    Down, 
    Finished
}

public class PlatformScript : MonoBehaviour
{

    public bool Moving;
    public Vector2[] ExtremePoints;
    public uint _pointIndex = 0;
    public float Speed = 2.2f;
    public Directions _direction;

    // Use this for initialization
    void Start()
    {
        _direction = transform.position.x < ExtremePoints[_pointIndex].x ? Directions.Right : Directions.Left;
    }

    // Update is called once per frame
    void Update()
    {
        if (Moving)
        {
            if (_direction == Directions.Left)
            {
                if (transform.position.x > ExtremePoints[_pointIndex].x)
                {
                    transform.Translate(new Vector3(-Speed * Time.deltaTime, 0));
                }
                else
                {
                    _direction = transform.position.y < ExtremePoints[_pointIndex].y ? Directions.Up : Directions.Down;
                }
            }
            else if (_direction == Directions.Right)
            {
                if (transform.position.x < ExtremePoints[_pointIndex].x)
                {
                    transform.Translate(new Vector3(Speed * Time.deltaTime, 0));
                }
                else
                {
                    _direction = transform.position.y < ExtremePoints[_pointIndex].y ? Directions.Up : Directions.Down;
                }
            }
            else if (_direction == Directions.Up)
            {
                if (transform.position.y < ExtremePoints[_pointIndex].y)
                {
                    transform.Translate(new Vector3(0, Speed * Time.deltaTime));
                }
                else
                {
                    _direction = Directions.Finished;
                }
            }
            else if (_direction == Directions.Down)
            {
                if (transform.position.y > ExtremePoints[_pointIndex].y)
                {
                    transform.Translate(new Vector3(0, -Speed * Time.deltaTime));
                }
                else
                {
                    _direction = Directions.Finished;
                }
            }
            else if (_direction == Directions.Finished)
            {
                _pointIndex = _pointIndex == ExtremePoints.Length-1 ? 0 : _pointIndex+1;
                _direction = transform.position.x < ExtremePoints[_pointIndex].x ? Directions.Right : Directions.Left;
            }




            //else if (transform.position.x > ExtremePoints[_pointIndex].x - 1)
            //{
            //    transform.Translate(new Vector3(-Speed * Time.deltaTime, 0));
            //}
            //else if (transform.position.y < ExtremePoints[_pointIndex].y + 1)
            //{
            //    transform.Translate(new Vector3(0, -Speed * Time.deltaTime));
            //}
            //else if (transform.position.y > ExtremePoints[_pointIndex].y - 1)
            //{
            //    transform.Translate(new Vector3(0, Speed * Time.deltaTime));
            //}
            //else
            //{
            //    // next, or back to start
            //    _pointIndex = _pointIndex == ExtremePoints.Length ? 0 : _pointIndex++;
            //}
        }
    }
}
