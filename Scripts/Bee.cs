using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    // public Collider2D[] colliders;
    GameManager gameManager;
    public GameObject hive;
    public GameObject flower;

    public bool collidingFlower = false;
    public bool collidingHive = false;
    //
    public bool carryingHoney = false;
    public bool isResting = false;
    //
    public static int Honey = 0;
    private Animator anim;


    public Transform[] target;
    private int randomSpot;
    public float speed = 3f;
    public Rigidbody2D rb;
    private bool drawing = false;
    public int lastIndex = 0;
    //
    [Header("Components")]
    public LineRenderer lr;


    //Not customizable:
    public int currentWayPoint = 0;
    private int wayIndex;
    private bool touchStartedOnPlayer;
    private int lrCounter = 0;
    // private bool readyToDestroyLastPoint = false;
    public List<GameObject> wayPoints;
    public List<GameObject> lastPoints;
    //public List<GameObject> Particles;
    [Header("Prefabs")]
    public GameObject wayPoint;
    public GameObject lastPoint;
    public GameObject NectarParticle;
    //

    //public CircleCollider2D warningCollider;
    // public CircleCollider2D beeCollider;

    private Vector2 position;

    void Start()
    {
        position = transform.position;
        //warningCollider = GetComponent<CircleCollider2D>();
        //beeCollider = GetComponent<CircleCollider2D>();
        isResting = false;
        anim = GetComponent<Animator>();
        randomSpot = Random.Range(0, target.Length);

        rb = GetComponent<Rigidbody2D>();
        wayIndex = 1;
        touchStartedOnPlayer = false;
        // colliders[0] = gameObject.GetComponentInChildren<Collider2D>(); // for warning
        // colliders[1] = gameObject.GetComponentInChildren<Collider2D>(); // for game over
    }

    // 한 오브젝트에 콜라이더가 너무 많아서 이게 작동을 제대로 안함....
    // 해결. 따로 gameobject를 만들었음. 

    private void OnMouseDown()
    {
        // 선 그리고 나서 다시 벌 위에 클릭하면 다시 긋도록
        // 해결. 다시 터치할때 Point들 리셋하도록 해서 해결함. 
        // last point가 없는데 지나다가다 collide 할 때 에러남.]
        // 해결. 그게 아니라 그냥 선을 다 긋지도 않았는데 도착햇을 때 에러가 난거라 lastpoint가 생겼고 flower 나 hive에 collide 해야 반응 일어나는 걸로 고침.
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

                    // 마지막 waypoint는 눈에 띄게 아이콘을 두고 싶다.
                    // 해결
                    // Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    // Vector3 direction = worldMousePos - (Vector2)Camera.main.transform.position;

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
            // Destroy(Particles[0]);
            // Particles.Clear();
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
            // 도착하면 last point랑 애들 사라지게
            // 해결.
            if (wayPoints.Count >= 1)
            {
                lr.positionCount = 1;
                lr.enabled = false;
                resetPoints();
                //
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


    // 벌을 클릭만 하거나 짧게 선을 그으면 out of range 에러가 남.
    // drawing bool 문제인듯?
    // 마우스를 뗐을 때 wayPoints.Count >= 1 조건을 두어서 짧게 클릭했을때는 에러 X
    // 하지만 짧게 그은 후 마지막 지점에 도착했을 때 followLine에서 에러 발생.
    // 해결. 李 그으면 lastIndex가 0이 되는데 마지막 지점 도착 시에 조건이 lastIndex != 0 이기때문에 에러 발생.




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
            // 마우스를 클릭한 채로 움직이지 않을 때 에러 발생
            // Destroy 한걸 access 하려 한다고 에러가 남. 흠...
            // 앞에 wayPoints[wayIndex-2]가 존재할때라는 조건을 붙여주니 이 에러는 사라졌지만 followLine에서 out of range 에러 발생.
            // 해결. currentWayPoint + 1 <= wayPoints.Count 에서 <=를 <로 바꿈. 같을 때도 실행되니 out of range가 나올 수밖에..

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



    // 다시 선 그으려고 할 때 에러나는 거 여기서 문제 있는 것 같음
    // 왜 처음 그을 때 문제 없는 애가 두번째 긋기 시작하면 OUT OF RANGE 에러가 나지??? 초기화 문제인가...
    // 해결. object 클릭 시 lrCounter, currnetWayPoint, lastIndex를 0값으로 초기화시킴.

    // 이제 긴 선, 곡선을 그으려 하면 out of range 에러 발생.....
    // 해결. 마지막 lastIndex를 통과할 때 조건에 lastIndex == 0 && wayPoints.Count >= 1 를 lastIndex == 0 && wayPoints.Count == 1로 교체. 
    // lastIndex가 0인 경우는 wayIndex가 1일때 뿐.

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
                    // 지나간 부분은 line도 안보이게 하고 싶음.
                    // 지운 웨이포인트 빼고 나머지를 line으로 이어주는 식?
                    // 해결
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


            // destroy한 걸 access 하려 한다고 error남
            // 해결. destroy를 먼저 하면 안 됐음.
            // 긴 선, 곡선을 그을 때 에러 발생. 뭐가 문제?
            // 해결. lastIndex가 정의되지 않았는데도 식에 있어서 에러가 났던 것. 앞에 lastIndex != 0을 붙여주니 해결~ 
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

                //waypoints들이 정리가 안됨.... + 다시 같은 벌을 조작할때 에러남.
                // 해결
                wayPoints.Clear();
            }

            // 이 부분 적용이 안됨... 마지막 waypoint까지 간 후에 이 부분 적용하고싶은데. 마우스 누르는 동안에만 시행되기 때문!
            // 해결 했는데 잘 가다가 라인이 사라짐. 그리고 도착하고 나면 index 에러가 남. out of range...
            // 에러는 이제 안뜨는데 도착하고 나서 움직이질 않음.
            // 해결. 어찌된 영문인지 randomFly 실행 안해도 지저절로 가더라 ㅋㅋ
            // if (currentWayPoint == wayPoints.Count - 1)
            //{
            //     randomFly();

            //}
        }
    }
    // 동작을 안하는구먼?
    // BeeIdle일 때로 해보니까 작동하던데 얘 애니메이션이 문제가 있나봄.
    // 혹시 다른 벌의 애니메이션까지 생각해서?.......
    // timer가 0.02밖에 증가가 안됨. 뭐가 문제?...
    // maxTimer를 0.02로 만들었는데도 안됨.
    // ontriggerented2d fuction에서는 무조건 0.02인가봄.... 여기서 타이머 못쓰겠네.
    // 아니 시발 왜 작동 안해?
    // 해결!!!!!!!!!!!! 애니메이션 끝에 이벤트를 추가해서 해당 함수(아래)를 실행하게 했다!!
    // ************ 아니 근데 될 때도 있고 안 될때도 있으면 우짜라고??
    void stopResting()
    {
        isResting = false;
        Audio.EmergeSoundPlay();

    }






}