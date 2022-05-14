using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D body;
    public float speed;
    public int maxHealth = 20;
    public int health;
    BoxCollider2D meleeRange;
    public PlayerMeleeProjectile projectile;
    Vector2 directionFacing;
    public int damage;
    public bool hasKey = false;
    public float maxIFrames;
    float iframes;
    bool isAttacking;
    public SlimeSmearController smear;
    float smearCurrentCd;
    public float smearMaxCd;

    SpriteRenderer rend;
    Animator anim;

    GameController controller;
    // Start is called before the first frame update
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        health = maxHealth;
        directionFacing = new Vector2(0, -1);
        rend = GetComponent<SpriteRenderer>();
        anim  = FindObjectOfType<Animator>();
        controller = FindObjectOfType<GameController>();
        iframes = maxIFrames;
        isAttacking = false;
        controller.ChangeObjective("Find the key");
    }


    // Update is called once per frame
    void Update()
    {
        int tempdir = 0;
        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        body.AddForce(playerInput * speed * Time.deltaTime);
        if (iframes > 0) iframes -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            anim.SetBool("attacking", true);
            tempdir = 2;
            rend.flipX = false;
            Attack(new Vector3(0, 0, 90));
            isAttacking = true;

        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            anim.SetBool("attacking", true);
            tempdir = 1;
            rend.flipX = true;
            Attack(new Vector3(0, 0, 180));
            isAttacking = true;


        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {

            anim.SetBool("attacking", true);
            tempdir = 1;
            rend.flipX = false;
            Attack(new Vector3(0, 0, 0));
            isAttacking = true;


        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {

            anim.SetBool("attacking", true);
            tempdir = 0;
            rend.flipX = false;
            Attack(new Vector3(0, 0, 270));
            isAttacking = true;


        }
        else
        {
            anim.SetBool("attacking", false);
            isAttacking = false;

            if (body.velocity != new Vector2(0, 0))
            {
                anim.SetBool("moving", true);
                if (Mathf.Abs(body.velocity.x) < Mathf.Abs(body.velocity.y))
                {
                    if (body.velocity.y > 0)
                    {
                        tempdir = 2;
                    }
                    else
                    {
                        tempdir = 0;
                    }
                }
                else
                {
                    if (body.velocity.x < 0)
                    {
                        rend.flipX = true;
                    }
                    else
                        rend.flipX = false;
                    tempdir = 1;

                }

            }
            else
            {
                anim.SetBool("moving", false);
                tempdir = 0;
            }
            

        }
        anim.SetInteger("direction", tempdir);

        if (smearCurrentCd <= 0)
        {
            if (health < maxHealth / 2)
            {
                Instantiate(smear, transform.position, transform.rotation);
                smearCurrentCd = smearMaxCd;
            }

        }
        else
        {
            smearCurrentCd -= Time.deltaTime;
        }

    }

    public void Damage(int amount)
    {
        if (iframes <= 0)
        {
            Debug.Log("Damaged!");
            health -= amount;
            iframes = maxIFrames;
            if (health < 0)
            {
                Die();
            }
        }
       
    }

    void Die()
    {
        controller.gameOver();
    }

    void Attack(Vector3 directionFacing)
    {
        PlayerMeleeProjectile spawnedProjectile = Instantiate(projectile, transform.position, Quaternion.Euler(directionFacing));
        if (spawnedProjectile)
        {
            //spawnedArrow.direction = body.velocity;
            //spawnedArrow.direction = new Vector2(-1, 0);
            spawnedProjectile.damage = damage;
            spawnedProjectile.arrowSpeed = spawnedProjectile.arrowSpeed + body.velocity.magnitude;
            spawnedProjectile.player = this;
            spawnedProjectile.Shoot();
            //Debug.Log("Spawned");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if ( collision.gameObject.CompareTag("KeyHole") && hasKey)
        {
            Destroy(collision.gameObject);
        }
    }

    public void GetKey()
    {
        hasKey = true;
        controller.displayKey();
        controller.ChangeObjective("Find the boss");
    }

    public void Heal (int amt)
    {
        health += amt;
    }
}
