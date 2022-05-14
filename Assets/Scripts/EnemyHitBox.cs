using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    Enemy enemy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>())
            {
                collision.GetComponentInParent<PlayerController>().Damage(enemy.damage);
            }
            
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.GetComponent<PlayerController>())
            {
                collision.GetComponentInParent<PlayerController>().Damage(enemy.damage);
            }

        }
    }
}
