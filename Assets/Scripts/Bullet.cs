using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 40;

    //public Image bulletProjectile;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.up * speed;
    }

    void OnTriggerEnter(Collider hitinfo)
    {

        Asteroid asteroid = hitinfo.GetComponent<Asteroid>();
        if(asteroid != null)
        {
            asteroid.TakeDamage(damage);
            Destroy(gameObject);
        }
        
    }

}