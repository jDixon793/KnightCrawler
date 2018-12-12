using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour {

    public PlayerController playerController;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //handles collision
    void OnTriggerEnter2D(Collider2D collision)
    {
        //When the player collides with the jewel
        if (collision.gameObject.tag == "Enemy")
        {
            //add the value of the jewel to the players total and destory
            collision.gameObject.SendMessage("TakeDamage", playerController.swordDamage);
        }

    }
}
