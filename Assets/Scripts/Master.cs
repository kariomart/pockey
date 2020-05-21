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
    public Color[] puckColors = new Color[4];
    public Vector2[] spawnPos = new Vector2[2];
    public Vector2[] goalPos = new Vector2[2];
    public float goalSize;
    public int goalsScored;
    public int[] multiplier = new int[2];
    public GameObject spawnedBumpers;
    public PuckController puck;
    public List<PuckController> pucks = new List<PuckController>();
    public Transform puckSpawn;

    public int livePoints;
    public int pointsToWin;
    public int periodNum = 1;
    public TextMeshProUGUI[] livePointsUI;
    public TextMeshProUGUI team1PointsUI;
    public TextMeshProUGUI team2PointsUI;
    public TextMeshProUGUI timerUI;
    public SpriteRenderer bg;

    public GameObject collisionParticle;
    public Screenshake shake;

    public AudioClip sfx_goal;
    public GameObject coinFX;
    public GameObject scanVFX;
    public GameObject angledBouncerVFX;

    Color defColor;

    public BouncerController[] bouncers; 

    public float gameTime;
    public float periodLength;

    public bool randomPuckMode;
    public GameObject startMenu;
    public bool gamePaused;

    public bool gameOver;
    public GameObject gameOverScreen;
    public TextMeshProUGUI winnerText;
    public Image winnerBg;
    public AudioSource musicSource;
    public AudioClip powerUp;
    public AudioClip powerDown;
    public AudioClip periodOver;
    public AudioClip countdownSFX;

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
        //livePoints = 5;
        shake = Camera.main.GetComponent<Screenshake>();
        UpdateUI();
        defColor = bg.color;
        gameTime = periodLength;
        //randomPuckMode = true;
    }
  

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            randomPuckMode = !randomPuckMode;
        }

        // if (gameTime > 60) {
        //     timerUI.text = "" + (int)(gameTime/60f) + ":" + (int)(gameTime % 60);
        // } else if ((gameTime % 60) > 10) {
        //     timerUI.text = "0:" + (int)gameTime;
        // } else {
        //     timerUI.text = "0:0" + (int)gameTime;
        // }


        
    }

    void FixedUpdate() {

        gameTime -= Time.deltaTime;
        string minutes = Mathf.Floor(gameTime / 60).ToString("0");
        string seconds = (gameTime % 60).ToString("00");
        timerUI.text = minutes + ":" + seconds;
        if (gameTime % 1 <= .017 && gameTime < 5 && gameTime > 1 && !gameOver) {
            SoundController.me.PlaySound(countdownSFX, 1f);
        }   

        if (gameTime <= 0) {
            if (periodNum == 3) {
                gameOver = true;
                //Time.timeScale = 0;
                gameOverScreen.SetActive(true);
                PlayerController p;
                if (players[0].points > players[1].points) {
                    p = players[0];
                    winnerText.text = "RED HAS WON";
                } else {
                    p = players[1];
                    winnerText.text = "BLUE HAS WON";
                }

                winnerBg.color = p.color;

            } else {
                NewPeriod();
            }
            //SoundController.me.PlaySound(sfx_goal, 1f);
            
        }


        if (Input.GetKeyDown(KeyCode.Escape)) {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            NewPeriod();
        }

        int r = Random.Range(0, 2000);
        r *= pucks.Count;
        if (r == 10 && randomPuckMode && !gamePaused) {
            SpawnRandomPuck();
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


        ResetBouncers();
        goalsScored++;

        if (p.id == 1) {
            livePoints*=2;
        }

        players[g.playerId].points+=livePoints;
        StartCoroutine(FlashBackground(playerColors[g.playerId], g));

        livePoints = 0;
        SoundController.me.PlaySound(sfx_goal, .5f);
        UpdateUI();
        SpawnPuck(g, 1-p.lastPlayerTouched.playerId);
    }

    public IEnumerator AddPoints(int points) {
        for(int i =0; i<= points; i++) {
            livePoints += 1;
            UpdateUI();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public IEnumerator AddPoints(int points, int id) {
        for(int i =0; i<= points; i++) {
            players[id].points += 1;
            UpdateUI();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void NewPeriod() {
        SoundController.me.PlaySound(periodOver, 1f);
        foreach(PlayerController p in players) {
            p.transform.position = spawnPos[p.playerId];
            p.Start();
            for(int i = pucks.Count - 1; i > -1; i--) {
                if (pucks[i]) {
                    Destroy(pucks[i].gameObject);
                }
            }

            pucks.Clear();
        }

        
        ResetBouncers();
        SpawnPuck();
        livePoints = 0;
        UpdateUI();
        gameTime = periodLength;
        periodNum++;
        
    }

     public void NewPeriod(int a) {
        foreach(PlayerController p in players) {
            p.transform.position = spawnPos[p.playerId];
            p.Start();
            for(int i = pucks.Count - 1; i > -1; i--) {
                if (pucks[i]) {
                    Destroy(pucks[i].gameObject);
                }
            }

            pucks.Clear();
        }

        
        ResetBouncers();
        SpawnPuck();
        livePoints = 0;
        UpdateUI();
        gameTime = periodLength;
        PauseGame();
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

    public void PauseGame() {

        if (!gamePaused) {
            SoundController.me.PlaySound(powerUp, 1f);
            startMenu.SetActive(true);
            Time.timeScale = 0;
            gamePaused = true;
            musicSource.Pause();
        } else {
            startMenu.SetActive(false);
            Time.timeScale = 1;
            gamePaused = false;
            SoundController.me.PlaySound(powerDown, 1f);
            musicSource.Play();
        }   


    }

   



    IEnumerator FlashBackground(Color color, GoalController g) {
        //bg.color = color;
        float time = 100f;
        SpriteRenderer s = Instantiate(scanVFX, g.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
        s.color = color;
        yield return new WaitForSeconds(0.01f);
        // for (int i=0;i<=time;i++) {
        //     Color desColor = Color.Lerp(color, defColor, i/time);
        //     bg.color = desColor;
        //     yield return new WaitForSeconds(0.01f);
        // }
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
        foreach (TextMeshProUGUI t in livePointsUI) {
            t.text = "" + livePoints;
        }
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

    public int ChoosePuckType() {
        int r = Random.Range(0, 101);

        if (r > 50) {
            return 0;
        } else if (r <= 5) {
            return 1;
        }
        // } else if (r <= 20) {
        //     return 2;
        // } 
        return 0;
    }

    public void SpawnRandomPuck() {
        if (Random.value > .5f) {
            PuckController p = Instantiate(puckObj, new Vector2(0, 57), Quaternion.identity).GetComponent<PuckController>();
            p.id = ChoosePuckType();
            Debug.Log(p.id);
            p.Start();
            p.temp = true;
            p.Shoot(Vector2.down, 5, true);
            pucks.Add(p);
        } else {
            PuckController p = Instantiate(puckObj, new Vector2(0, -37), Quaternion.identity).GetComponent<PuckController>();
            p.id = ChoosePuckType();
            Debug.Log(p.id);
            p.Start();
            p.temp = true;
            p.Shoot(Vector2.up, 5, true);
            pucks.Add(p);
        }

    }

    public void SpawnPuck(Transform t, int id) {

        PuckController p = Instantiate(puckObj, t.position, Quaternion.identity).GetComponent<PuckController>();
        p.coll.enabled = false;
        players[id].flicking = true;
        Destroy(puck.gameObject);
        puck = p;
        pucks.Add(p);

        puck.reticle.SetActive(true);
        puck.coll.enabled = false;
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;
    }



    void SpawnPuck(GoalController g, int id) {

        PuckController p = Instantiate(puckObj, g.puckSpawn.position, Quaternion.identity).GetComponent<PuckController>();
        if (g.playerId != 3) {
            players[1-g.playerId].flicking = true;
        } else {
            players[id].flicking = true;
        }

        p.Start();
        p.coll.enabled = false;
        p.justSpawned = true;
        p.reticle.SetActive(true);
        Debug.Log("added p");
        pucks.Add(p);
        puck = p;
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;
    }

    void SpawnPuck() {

        PuckController p = Instantiate(puckObj, puckSpawn.position, Quaternion.identity).GetComponent<PuckController>();
        PuckController pOld = puck;
        puck = p;
        Destroy(pOld);
        StartCoroutine(PuckInvuln());
        puck.justSpawned = true;
        pucks.Add(p);
        Camera.main.GetComponent<CameraController>().t3 = puck.transform;

    }

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, spawnPos[i], Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i%2;
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

        if (coll) {
            coll.enabled = true;
            puck.justSpawned = false;
        }

    }

    public float GetLargestDist(Vector2 pos) {
        float d = 0;

        foreach(PuckController p in pucks) {
            if (p) {
                if (!p.controlled) {
                    float dis = Vector2.Distance(pos, p.transform.position);
                    if (dis > d) {
                        d = dis;
                    }
                }
            } 
        }

        for(int i = pucks.Count - 1; i > -1; i--) {
            if (pucks[i] == null) {
                pucks.RemoveAt(i);
            }
        }

        return d;
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
