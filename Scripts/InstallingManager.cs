using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstallingManager : MonoBehaviour
{
    public static bool onImage = false;
    public static bool stop = false;
    public void ColickedHive()
    {

        if(HiveNumber.newHive > 0)
        {
            stop = true;
            onImage = true;
        }
        
        
    }

}
