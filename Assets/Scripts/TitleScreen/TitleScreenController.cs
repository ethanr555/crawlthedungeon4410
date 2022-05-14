using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TitleScreenController : MonoBehaviour
{
    ScenePayload payload;
    public int roomamount = 8;
    public List<TileBase> ground;
    public List<TileBase> wall;
    public List<TileBase> pit;
    public GameObject grassBackground;
    public GameObject sandBackground;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        payload = FindObjectOfType<ScenePayload>();
        if (payload)
        {
            payload.ground = ground[0];
            payload.wall = wall[0];
            payload.pit = pit[0];
        }
        grassBackground.SetActive(true);
        sandBackground.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play8()
    {
        payload.roomnumber = 5;
        DontDestroyOnLoad(payload);
        SceneManager.LoadScene("SampleScene");
    }

    public void Play18()
    {
        payload.roomnumber = 15;
        DontDestroyOnLoad(payload);
        SceneManager.LoadScene("SampleScene");
    }

    public void Play28()
    {
        payload.roomnumber = 25;
        DontDestroyOnLoad(payload);
        SceneManager.LoadScene("SampleScene");
    }

    public void SetGrassTheme()
    {
        payload.ground = ground[0];
        payload.wall = wall[0];
        payload.pit = pit[0];
        grassBackground.SetActive(true);
        sandBackground.SetActive(false);

    }

    public void SetSandTheme()
    {
        payload.ground = ground[1];
        payload.wall = wall[1];
        payload.pit = pit[1];
        grassBackground.SetActive(false);
        sandBackground.SetActive(true);
    }
}
