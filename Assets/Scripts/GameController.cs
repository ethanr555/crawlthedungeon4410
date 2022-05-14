using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Image healthBar;
    public GameObject GameOverScreen;
    public GameObject WinScreen;
    bool isGameOver = false;
    public GameObject KeyUI;
    public GameObject[] BossUI;
    public Image bossHP;
    public BossController boss;
    public AudioSource audio;
    public Text objective;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void ChangeObjective(string description)
    {
        objective.text = description;
    }

    // Update is called once per frame
    void Update()
    {
        bossHP.fillAmount = boss.GetHealthRatio();
        healthBar.fillAmount = (float) FindObjectOfType<PlayerController>().health / (float) FindObjectOfType<PlayerController>().maxHealth;
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("TitleScreen");
        }

    }

    void UpdatePlayerHealth(int health, int maxHealth)
    {
        
    }

    public void gameOver()
    {
        Time.timeScale = 0;
        GameOverScreen.SetActive(true);
        isGameOver = true;
        audio.Stop();
    }

    public void winGame()
    {
        Time.timeScale = 0;
        WinScreen.SetActive(true);
        isGameOver = true;
        audio.Stop();

    }

    public void displayKey()
    {
        KeyUI.SetActive(true);
    }

    public void displayBossUI()
    {
        for (int i = 0; i < BossUI.Length; i++)
        {
            BossUI[i].SetActive(true);
        }
        objective.gameObject.SetActive(false);
    }

}
