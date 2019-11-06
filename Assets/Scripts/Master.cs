using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Master : MonoBehaviour
{

    public static Master me;
    public GameObject playerPrefab;
    public GameObject puckObj;
    public FlapController leftFlap;
    public FlapController rightFlap;

    public int numPlayers;
    public PlayerController[] players;
    public Color[] playerColors = new Color[4];
    public PuckController puck;
    public Transform puckSpawn;

    public PlayerController playerLastTouched;
    public int livePoints;
    public int pointsToWin;
    public TextMeshProUGUI livePointsUI;
    public Image middleCircle;

    public GameObject collisionParticle;

    public AudioClip sfx_goal;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        players = new PlayerController[numPlayers];
        SpawnPlayers();
        puck = Instantiate(puckObj, Vector2.zero, Quaternion.identity).GetComponent<PuckController>();
        ResetPuck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {


    }


    public void GoalScored() {

        playerLastTouched.points += livePoints;
        SoundController.me.PlaySound(sfx_goal, 1f);
        livePoints = 0;
        UpdateUI();
        ResetPuck();

    }

    public void ResetPuck() {

        puck.transform.position = puckSpawn.position;
        puck.spd = 0;

    }

    public void SpawnParticle(GameObject p, Vector2 pos) {
        Instantiate(p, pos, Quaternion.identity);
    }

    public void UpdateUI() {
        livePointsUI.text = "" + livePoints;

        // PlayerController p = GetWinningPlayer();
        // if (p) {
        //     middleCircle.color = p.spr.color;
        // }
        
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

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, new Vector2(-5 + i*1.5f, 0), Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i;
            players[i] = playerController;
        }

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
