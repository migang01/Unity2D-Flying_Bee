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

        // 시작할 때 벌이 안만들어지는걸? 흠...
        // 해결.


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
                // 이 함수를 실행시키면 유니티가 멈춤.. 에러도 아니고 그냥 벌들이 멈춤.
                //  게임매니저에 time.timescale 때문이었음... 뭔상관인진 모르겠다.
                // 해결. 게임매니저에 게임오버 함수 실행 안되는 이유와 연관된듯.
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
        // 왜 실행을 안 하지?...
        // hive에 rigidbody2d를 추가해서 되긴 했는데 벌이 나올때 생성돼서 문제임. 흠...
        // 엥 다시 하니까 안되노 시발...
        // collider 게임오브젝트도 리지드바디 있었구만;; 시발 뭐야? 
        // 일단 이렇게 고쳤다. 꽉 찬 벌집에 벌이 들어가면 새 벌집에 벌이 나오긴 하는데...휴..

        if (other.CompareTag("Colliding") && other.GetComponent<Death>().owner.GetComponent<Bee>().carryingHoney == true)
        {
            isCollided = true;
        }

    }
}




