using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HiveNumber : MonoBehaviour
{
    public Text hiveText;
    public static int hive = 1;
    public static int newHive = 0;

    void Update()
    {
        hiveText.text = newHive.ToString() + "/" + hive.ToString(); // installable hive / installed hive
    }
}