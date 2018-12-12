using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Goblin Controller
 * Controlls the AI of the Goblin enemy
 * Moves just like the octo enemy
 * Shoots at the player if they are in LOS (line of sight) and aim range
 * 
 * 
 * */
public class GoblinController : MonoBehaviour
{

    public int health = 10;                 //Health
    public float speed = 3;                   //Speed
    public float maxRange;                  //The farthest the octo can move away from the move origin
    public float minRange;                  //the shortest distance the octo can move  awway from the move origin
    public float shootProb = .1f;           //the probability that you will take a shoot action (out of 0.0 - 1.0)
    public GameObject arrow;                 //the arrow that the octo shoots
    public int arrowUpgradeLevel=0;
    public int arrowBaseDamage=2;
    public float shootTime;
    public float shootStartTime;
    public float waitTime = .5f;            //the amount of time to pause after taking an action
    public float waitStartTime = 0;         //the time the octo started waiting
    public float aimRange = 4.0f;           
    [SerializeField]
    int state;                              //the current state the AI is in
    int direction;                          //the direction the octo is facing
    Vector3 movement;                       //the vector direction the octo will move in
    Vector3 origin;                         //the start movition of the movement state
    [SerializeField]
    float distance;                         //the distance to move during movement state
    public Animator anim;                   //the octo animator
    Vector3 rayOrigin, rayOrigin2;          //where to start the rays for movement

    public GameObject[] lootTable;
    public int[] lootTableDropChance;

    // Use this for initialization
    void Start()
    {
        state = 0;
        movement = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {

        //if health reaches 0, die
        if (health <= 0)
        {
            //death
            DropLoot();
            Destroy(this.gameObject);
        }


        //Control the AI of the octo


        switch (state)
        {
            //State 0 - Seek State
            //finds a destination to head to and a distance to travel
            //shots two rays from each edge of the octo
            //if the found direction and distance is valid then move to the movement state
            case 0:             //Find a direction and distance to go
                direction = (int)(Random.value * 4);
                switch (direction)
                {
                    case 0:
                        movement = Vector3.down;
                        rayOrigin = (movement * .7f) + new Vector3(.25f, 0, 0);
                        rayOrigin2 = (movement * .7f) + new Vector3(-.25f, 0, 0);
                        break;
                    case 1:
                        movement = Vector3.left;
                        rayOrigin = (movement * .7f) + new Vector3(0, .25f, 0);
                        rayOrigin2 = (movement * .7f) + new Vector3(0, -.25f, 0);
                        break;
                    case 2:
                        movement = Vector3.up;
                        rayOrigin = (movement * .7f) + new Vector3(.25f, 0, 0);
                        rayOrigin2 = (movement * .7f) + new Vector3(-.25f, 0, 0);
                        break;
                    case 3:
                        movement = Vector3.right;
                        rayOrigin = (movement * .7f) + new Vector3(0, .25f, 0);
                        rayOrigin2 = (movement * .7f) + new Vector3(0, -.25f, 0);
                        break;
                }

                RaycastHit2D hitt = Physics2D.Raycast(transform.position + rayOrigin, movement);
                RaycastHit2D hitt2 = Physics2D.Raycast(transform.position + rayOrigin2, movement);

                //make sure the distance is not less than min range
                if (Mathf.Min(hitt.distance, hitt2.distance) > minRange)
                {
                    //calculate a distance to move; gets a random value between minRange and the smallest of the two raycasts hits or maxRange
                    distance = Random.Range(minRange, Mathf.Min(hitt.distance, hitt2.distance, maxRange));
                    origin = transform.position;
                    anim.SetFloat("direction", direction);
                    anim.SetBool("moving", true);
                    state = 1;
                }


                break;
            //State 1 - Movement State
            //moves in the found direction for the found distance
            //if we have moved the desired distance or an object moves into our path start the might shoot phase
            case 1:

                RaycastHit2D hit = Physics2D.Raycast(transform.position + rayOrigin, movement);
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position + rayOrigin2, movement);
                Debug.DrawLine(transform.position + rayOrigin, hit.point);
                Debug.DrawLine(transform.position + rayOrigin2, hit2.point);
                AimShoot();
                //if it gets too close to another collider or it has reached the destination
                if (Mathf.Min(hit.distance, hit2.distance) < .2f || Vector3.Distance(origin, transform.position) > distance)
                {
                    state = 2;
                }
                //if our rays hit the player then shoot that direction and move to the wait phase
                if (hit.collider != null && hit2.collider != null && (hit.collider.gameObject.tag == "Player" || hit2.collider.gameObject.tag == "Player"))
                {
                    Shoot();
                    waitStartTime = Time.time;
                    state = 3;
                }
                //continue moving
                else
                {
                    transform.position += movement * speed * Time.deltaTime;
                }
                break;

            //State 2 - Shoot State
            //gives the Octo the opporunity to shoot based on the shootProb var
            //if we should shoot then pick a random direction and shoot
            //move to wait state
            case 2:

                if (Random.value < shootProb)              // shoot probability 
                {
                    direction = (int)(Random.value * 4);
                    Shoot();
                }
                waitStartTime = Time.time;
                state = 3;

                break;
            //State 3 - Wait State
            //wait for wait time then move to seek phase
            case 3:
                anim.SetBool("moving", false);
                if (Time.time > waitStartTime + waitTime)
                {
                    state = 0;
                }
                break;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    void Shoot()
    {
        anim.SetFloat("direction", direction);
        GameObject arrowClone = Instantiate(arrow, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(arrowClone.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        arrowClone.SendMessage("SetDirection", direction);
        arrowClone.SendMessage("SetUpgrade", arrowUpgradeLevel);
        arrowClone.SendMessage("SetBaseDamage", arrowBaseDamage);
    }

    void AimShoot()
    {
        if (Time.time > shootStartTime + shootTime)
        {
            int lookDir = 0;
            Vector3 look = new Vector3();
            Vector3 orgin = new Vector3();
            while (lookDir <= 3)
            {
                switch (lookDir)
                {
                    case 0:
                        look = Vector3.down;
                        orgin = (look * .5f);
                        break;
                    case 1:
                        look = Vector3.left;
                        orgin = (look * .5f);
                        break;
                    case 2:
                        look = Vector3.up;
                        orgin = (look * .5f);
                        break;
                    case 3:
                        look = Vector3.right;
                        orgin = (look * .5f);
                        break;
                }
                RaycastHit2D hittt = Physics2D.Raycast(transform.position + orgin, look, aimRange);
                

                //if our rays hit the player then shoot that direction and move to the wait phase
                if (hittt.collider != null && hittt.collider.gameObject.tag == "Player")
                {
                    Debug.DrawLine(transform.position + rayOrigin, hittt.point);
                    direction = lookDir;
                    Shoot();
                    shootStartTime = Time.time;
                    waitStartTime = Time.time;
                    state = 3;
                    break;
                }
                lookDir++;

            }
        }

    }

    void DropLoot()
    {
        int totalDropChance = 0;
        foreach (int v in lootTableDropChance)
        {
            totalDropChance += v;
        }
        int drop = (int)Random.Range(0f, (float)totalDropChance);
        int curDropChance = 0;
        int indexOfLoot = -1;
        for (int i = 0; i < lootTable.Length; i++)
        {
            curDropChance += lootTableDropChance[i];
            if (drop < curDropChance)
            {
                indexOfLoot = i;
                break;
            }
        }

        Instantiate(lootTable[indexOfLoot], transform.position, Quaternion.identity);
    }
}
