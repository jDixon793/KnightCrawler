using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

/*Player Controller
 * Script that controls the player and there many actions
 * */
public class PlayerController : MonoBehaviour {

    public int speed = 10;                      //Set the rate at which the player can move
    public GameObject arrow;                    //Prefab for shooting arrows
    public GameObject wave;                     //Prefab for shooting magic waves
    GameObject waveClone;                       //the clone of the wave that is in the game
    GameObject arrowClone;                      //the clone of the arrow that is in the game
    public float attackDelay = .1f;             //the amount of time to delay movment after attacking
    public float jewels = 0;                    //number of jewels currently in the players inventory
    public int direction = 0;                   //the direction the player is facing for animations ( Down -0 Left -1 Up -2 Right -3)
    float nextAttack;                           //the time when you can attack again (time attack started + attackDelay)
    float castStartTime;                        //when you starting pressing the cast button down
    bool moving = false;                        //if the player is moving
    bool attacking = false;                     //if the player is attacking
    bool casting = false;                       //if the player is casting
    bool slashing = false;
    Animator anim;                              //the animator controller for the player
    Vector3 movement;                           //the direction the player needs to move
    public float swordDamage = 5;
    public GameObject downSwing;
    public GameObject leftSwing;
    public GameObject rightSwing;
    public GameObject upSwing;
    public GameObject lifeGage;
    public GameObject heart;
    public GameObject[] hearts;
    int lastIndex;
    float maxCastTime;          // max lenfth of cast
    [SerializeField]
    int health = 20;                           //the amount of health the player has remaining
    public int swordUpgradeLevel, waveUpgradeLevel, arrowUpgradeLevel;


    //GUI Elements
    public Text jewelText;

	// Use this for initialization
	void Start () {
        //set nextAttack so you can attack
        nextAttack = Time.time;
        //get the aminator component
        anim = GetComponent<Animator>();
        maxCastTime = (attackDelay * 6.0f);

        hearts = new GameObject[20];
        for(int i=1;i*4<health;i++)
        {
            hearts[i-1] = Instantiate(heart, lifeGage.transform);
            hearts[i-1].transform.localScale = Vector3.one;
        }
        if(health % 4 != 0)
        {
            hearts[health/4 -1].GetComponent<Image>().fillAmount = .25f * (health % 4);
        }
        lastIndex = health / 4 - 1;
        //UpdateLifeGage();
    }
	
	// Update is called once per frame
	void Update () {
        //Get the input assigned to diferent actions
        float moveHor = Input.GetAxisRaw("Horizontal");
        float moveVer = Input.GetAxisRaw("Vertical");
        bool slash = Input.GetButtonDown("Fire3");
        bool cast = Input.GetButtonDown("Fire2");
        bool shoot = Input.GetButtonDown("Fire1");
        //bool punch = Input.GetButtonDown("Fire4");

        //reset the movement direction
        movement = new Vector3(0, 0, 0);

        //code that handles sword attacks
        if(slash && !attacking)
        {
            anim.SetFloat("swordUpgrade",swordUpgradeLevel);
            nextAttack = Time.time + attackDelay;
            attacking = true;
            slashing = true;
            anim.SetBool("slashing",true);
        }
        //code that can handle punching
       /* else if (punch && !attacking)
        {
            nextAttack = Time.time + attackDelay;
            attacking = true;
            anim.SetBool("punching", true);
        }*/
        //shoots an arrow in the direction the player is facing
        else if (shoot && !attacking)
        {
            nextAttack = Time.time + attackDelay;
            attacking = true;
            anim.SetBool("shooting", true);
            arrowClone = Instantiate(arrow, transform.position,Quaternion.identity);
            Physics2D.IgnoreCollision(arrowClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
            arrowClone.SendMessage("SetDirection",direction);
            arrowClone.SendMessage("SetUpgrade", (float)arrowUpgradeLevel);

        }
       //handles charging the magic wave
        else if (cast && !attacking)    //Start channeling the cast
        {
            attacking = true;
            casting = true;
            castStartTime = Time.time;
            maxCastTime = (attackDelay * (6.0f - waveUpgradeLevel));
            nextAttack = Time.time + (maxCastTime); //stop casting at the max cast time

            Vector3 waveOffset = new Vector3();
            switch((int)direction)
            {
                case 0:             //Down
                    waveOffset = new Vector3(.05f, -1, 0);
                    break;
                case 1:             //Left
                    waveOffset = new Vector3(-1, 0, 0);
                    break;
                case 2:             //Up
                    waveOffset = new Vector3(0, 1, 0);
                    break;
                case 3:             //Right
                    waveOffset = new Vector3(1, 0, 0);
                    break;
                default:
                    break;
            }
            Collider2D wandTest = Physics2D.OverlapCircle(transform.position + waveOffset, .2f);
            if (wandTest == null )
            {
                waveClone = Instantiate(wave, transform.position + waveOffset, Quaternion.identity);
                waveClone.SendMessage("SetDirection", direction);
                waveClone.SendMessage("SetUpgrade", (float)waveUpgradeLevel);
                anim.SetBool("casting", true);
            }
            
        }

        if(casting && (Input.GetButtonUp("Fire2")|| Time.time > nextAttack))   //if we have cast and released
        {
            nextAttack = Time.time;
            waveClone.SendMessage("Shoot");
            casting = false;
        }
        else if(casting)                            //if we are still casting
        {
            //if the wave got destroyed while we are channeling then cancel casting
            if (waveClone == null)
            {
                casting = false;
                anim.SetBool("casting", false);
                attacking = false;
            }
            else
            {
                
                float curCastLength = Time.time - castStartTime; // current length of cast;
                float timePerLever = maxCastTime / 5.0f;
                float power = curCastLength / timePerLever;
                waveClone.SendMessage("UpdatePower", power);
            }
        }

        if (attacking && Time.time > nextAttack)
        {
            attacking = false;
            slashing = false;
            downSwing.SetActive(false);
            leftSwing.SetActive(false);
            upSwing.SetActive(false);
            rightSwing.SetActive(false);
            anim.SetBool("slashing", false);
            anim.SetBool("punching", false);
            anim.SetBool("shooting", false);
            anim.SetBool("casting", false);
        }

        if(slashing)
        {
            switch(direction)
            {
                case 0:
                    downSwing.SetActive(true);
                    break;
                case 1:
                    leftSwing.SetActive(true);
                    break;
                case 2:
                    upSwing.SetActive(true);
                    break;
                case 3:
                    rightSwing.SetActive(true);
                    break;
            }
        }


        //movement code
        //Set up for snap movement the input will always be -1 0 or 1
        //change the movement vector based on movement.
        //only change directing if we are not in the middle of casting
        if (!casting)
        {
            if (moveVer < 0)                        //down
            {
                direction = 0;
                moving = true;
                movement = new Vector3(0, moveVer, 0);
            }
            else if (moveHor < 0)                   //left
            {
                direction = 1;
                moving = true;
                movement = new Vector3(moveHor, 0, 0);
            }
            else if (moveVer > 0)                   //up
            {
                direction = 2;
                moving = true;
                movement = new Vector3(0, moveVer, 0);
            }
            else if (moveHor > 0)                   //right
            {
                direction = 3;
                moving = true;
                movement = new Vector3(moveHor, 0, 0);
            }
        }
        //if move Hor and Ver are both 0 then we are not moving
        if (moveHor == 0 && moveVer == 0)
        {
            moving = false;
        }

        //move the character
        //movement is no allowed while attacking.
        if (moving && !attacking)
        {
            transform.position += movement * speed * Time.deltaTime;
        }
        
        anim.SetFloat("direction",direction);
        anim.SetBool("moving", moving);
    }

    void UpdateLifeGage()
    {
        if(hearts[lastIndex]!=null)
        {
            hearts[lastIndex].GetComponent<Image>().fillAmount = 1f;
        }
        for (int i = 1; i * 4 < health; i++)
        {
            if (hearts[i - 1] == null)
            {
                hearts[i - 1] = Instantiate(heart, lifeGage.transform);
                hearts[i - 1].transform.localScale = Vector3.one;
            }
        }
        if(lastIndex > health / 4 - 1)
        {
            for (int i = health / 4; i<20; i++)
            {
                if (hearts[i] != null)
                {
                    GameObject.Destroy(hearts[i]);
                }
            }
        }
        if (health % 4 != 0)
        {
            hearts[health / 4 - 1].GetComponent<Image>().fillAmount = .25f * (health % 4);
        }
        lastIndex = health / 4 - 1;
    }
    void TakeDamage(int amount)
    {
        health -= amount;
        if (health > 0)
        {
            UpdateLifeGage();
        }
    }

    void AddJewels(int amount)
    {
        jewels += amount;
        jewelText.text = "X " + jewels;
    }

    void UpgradeSword()
    {
        swordUpgradeLevel += swordUpgradeLevel < 2 ? 1:0;
    }
    void UpgradeWave()
    {
        waveUpgradeLevel += waveUpgradeLevel < 2 ? 1:0;
    }
    void UpgradeArrows()
    {
        arrowUpgradeLevel += arrowUpgradeLevel < 2 ? 1 :0;
    }

    //handles collision
    void OnCollisionEnter2D(Collision2D collision)
    {
        //When the player collides with the jewel
        if (collision.gameObject.tag == "Enemy")
        {
            //add the value of the jewel to the players total and destory
            TakeDamage(1);
            Vector2 bounce = new Vector2(movement.x * -1f, movement.y * -1f);
            this.GetComponent<Rigidbody2D>().AddForce(bounce/Time.deltaTime,ForceMode2D.Impulse);
        }

    }
}
