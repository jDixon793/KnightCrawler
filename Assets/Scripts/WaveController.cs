using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Wave Controller
 * Script that controls the charging, movement, and collision of magic waves
 * */
public class WaveController : MonoBehaviour {
    public Animator anim;                   //Animator controller for the magic wave 
    public float lifeSpan;                  //how long the rock can exist before being destoyed
    public float speed;                     //the rate at which the rock moves 
    public float damangePerLevel = 2;       //how much damage each level of charge counts as (max 4 charge count)
    bool moving = false;                    //if the wave is moving
    float deathTime;                        //time the rock was created plus lifeSpan; when to destroy the rock
    float direction;                        //the direction the wave is facing (for animation)
    Vector3 movement;                       //the direction in which to move in x,y,z
    [SerializeField]
    float powerLevel;                       //the current charge level of the wave
    float upgradeLevel;                 //the upgrade level of the wave

    // Use this for initialization
    void Start () {
        //get the animator component
        anim = GetComponent<Animator>();
        //set deathTime so that it cannot time out before being shot
        deathTime = Time.time ;
        //initial powerlevel is 0
        powerLevel = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //if the wave is ment to be moving
        if (moving)
        {
            //if wave is still alive move it 
            if (Time.time < deathTime)
            {
                transform.position += movement * speed * Time.deltaTime;
            }
            //else destroy wave
            else
            {
                Destroy(this.gameObject);

            }
        }
    }

    //transition from a charging wave to a moving wave
    void Shoot()
    {
        //set moving to true and set a time to be destoryed
        moving = true;
        deathTime = Time.time + lifeSpan;
    }

    void SetUpgrade(float level)
    {
        upgradeLevel = level;
        anim.SetFloat("waveUpgrade", upgradeLevel);
    }
    //Set the direction and movement direction
    void SetDirection(float d)
    {
        direction = d;
        anim.SetFloat("direction", direction);
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
                break;

        }
    }

    //updates the powerlevel of the wave while charging
    void UpdatePower(float p)
    {
        powerLevel = p;
        anim.SetFloat("power",powerLevel);
    }

    //handles collition with other objects
    void OnTriggerEnter2D(Collider2D collision)
    {
        //if the object we collided with is the emeny
        if (collision.gameObject.tag == "Enemy")
        {
            //apply damage to that enemy for the current powerLevel
            collision.gameObject.SendMessage("TakeDamage", damangePerLevel * powerLevel * (upgradeLevel + 1));
        }
        //destory the wave if it collides with and object
        Destroy(this.gameObject);
    }
}
