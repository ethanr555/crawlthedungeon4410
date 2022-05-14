using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyType
{
    small,
    large,
    boss
}
public class Enemy : MonoBehaviour
{
    public int maxHealth;
    [SerializeField]
    protected int health;
    public float speed;
    protected Rigidbody2D body;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();

        health = maxHealth;
        AwakeInternal();

    }

virtual protected void AwakeInternal()
    {

    }

    public void Damage(int amount, PlayerController player)
    {
        Debug.Log("Damaged!");
        health -= amount;
        if (health < 0)
        {
            Die(player);
        }
    }

    virtual protected void Die(PlayerController player)
    {
        player.Heal(1);
        Destroy(gameObject);
    }
}
