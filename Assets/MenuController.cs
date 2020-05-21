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
    public Button selectedItem;
    public AudioClip selectionChangedSound;
    public AudioClip selectedSound;
    public AudioSource audioSource;
    

    // Start is called before the first frame update
    void Start()
    {
        p1 = ReInput.players.GetPlayer(0);
		p2 = ReInput.players.GetPlayer(1);
        audioSource = GetComponent<AudioSource>();
        startButton.Select();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectionChangeSound() {
        audioSource.PlayOneShot(selectionChangedSound);
    }

    public void selectionPressed() {
        audioSource.PlayOneShot(selectedSound);
    }


}
