using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public AudioSource Audio;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pickup()
    {
        StartCoroutine(Kill());
    }

    private IEnumerator Kill()
    {
        transform.position = new Vector3(-10000, 10000, 0);
        Audio.Play();
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
}
