using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastPoint : MonoBehaviour
{
    SpriteRenderer sprite;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Flower"))
        {
            sprite.color = new Color(1,0,0,1);
        }

        if(other.CompareTag("Hive"))
        {
            sprite.color = new Color(1, 0.92f, 0.016f, 1);
        }
    }
}
