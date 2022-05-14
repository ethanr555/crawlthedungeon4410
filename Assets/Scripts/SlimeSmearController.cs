using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSmearController : MonoBehaviour
{
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
        Invoke("Destruction", 1f);
    }

    void Destruction()
    {
        Destroy(gameObject);
    }
}
