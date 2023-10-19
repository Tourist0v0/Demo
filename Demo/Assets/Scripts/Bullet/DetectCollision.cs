using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.CompareTag("Player") && other.gameObject.layer != gameObject.layer) Destroy(gameObject);
        if(other.gameObject.CompareTag("Sentry")) Destroy(gameObject);
        if(other.gameObject.CompareTag("Tower")) Destroy(gameObject);
    }
}
