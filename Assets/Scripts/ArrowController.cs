using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Jewel Controller
 * Script that controls arrow behavior
 * */
public class ArrowController : MonoBehaviour {
    public Animator anim;                   //the animator controller for the arrow
    public SpriteRenderer sprite;           //the sprite render for the arrow   
    public float lifeSpan;                  //how long the arrow can exist before being destoyed
    public float speed;                     //the rate at which the arrow moves 
    float direction;                        //the direction the arrow is facing (for animation)
    float deathTime;                        //time the arrow was created plus lifeSpan; when to destroy the arrow
    Vector3 movement;                       //the direction in which to move in x,y,z
    float upgradeLevel;
    int damage=2;
    public string targetTag="";

    // Use this for initialization
    void Start () {
        //assign the components on start
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        //set deathTIme
        deathTime = Time.time + lifeSpan;
        
    }

    // Update is called once per frame
    void Update () {
        //if arrow is still alive move it 
        if(Time.time < deathTime)
        {
            transform.position += movement * speed * Time.deltaTime;
        }
        //else destroy arrow
        else
        {
            Destroy(this.gameObject);
        }
    }

    void SetDirection(float d)
    {
        //set the direction so the arrow faces the correct direction
        direction = d;
        anim.SetFloat("direction", direction);
        int dir = (int)d;
        switch(dir)
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
                //Place the spirte on top of the player so it looks like it is being shot
                //All other directings draw behind so that the shot is "hidden"
                sprite.sortingLayerName = "Projectile_Front"; //default is Projectile_Behind
                movement = new Vector3(1, 0, 0);
                break;
            default:
                //error
                break;

        }

    }

    void SetUpgrade(float level)
    {
        upgradeLevel = level;
        anim.SetFloat("arrowUpgrade", upgradeLevel);
    }

    void SetBaseDamage(int d)
    {
        damage = d;
    }

    //Deal with collisions with other objects
    void OnTriggerEnter2D(Collider2D collision)
    {
        //if the object we collided with is the emeny
        if (collision.gameObject.tag == targetTag)
        {
            //apply damage to player
            collision.gameObject.SendMessage("TakeDamage", damage * (upgradeLevel + 1));
        }
        //if the arrow hit any object, destory it
        Destroy(this.gameObject);
    }
}
