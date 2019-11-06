using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Master : MonoBehaviour
{

    public static Master me;
    public GameObject playerPrefab;
    public GameObject puckObj;

    public int numPlayers;
    public PlayerController[] players;
    public Color[] playerColors = new Color[4];
    public PuckController puck;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        players = new PlayerController[numPlayers];
        SpawnPlayers();
        puck = Instantiate(puckObj, Vector2.zero, Quaternion.identity).GetComponent<PuckController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {


    }


    public void GoalScored() {

        ResetPuck();

    }

    public void ResetPuck() {

        puck.transform.position = new Vector2(0, 0);
        puck.spd = 0;

    }

    void SpawnPlayers() {

        for (int i = 0; i < players.Length; i++)
        {
            GameObject playerObj = Instantiate(playerPrefab, new Vector2(-5 + i*1.5f, 0), Quaternion.identity);
            PlayerController playerController = playerObj.GetComponent<PlayerController>();
            playerController.playerId = i;
        }

    }

    void Initialize() {
        if (me == null) {
		    me = this;	
		} else {
			Destroy (this.gameObject);
		}
    }
}
