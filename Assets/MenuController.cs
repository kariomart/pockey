using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class MenuController : MonoBehaviour
{
    public Player p1;
	public Player p2;
    public Button startButton;
    public GameObject selection;

    // Start is called before the first frame update
    void Start()
    {
        p1 = ReInput.players.GetPlayer(0);
		p2 = ReInput.players.GetPlayer(1);
        startButton.Select();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
