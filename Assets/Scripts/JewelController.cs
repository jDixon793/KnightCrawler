using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Jewel Controller
 * Script that controls jewels value and collision
 * */
public class JewelController : MonoBehaviour {

    public int value;               //value set in inspector
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
        if (collision.gameObject.tag == "Player")
        {
            //add the value of the jewel to the players total and destory
            collision.gameObject.SendMessage("AddJewels", value);
            Destroy(this.gameObject);
        }
        
    }
}
