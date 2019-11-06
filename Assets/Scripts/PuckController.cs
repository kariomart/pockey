using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckController : MonoBehaviour
{

    SpriteRenderer spr;
    Rigidbody2D rb;
    Collider2D coll;

    public Vector2 vel;
    public float maxSpd;
    public float spd;
    public float damping;
    bool controlled;

    Transform stick;
    public PlayerController playerControllingPuck;



    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {

        if (!controlled) {
            spd *= damping;
            rb.MovePosition((Vector2)transform.position + vel * spd);
        } else {
            transform.position = stick.position;
        }

    }

    public void Shoot(float inheritedVel, Vector2 dir) {
        controlled = false;
        playerControllingPuck = null;
        vel = dir;
        spd = inheritedVel + maxSpd;
    }

    public void Control(Transform stick) {
        this.stick = stick;
        controlled = true;
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.8f);
        }

    }

    void OnTriggerEnter2D(Collider2D coll) {

        if (coll.gameObject.tag == "Goal") {
            Master.me.ResetPuck();
        }
    }
}
