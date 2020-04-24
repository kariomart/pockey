using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Master : MonoBehaviour
{

    public static Master me;
    public GameObject playerPrefab;
    public GameObject goalieObj;
    public GameObject puckObj;
    public bool goaliesEnabled;
    public bool collided;

    public int numPlayers;
    public PlayerController[] players;
    public GoalController[] goals;
    public Color[] playerColors = new Color[4];
    public Vector2[] spawnPos = new Vector2[2];
    public Vector2[] goalPos = new Vector2[2];
    public float goalSize;
    public int goalsScored;
    public int[] multiplier = new int[2];
    public GameObject spawnedBumpers;
    public PuckController puck;
    public Transform puckSpawn;

    public int livePoints;
    public int pointsToWin;
    public TextMeshProUGUI livePointsUI;
    public TextMeshProUGUI team1PointsUI;
    public TextMeshProUGUI team2PointsUI;
    public TextMeshProUGUI timerUI;
    public SpriteRenderer bg;

    public GameObject collisionParticle;
    public Screenshake shake;

    public AudioClip sfx_goal;
    public GameObject coinFX;

    Color defColor;

    public BouncerController[] bouncers; 

    public float gameTime;
    public float periodLength;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        players = new PlayerController[numPlayers];
        GameObject[] goalObjs = GameObject.FindGameObjectsWithTag("Goal");
        goals = new GoalController[goalObjs.Length];
        for (int i = 0; i < goalObjs.Length; i++)
        {
            goals[i] = goalObjs[i].GetComponent<GoalController>();
        }
        SpawnPlayers();
        SpawnPuck();
        livePoints = 5;
        shake = Camera.main.GetComponent<Screenshake>();
        UpdateUI();
        defColor = bg.color;
        gameTime = periodLength;
    }
  

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }

        gameTime -= Time.deltaTime;
        string minutes = Mathf.Floor(gameTime / 60).ToString("0");
        string seconds = (gameTime % 60).ToString("00");
        timerUI.text = minutes + ":" + seconds;
        // if (gameTime > 60) {
        //     timerUI.text = "" + (int)(gameTime/60f) + ":" + (int)(gameTime % 60);
        // } else if ((gameTime % 60) > 10) {
        //     timerUI.text = "0:" + (int)gameTime;
        // } else {
        //     timerUI.text = "0:0" + (int)gameTime;
        // }

        if (gameTime <= 0) {
            SoundController.me.PlaySound(sfx_goal, 1f);
            Time.timeScale = 0;
        }
        
    }

    void FixedUpdate() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }


    }


    public void GoalScored(PuckController p, GoalController g) {

        GameObject[] bumpers;
        bumpers = GameObject.FindGameObjectsWithTag("Bumper");
        foreach(GameObject b in bumpers) {
            BumperController bump = b.GetComponent<BumperController>();
            if (bump) {
                bump.Reset();
            }
        }

        if (goalsScored == 0) {
            spawnedBumpers.SetActive(true);
        }

        Debug.Log("goal scored");
        ResetBouncers();
        goalsScored++;
        if (g.playerId == 3) {
            p.lastPlayerTouched.points += livePoints;
        } else if (p.lastPlayerTouched.playerId == g.playerId) {
            players[g.playerId].points += livePoints*multiplier[g.playerId];
        } else {
            players[1].points += livePoints/2;
            players[0].points += livePoints/2;
        }
        StartCoroutine(FlashBackground(playerColors[g.playerId]));

        livePoints = 0;
        SoundController.me.PlaySound(sfx_goal, .5f);
        UpdateUI();
        SpawnPuck(g, 1-p.lastPlayerTouched.playerId);
    }

    public void AddPoints(int points) {
        livePoints += points;
        UpdateUI();
    }
    
    public void AddPoints(int points, Vector2 pos) {
        livePoints += points;
        UpdateUI();
        //StartCoroutine(ShootCoinFX(points, pos));
    }

    public void ChangeGoal(int id) {
        goals[0].playerId = id;
        goals[0].ChangeColor(playerColors[id]);
    }


    public void CheckBouncers() {
        int t1Controlled = 0;
        int t2Controlled = 0;

        foreach (BouncerController b in bouncers) {
            if (b.controlledID == 0) {
                t1Controlled++;
            }

            else if (b.controlledID == 1) {
                t2Controlled++;
            }

            if (t1Controlled > 2) {
                // goals[1].DisableBlocker();
                // goals[0].EnableBlocker();
            }else if (t2Controlled > 2) {
                // goals[0].DisableBlocker();
                // goals[1].EnableBlocker();
            }
        }

        multiplier[0] = t1Controlled;
        multiplier[1] = t2Controlled;
    }

    public void ResetBouncers() {
        foreach (BouncerController b in bouncers) {
            b.Reset();
        }
    }

   

    IEnumerator ShootCoinFX(int amt, Vector2 pos) {

        for (int i = 0; i < amt; i++)
        {
            CoinFX coin = Instantiate(coinFX, pos, Quaternion.identity).GetComponent<CoinFX>();   
            coin.destination = Vector2.zero;
            yield return new WaitForSeconds(.1f);
        }
    }

    IEnumerator FlashBackground(Color color) {
        bg.color = color;
        float time = 100f;

        for (int i=0;i<=time;i++) {
            Color desColor = Color.Lerp(color, defColor, i/time);
            bg.color = desColor;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator HitPos(float t = 0.01f) {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(t);
        Time.timeScale = 1;
    }



    public void SpawnParticle(GameObject p, Vector2 pos) {
        Instantiate(p, pos, Quaternion.identity);
    }

    public void SpawnParticle(GameObject p, Vector2 pos, Color c) {
        var main = p.gameObject.GetComponent<ParticleSystem>().main;
        main.startColor = c;
        Instantiate(p, pos, Quaternion.identity);
    }

    public void UpdateUI() {
        livePointsUI.text = "" + livePoints;
        team1PointsUI.text = "" + players[0].points;
        team2PointsUI.text = "" + players[1].points;
    }


    PlayerController GetWinningPlayer() {
        PlayerController ply = null;
        int s = 0;
        for (int i = 0; i < numPlayers; i++)
        {
            PlayerController p = players[i];
            if (p.points > s) {
                ply = p;
                s = p.points;
            }
        }

        return ply;
    }

    public void SpawnPuck(Transform t, int id) {

        PuckController p = Instantiate(puckObj, t.position, Quaternion.identity).GetComponent<PuckController>();
        p.coll.enabled = false;
        players[id].flicking = true;
        Destroy(puck.gameObject);
        puck = p;

        puck.reticle.SetActive(true);
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;
    }



    void SpawnPuck(GoalController g, int id) {

        puck = Instantiate(puckObj, g.puckSpawn.position, Quaternion.identity).GetComponent<PuckController>();
        if (g.playerId != 3) {
            players[1-g.playerId].flicking = true;
        } else {
            players[id].flicking = true;
        }
        puck.reticle.SetActive(true);
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;
    }

    void SpawnPuck() {

        puck = Instantiate(puckObj, puckSpawn.position, Quaternion.identity).GetComponent<PuckController>();
        StartCoroutine(PuckInvuln());
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;

    }

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPos[i], Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i;
            players[i] = playerController;

            if (goaliesEnabled)
            {
                GoalieController g = Instantiate(goalieObj, goalPos[i], Quaternion.identity).GetComponent<GoalieController>();
                g.gameObject.GetComponent<SpriteRenderer>().color = playerColors[i];
                g.transform.eulerAngles = new Vector3(0, 0, -90f);
                playerController.goal = g;
            }

        }

    }

    IEnumerator PuckInvuln() {

        float maxScale = 5;
        float defScale = puck.transform.localScale.x;
        Collider2D coll = puck.GetComponent<Collider2D>();
        coll.enabled = false;
        puck.transform.localScale = new Vector3(maxScale, maxScale, maxScale);
        float len = 50f;
        for (int i = 0; i <= len; i++)
        {
            float desScale = Mathf.Lerp(maxScale, defScale, i/len);
            puck.transform.localScale = new Vector3(desScale, desScale, desScale);
            yield return new WaitForSeconds(.0001f);        
        }

        Debug.Log("!");
        coll.enabled = true;

    }

    

    void Initialize() {
        if (me == null) {
		    me = this;	
		} else {
			Destroy (this.gameObject);
		}

       // GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
