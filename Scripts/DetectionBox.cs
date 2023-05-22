using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// collider for mouse clicking and drawing path
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

