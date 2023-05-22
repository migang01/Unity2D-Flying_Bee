using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallingArea : MonoBehaviour
{
    public GameObject hive;
    public Collider2D colliderBox;
    public static bool stopTime = false;

    private void Start()
    {
        colliderBox.enabled = false;
    }

    private void Update()
    {
        if(InstallingManager.onImage == true)
        {
            colliderBox.enabled = true;
            Time.timeScale = 0;

        }
    }
    private void OnMouseDown()
    {

        if (Input.GetMouseButtonDown(0) && InstallingManager.onImage == true)
        {
            Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
            Vector3 direction = worldMousePos - (Vector2)Camera.main.transform.position;

            GameObject newHive = Instantiate(hive, direction, Quaternion.identity);
            HiveNumber.hive++;      
            HiveNumber.newHive--; 

            Score.score += 10;
            Audio.ScoreSoundPlay();

        }
        InstallingManager.onImage = false;
        InstallingManager.stop = false;
        colliderBox.enabled = false;
        Time.timeScale = 1;

    }
}
