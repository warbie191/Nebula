using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 40;
    public Rigidbody2D rb;
    //public Image bulletProjectile;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.right * speed;
    }

    void OnTriggerEnter2D(Collider2D hitinfo)
    {

        Asteroid asteroid = hitinfo.GetComponent<Asteroid>();
        if(asteroid != null)
        {
            asteroid.TakeDamage(damage);
        }
        Debug.Log(hitinfo.name);
        Destroy(gameObject);
    }

}