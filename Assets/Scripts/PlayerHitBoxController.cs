using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBoxController : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerController player;
    void Start()
    {
        
    }

    private void Awake()
    {
        player = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public PlayerController GetPlayer()
    {
        return player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision);
        if (collision.gameObject.CompareTag("Key"))
        {
            player.GetKey();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Pits"))
        {
            player.Damage(2);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Pits"))
        {
            player.Damage(2);
        }
    }
}
