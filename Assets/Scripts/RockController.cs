using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Rock Controller
 * Script that controls the movement and collision of rocks
 * */
public class RockController : MonoBehaviour {
    public float lifeSpan;              //how long the rock can exist before being destoyed
    public float speed;                 //the rate at which the rock moves 
    float deathTime;                    //time the rock was created plus lifeSpan; when to destroy the rock
    public int damage = 2;              //the amount of damage the rock does to the player
    Vector3 movement;                   //the direction in which to move in x,y,z

    // Use this for initialization
    void Start () {
        //calculate deathTime on rock creation
        deathTime = Time.time + lifeSpan;   
    }
	
	// Update is called once per frame
	void Update () {
        //if rock is still alive move it 
        if (Time.time < deathTime)
        {
            transform.position += movement * speed * Time.deltaTime;
        }
        //else destroy rock
        else
        {
            Destroy(this.gameObject);
        }
    }

    //set the movement vector based on the direction the rock was fired
    void SetDirection(float d)
    {
        int dir = (int)d;
        switch (dir)
        {
            case 0:             //Down
                movement = new Vector3(0, -1, 0);
                break;
            case 1:             //Left
                movement = new Vector3(-1, 0, 0);
                break;
            case 2:             //Up
                movement = new Vector3(0, 1, 0);
                break;
            case 3:             //Right
                movement = new Vector3(1, 0, 0);
                break;
            default:
                //error
                break;

        }

    }

    //Deal with collisions with other objects
    void OnTriggerEnter2D(Collider2D collision)
    {
        //if the object we collided with is the player
        if (collision.gameObject.tag == "Player")
        {
            //apply damage to player
            collision.gameObject.SendMessage("TakeDamage", damage);
        }
        //if the rock hit any object, destory it
        Destroy(this.gameObject);
    }
}
