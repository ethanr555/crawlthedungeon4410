using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : Enemy
{

    SpriteRenderer rend;
    Animator anim;
    public PlayerController player;
    public float distance;
    WorldGenController worldcont;
    bool hasSpawnedWalls = false;
    GameController cont;
    public SlimeSmearController smear;
    float smearCurrentCd;
    public float smearMaxCd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        float currentDistance = Vector2.Distance(player.transform.position, transform.position);
        float distancex = player.transform.position.x - transform.position.x;
        float distancey = player.transform.position.y - transform.position.y;
        if (currentDistance < distance )
        {
            if (!hasSpawnedWalls)
            {
                hasSpawnedWalls = worldcont.SpawnBossWall();
                cont.displayBossUI();
            }
            anim.SetBool("moving", true);
            if (Mathf.Abs(distancex) <= Mathf.Abs(distancey))
            {
                anim.SetInteger("direction", 1);
            }
            else
            {
                if (distancey < 0)
                {
                    anim.SetInteger("direction", 0);
                }
                else
                {
                    anim.SetInteger("direction", 2);
                }
            }
            body.AddForce((player.transform.position - transform.position) * speed * Time.deltaTime);
            if (smearCurrentCd <= 0)
            {
                Instantiate(smear, transform.position, transform.rotation);
                smearCurrentCd = smearMaxCd;
            }
            else
            {
                smearCurrentCd -= Time.deltaTime;
            }

        }
        else
        {
            anim.SetBool("moving", false);
        }
    }

    protected override void AwakeInternal()
    {
        base.AwakeInternal();
        player = FindObjectOfType<PlayerController>();
        rend = GetComponent<SpriteRenderer>();
        anim = FindObjectOfType<Animator>();
        worldcont = FindObjectOfType<WorldGenController>();
        cont = FindObjectOfType<GameController>();
        cont.boss = this;
        smearCurrentCd = smearMaxCd;
    }

    public float GetHealthRatio()
    {
        return (float)health / (float)maxHealth;
    }

    protected override void Die(PlayerController player)
    {
        cont.winGame();
        base.Die(player);
    }
}
