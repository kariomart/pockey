using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{

    public int playerId;
    private Player rewiredPlayer;

    Rigidbody2D rb;
    public SpriteRenderer spr;
    Collider2D coll;

    Vector2 lStickDir;
    Vector2 lastStickDir;
    Vector2 vel;
    public float accel;
    public float deaccel;
    public float speed;
    public float maxSpeed;
    public float damping;
    public float dashSpeed;

    public bool hasPuck;
    PuckController puck;
    public bool shooting;
    int framesSinceShot = 11;
    int shotTimer = 15;
    public float shootSpd;
    public float maxShootSpd;
    float framesSinceDash;
    public float dashTimer;

    public int livePoints;
    public int points;

    public AudioClip sfx_shoot;

    public GoalieController goal;



    // Start is called before the first frame update
    void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(playerId);
        rb = GetComponent<Rigidbody2D>();
        spr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetColor(Master.me.playerColors[playerId]);
        coll = transform.GetChild(0).GetComponent<CircleCollider2D>();

        // PlayerTuning tuning = Resources.Load<PlayerTuning>("MyTune");
        // jerk = tuning.jerk;
        // deaccel = tuning.deaccel;
        // speed = tuning.speed;
        // maxAccel = tuning.maxAccel;
        // maxSpeed = tuning.maxSpeed;
        // damping = tuning.damping;
        
    }

    // Update is called once per frame
    void Update()
    {

        if (rewiredPlayer.GetButtonDown("Restart")) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        lStickDir = new Vector2(rewiredPlayer.GetAxis("Move Horizontal"), rewiredPlayer.GetAxis("Move Vertical"));

        if (rewiredPlayer.GetButton("Swing")) {
            if (hasPuck) {
                shooting = true;
                shootSpd += maxShootSpd/80f;
                shootSpd = Mathf.Clamp(shootSpd, 0, maxShootSpd);
            }
        }

        if (rewiredPlayer.GetButtonDown("Swing") && !hasPuck) {
            Dash();
        }   


        if (rewiredPlayer.GetButtonUp("Swing") && hasPuck && shooting) {
            if (hasPuck) {
                ShootPuck();
            }
        }

        if (rewiredPlayer.GetButtonDown("Call Puck")) {
            // PuckController p = Master.me.puck;
            // p.Control(transform.GetChild(0).transform.GetChild(0).transform.GetChild(0));
            // p.playerControllingPuck.DropPuck();
            // ControlPuck(p);
        }

        if (rewiredPlayer.GetButtonDown("LeftFlap")) {
            Master.me.leftFlap.StartFlap();
        } 

        if (rewiredPlayer.GetButtonDown("RightFlap")) {
            Master.me.rightFlap.StartFlap();
        }
    }

    void FixedUpdate() {


        framesSinceShot ++;


        Vector2 moveDir = lStickDir.normalized; 
        if (moveDir != Vector2.zero) {
            speed+=accel;
            lastStickDir = moveDir;
            //transform.localEulerAngles = transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(lStickDir) - 90);
            transform.localEulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Geo.ToAng(vel) - 90);
        } else {
            speed -= deaccel;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
        }

        if (speed>=maxSpeed) {
            speed = maxSpeed;
        }

        vel += moveDir*speed;  
        if (!shooting) {   
            vel *= damping;   
        } else {
            vel *= .85f;
        }


        rb.MovePosition((Vector2)transform.position + vel);
    }

    void Dash() {
        if (framesSinceDash > dashTimer) {
            Vector2 dir = lStickDir.normalized;
            float mag = vel.magnitude;
            //Debug.Log(mag);
            dir *= dashSpeed;
            vel *= .5f;
            vel += dir*(mag*10);
            framesSinceDash = 0;
        }
    }

    void ShootPuck() {
        if (puck) {
            SoundController.me.PlaySoundAtNormalPitch(sfx_shoot, 1f, transform.position.x);
            shooting = false;
            Vector2 dir;
            if (lStickDir == Vector2.zero) {
                dir = Geo.ToVect(transform.localEulerAngles.z + 90f);
            } else {
                dir = lastStickDir;
            }
            puck.Shoot(vel.magnitude + shootSpd, dir);
            shootSpd = 0;
            hasPuck = false;
            framesSinceShot = 0;
            puck.lastPlayerTouched = this;
            puck = null;
        }
    }

    public void ControlPuck(PuckController p) {
        if (framesSinceShot > shotTimer) {
            p.playerControllingPuck = this;
            hasPuck = true;
            puck = p;
        }
    }

    public void DropPuck() {
        hasPuck = false;
        puck.controlled = false;
        puck.lastPlayerTouched = this;
        puck = null;
    }

    public bool CanShoot() {
        return framesSinceShot > shotTimer;
    }

    void MoveGoalie() {
        float y = lStickDir.y;
        float goalSpd = .1f;

        float desPos = goal.transform.position.y + goalSpd*y;
        if (desPos > Master.me.goalPos[playerId].y + .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y + .5f*Master.me.goalSize;       
        } else if (desPos < Master.me.goalPos[playerId].y - .5f*Master.me.goalSize) {
            desPos = Master.me.goalPos[playerId].y - .5f*Master.me.goalSize;       
        }
        goal.transform.position = new Vector2(goal.transform.position.x, desPos);
    }

    void OnCollisionEnter2D(Collision2D coll) {

        if (coll.gameObject.tag == "Wall") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
        }

        if (coll.gameObject.tag == "Player") {
            PlayerController p = coll.gameObject.GetComponent<PlayerController>();
            if (puck && p.vel.magnitude > 0.2f) {
                puck.DropPuck(p.vel);
                DropPuck();
            }

            //vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (p.vel.magnitude*5);
            Vector2 collDir = transform.position - coll.transform.position;
            vel = collDir * p.vel.magnitude;

        }

        if (coll.gameObject.tag == "Bumper") {
            vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * Random.Range(.9f, 1.3f));
        }

        if (coll.gameObject.tag == "Puck") {
            PuckController p = coll.gameObject.GetComponent<PuckController>();
            Vector2 dir = (Vector2)p.transform.position - coll.contacts[0].point;
            p.vel = dir;
            p.lastPlayerTouched = this;
            //Debug.Log("DIR: " + p.vel);
            //Debug.Log("SPEED: " + p.spd);
        }

        if (coll.gameObject.tag == "Flap") {
            FlapController f = coll.gameObject.GetComponent<FlapController>();
            if (!f.flapping) {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude * 0.5f);
            } else {
                vel = Geo.ReflectVect (vel.normalized, coll.contacts [0].normal) * (vel.magnitude);
                //accel*=2;
            }
        }

    }

    public void SetColor(Color c) {
        spr.color = c;
    }
}
