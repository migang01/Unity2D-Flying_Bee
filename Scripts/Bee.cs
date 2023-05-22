using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    GameManager gameManager;
    public GameObject hive;
    public GameObject flower;

    public bool collidingFlower = false;
    public bool collidingHive = false;
    
    public bool carryingHoney = false;
    public bool isResting = false;
    
    public static int Honey = 0;
    private Animator anim;


    public Transform[] target;
    private int randomSpot;
    public float speed = 3f;
    public Rigidbody2D rb;
    private bool drawing = false;
    public int lastIndex = 0;
    
    [Header("Components")]
    public LineRenderer lr;

    public int currentWayPoint = 0;
    private int wayIndex;
    private bool touchStartedOnPlayer;
    private int lrCounter = 0;
   
    public List<GameObject> wayPoints;
    public List<GameObject> lastPoints;
    
    [Header("Prefabs")]
    public GameObject wayPoint;
    public GameObject lastPoint;
    public GameObject NectarParticle; 

    private Vector2 position;

    void Start()
    {
        position = transform.position;
        isResting = false;
        anim = GetComponent<Animator>();
        randomSpot = Random.Range(0, target.Length);

        rb = GetComponent<Rigidbody2D>();
        wayIndex = 1;
        touchStartedOnPlayer = false;
    }

    // there was too many colliders in one object so it didn't work -> fixed by adding game object
    private void OnMouseDown()
    {
        // when touch bee again rest the point
        if (lastPoints.Count != 0)
        {
            resetPoints();
        }

        lr.enabled = true;

        touchStartedOnPlayer = true;
        wayIndex = 1;

        lr.positionCount = 1;
        lr.SetPosition(0, transform.position);

        lrCounter = 0;
        currentWayPoint = 0;
        lastIndex = 0;

    }

    private void resetPoints()
    {
        if (isResting == false)
        {
            Destroy(lastPoints[0]);
            lastPoints.Clear();
        }


        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (wayPoints[i])
            {
                Destroy(wayPoints[i]);
            }

        }

        wayPoints.Clear();
    }
    void Update()
    {


        if (isResting == false)
        {

            randomFly();

            if (Input.GetMouseButton(0) && touchStartedOnPlayer == true)
            {
                drawLine();

            }


            if (Input.GetMouseButtonUp(0))
            {
                touchStartedOnPlayer = false;
                if (drawing == true && wayPoints.Count >= 1)
                {
                    lastIndex = wayPoints.Count - 1;

                    GameObject newLastPoint = Instantiate(lastPoint, wayPoints[lastIndex].transform.position, Quaternion.identity);
                    lastPoints.Add(newLastPoint);

                    Vector3 direct = newLastPoint.transform.position - transform.position;
                    float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
                    rb.rotation = angle;
                    transform.position = Vector2.MoveTowards(transform.position, newLastPoint.transform.position, speed * Time.deltaTime);
                }
                drawing = false;

            }


            if (wayPoints.Count >= 1)
            {
                followLine();
            }


            if (carryingHoney == true && isResting == false)
            {
                NectarParticle.SetActive(true);
            }


        }

        if (carryingHoney == true && collidingHive == true && lastPoints.Count > 0)       // Arrive to Hive with Honey
        {
            Audio.ScoreSoundPlay();
            Score.score += 10;
            HoneyNumber.honey += 1;
            HoneyNumber.lastHNumber = HoneyNumber.honey - 1;

            NectarParticle.SetActive(false);

            if (wayPoints.Count >= 1)
            {
                lr.positionCount = 1;
                lr.enabled = false;
                resetPoints();
            }

            anim.SetTrigger("Arrive");
            carryingHoney = false;
            isResting = true;
        }

        if (carryingHoney == false && collidingFlower == true && lastPoints.Count > 0)     // Arrive to Flower without Honey
        {
            Audio.ArriveSoundPlay();
            if (wayPoints.Count >= 1)
            {
                lr.positionCount = 1;
                lr.enabled = false;
                resetPoints();                
            }

            anim.SetTrigger("Arrive");
            carryingHoney = true;
            isResting = true;
        }
    }

    private void randomFly()
    {
        Vector3 direct = target[randomSpot].position - transform.position;
        float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        transform.position = Vector2.MoveTowards(transform.position, target[randomSpot].position, speed * Time.deltaTime);
    }

    // out of range error when player just clicks bee or it's short line -> assume because of drawing boolean?
    // fixed1: "wayPoints.Count >= 1" condition when left mouse button up so there's no error when palyer only clicks bee
    // but when bee arrives somewhere with short line there's an error at "followLine"
    // fixed2: because of condition "lastIndex != 0" when lastIndex is 0

    private void drawLine()
    {
        drawing = true;
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        Vector3 direction = worldMousePos - (Vector2)Camera.main.transform.position;


        if (wayIndex == 1) // at first
        {
            if (Vector2.Distance(direction, transform.position) > 1f)
            {
                GameObject newWaypoint = Instantiate(wayPoint, direction, Quaternion.identity);
                wayPoints.Add(newWaypoint);
                lr.positionCount = wayIndex + 1;
                lr.SetPosition(wayIndex, newWaypoint.transform.position);
                wayIndex++;
            }
        }
        else
        {
            // error when mouse button is down and not moving - saying I'm trying to access destroyed object
            // fixed1: adding condition "wayPoints[wayIndex-2] is existing" but out of range error at followLine
            // fixed2: changed "<=" to "<" at "currentWayPoint + 1 <= wayPoints.Count" there's out of range because it's executing when it's same

            if (wayPoints[wayIndex - 2] && Vector2.Distance(direction, wayPoints[wayIndex - 2].transform.position) > .5f)
            {
                GameObject newWaypoint = Instantiate(wayPoint, direction, Quaternion.identity);
                wayPoints.Add(newWaypoint);
                lr.positionCount = wayIndex + 1;
                lr.SetPosition(wayIndex, newWaypoint.transform.position);
                wayIndex++;
            }



        }



    }

    // fixed out of range error when tyring to draw line again by initializing lrCounter, currnetWayPoint, lastIndex as 0

    // when drawing long and curvy line there's out of range error
    // fixed: "lastIndex == 0 && wayPoints.Count >= 1" -> "lastIndex == 0 && wayPoints.Count == 1" 
    // lastIndex is 0 only when wayIndex is 1

    private void followLine()
    {
        if (wayPoints[currentWayPoint])
        {
            Vector2 direct = wayPoints[currentWayPoint].transform.position - transform.position;
            float angle = Mathf.Atan2(direct.y, direct.x) * Mathf.Rad2Deg;
            rb.rotation = angle;

            transform.position =
                Vector2.MoveTowards(transform.position, wayPoints[currentWayPoint].transform.position, 3f * Time.deltaTime);


            if (transform.position == wayPoints[currentWayPoint].transform.position)
            {
                if (currentWayPoint + 1 < wayPoints.Count)
                {
                    Destroy(wayPoints[currentWayPoint]);

                    for (int j = 0; j < lr.positionCount; j++)
                    {
                        if (lr.GetPosition(j) == wayPoints[currentWayPoint].transform.position)
                        {
                            lrCounter = j;
                            for (int p = 0; p < lrCounter; p++)
                            {
                                lr.SetPosition(p, lr.GetPosition(p + 1));

                                for (int i = 0; i < p; i++)
                                {
                                    lr.SetPosition(i, lr.GetPosition(j));
                                }
                            }
                            break;
                        }
                    }

                    currentWayPoint++;
                }

            }

            if ((lastIndex != 0 || (lastIndex == 0 && wayPoints.Count == 1)) && transform.position == wayPoints[lastIndex].transform.position)
            {
                lr.positionCount = 1;
                // 왜 실행이 안되는걸까?
                // 마지막 waypoint에 생기게 했는데도 안없어지네...
                // 벌의 위치가 마지막 포인트와 같지가 않나봄. 어케 해결하지...
                // 해결. lastPoints 라는 list를 만들고 0번째 인덱스에 저장후에 그걸 그냥 destroy했음.
                Destroy(lastPoints[0]);
                lastPoints.Clear();
                lr.enabled = false;

                Destroy(wayPoints[lastIndex]);

                wayPoints.Clear();
            }      
        }
    }
    // using this function at the animation adding event
    void stopResting()
    {
        isResting = false;
        Audio.EmergeSoundPlay();

    }






}
