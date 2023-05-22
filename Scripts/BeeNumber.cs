using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeeNumber : MonoBehaviour
{
    public Text beeText;
    public static int bee;
    public GameManager gameManager;


    private void Start()
    {
        bee = 0;
    }
    void Update()
    {
        beeText.text = bee.ToString();

        if(bee <= 0 && gameManager.startCanvasOn == false)
        {
            gameManager.gameOver();
        }

    }
}