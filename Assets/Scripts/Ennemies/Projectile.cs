using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    [SerializeField] private float maxLifeTime = 5;

    private float lifeTime = 0;
    
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Check to don't touch self
        if (lifeTime > 0.1f)
        {
            Destroy(gameObject);
        }

    }
}
