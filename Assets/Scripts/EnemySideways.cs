using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySideways : Enemy
{
    bool directionRight = false;
    SpriteRenderer rend;
    public float smearMaxCd;
    float smearCurrentCd;
    public SlimeSmearController smear;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    protected override void AwakeInternal()
    {
        rend = GetComponent<SpriteRenderer>();
        smearCurrentCd = smearMaxCd;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!body.simulated) body.simulated = true;
        int direction = -1;
        if (directionRight)
        {
            direction = 1;
            rend.flipX = false;

        }
        else
        {
            rend.flipX = true;

        }
        body.AddForce(direction * new Vector2(speed * Time.deltaTime, 0));
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
       // Debug.Log("Hit");
        directionRight = !directionRight;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       // directionRight = !directionRight;
        //body.simulated = false;

    }

}
