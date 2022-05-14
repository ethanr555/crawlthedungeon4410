using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRanged : Enemy
{
    [SerializeField]
    float currentArrowCD;
    public float arrowCD;
    public Arrow arrow;
    Arrow spawnedArrow;
    List<Quaternion> directions;
    int currentIndex;
    Animator anim;
    SpriteRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void AwakeInternal()
    {
        base.AwakeInternal();
        currentArrowCD = arrowCD;
        currentIndex = 0;
        directions = new List<Quaternion>();
        directions.Add(Quaternion.Euler(0, 0, 0));
        directions.Add(Quaternion.Euler(0, 0, 90));
        directions.Add(Quaternion.Euler(0, 0, 180));
        directions.Add(Quaternion.Euler(0, 0, 270));
        anim = FindObjectOfType<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rend.flipX = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedArrow && spawnedArrow.isActiveAndEnabled)
        {
            //spawnedArrow.direction = body.velocity;
            //spawnedArrow.direction = new Vector2(-1, 0);
            spawnedArrow.damage = damage;
            spawnedArrow.Shoot();
            //Debug.Log("Spawned");
            spawnedArrow = null;
        }
        if (currentArrowCD > 0)
        {


            currentArrowCD -= Time.deltaTime;
        }
        else
        {
            spawnedArrow = Instantiate(arrow, transform.position, directions[currentIndex]);
            anim.SetTrigger("attacking");
            //Rigidbody2D ArrowBody = spawnedArrow.GetComponent<Rigidbody2D>();
            //ArrowBody.AddForce(body.velocity * arrowSpeed);
            currentArrowCD = arrowCD;
            currentIndex++;
            if (currentIndex >= directions.Count)
            {
                currentIndex = 0;
            }
            if (currentIndex == 1)
            {
                anim.SetInteger("direction", 0);
            }
            else if (currentIndex == 0 || currentIndex == 2)
            {
                anim.SetInteger("direction", 1);
                if (currentIndex == 0)
                {
                    rend.flipX = true;
                }
                else
                {
                    rend.flipX = false;
                }
            }
            else
            {
                anim.SetInteger("direction", 2);
            }

        }
    }
}
