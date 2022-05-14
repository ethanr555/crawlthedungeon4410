using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeProjectile : MonoBehaviour
{
    public float arrowSpeed;
    public Vector2 direction;
    public int damage;
    Rigidbody2D body;
    public PlayerController player;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        //Shoot(direction, arrowSpeed);
        Invoke("Expired", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponentInParent<Enemy>().Damage(damage, player);
            Debug.Log(collision);
            Destroy(gameObject);
        }
    }

    public void Shoot()
    {

        Vector2 rotation;
        if (transform.rotation.eulerAngles.z == 0f)
        {
            rotation = new Vector2(1, 0);
        }
        else if (transform.rotation.eulerAngles.z == 90f)
        {
            rotation = new Vector2(0, 1);

        }
        else if (transform.rotation.eulerAngles.z == 180f)
        {
            rotation = new Vector2(-1, 0);

        }
        else
        {
            rotation = new Vector2(0, -1);

        }


        body.AddForce(rotation * arrowSpeed);
    }
    void Expired()
    {
        Destroy(this.gameObject);
    }
}
