using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hive : MonoBehaviour
{
    public int residents = 0;
    public GameObject bee;
    // public static int beeNumber = 1;
    GameManager gameManager;
    private Animator anim;
    private bool HiveFull;
    public static bool isStarted;
    private bool isCollided;

    void Start()         // need one bee to start game
    {

        isStarted = false;
        HiveFull = false;
        isCollided = false;
        anim = GetComponent<Animator>();


    }

    void makingBee()
    {
        GameObject newBee = Instantiate(bee);
        newBee.transform.position = this.gameObject.transform.position;
        BeeNumber.bee += 1;
        residents += 1;

    }

    void Update()
    {
        if (isStarted == true)
        {
            makingBee();
        }
        isStarted = false;

        if (residents == 5) // if this hive gets 5 bees
        {
            HiveNumber.newHive += 1;  // you can build one hive more
            Audio.HiveSoundPlay();
            anim.SetTrigger("HiveFull");
            HiveFull = true;
            residents = 0;
        }

        if (HoneyNumber.honey - 1 == HoneyNumber.lastHNumber && isCollided == true)
        {
            if (HiveFull == false)
            {
                anim.SetTrigger("Plus");
                makingBee();
            }

            else
            {
                anim.SetTrigger("FullPlus");
                makingBee();
            }
        }
        HoneyNumber.lastHNumber = HoneyNumber.honey;
    }





    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Colliding") && other.GetComponent<Death>().owner.GetComponent<Bee>().carryingHoney == true)
        {
            isCollided = true;
        }

    }
}




