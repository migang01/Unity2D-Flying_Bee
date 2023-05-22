using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionBox : MonoBehaviour
{
    GameManager gameManager;
    public GameObject ownerB;

    private bool drawing;
    private int lastIndex;

    private LineRenderer lr;

    private int currentWayPoint;
    private int wayIndex;
    private bool touchStartedOnPlayer;
    private int lrCounter;

    public List<GameObject> wayPoints;
    public List<GameObject> lastPoints;

    public GameObject wayPoint;
    public GameObject lastPoint;

    private void Start()
    {
        lastIndex = 0;
        currentWayPoint = 0;
        lrCounter = 0;
        drawing = false;
    }

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
        if (ownerB.GetComponent<Bee>().isResting == false)
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

    private void Update()
    {
        if (ownerB.GetComponent<Bee>().isResting == false)
        {
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
                    ownerB.GetComponent<Bee>().rb.rotation = angle;
                    transform.position = Vector2.MoveTowards(transform.position, newLastPoint.transform.position, ownerB.GetComponent<Bee>().speed * Time.deltaTime);
                }
                drawing = false;
            }
        }

       
    }
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

        // 마우스를 클릭한 채로 움직이지 않을 때 에러 발생
        // Destroy 한걸 access 하려 한다고 에러가 남. 흠...
        // 앞에 wayPoints[wayIndex-2]가 존재할때라는 조건을 붙여주니 이 에러는 사라졌지만 followLine에서 out of range 에러 발생.
        // 해결. currentWayPoint + 1 <= wayPoints.Count 에서 <=를 <로 바꿈. 같을 때도 실행되니 out of range가 나올 수밖에..
        else
        {

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
}

