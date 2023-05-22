using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
   GameManager gameManager;
    public GameObject owner;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Flower"))
        {
            owner.GetComponent<Bee>().collidingFlower = true;
        }

        if (other.CompareTag("Hive"))
        {
            owner.GetComponent<Bee>().collidingHive = true;
        }

        // 작동 안함.
        // 같은 게임오브젝트 안에서 여러 개의 collider 넣어봤자여서...
        // 어떡하지?
        // 아니 collide는 일어나는데 안에 게임매니저에 fuction 불러와도 반응이 없음.
        // time.timescale 넣어서 멈추게 하면 작동은 함. 근데 다른 건 작동을 안해?
        // 해결. 게임매니저 gameover부분 참고.
        if (other.CompareTag("Colliding"))
        {
            
            // game over when both are not resting    
            if(other.GetComponent<Death>().owner.GetComponent<Bee>().isResting == false && owner.GetComponent<Bee>().isResting == false)
            {
                // 자꾸 안 돼
                // 해결. 게임매니저를 prefab에 넣고 하면 실행이 제대로 안되서 findobjectoftype 을 썼다.
                Audio.gameOverSoundPlay();
                FindObjectOfType<GameManager>().gameOver();
            }
               
        }

        if (other.CompareTag("Death"))
        {
            Audio.losingScoreSoundPlay();
            BeeNumber.bee--;
            //  왜 스코어가 작동 안하지
            // 해결. 스코어 스크립에서 score = honey*10 으로 해놨어서 변경이 불가능했던것.
            Score.score -= 5;
            Destroy(owner);
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Flower"))
        {
            owner.GetComponent<Bee>().collidingFlower = false;

        }

        if (collision.CompareTag("Hive"))
        {
            owner.GetComponent<Bee>().collidingHive = false;

        }
    }
}
