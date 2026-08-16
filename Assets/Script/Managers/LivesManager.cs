using System;
using System.Collections;
using UnityEngine;

// This script acts as the Master Conductor for the game flow.
public class LivesManager : MonoBehaviour
{
    [Header("Dependecies")]
    public LevelManager levelManager;
    [Header("Values")]
    public int MaxLives;

    [Header("Variables")]
    public int lives;

    public Action GetExtraLives;

    public GameObject[] livesImageObj;

    void Awake()
    {
        if(levelManager == null) Debug.LogError("No Level Manager Asigned");
        GetExtraLives += ResetLives;
    }

    public void Damage()
    {
        if(lives>0)livesImageObj[lives---1].SetActive(false);
        if(lives<=0)levelManager.NoLiveRemain();
    }

    public void ResetLives()
    {
        lives = MaxLives;
        for (int i = 0; i < MaxLives; i++)
        {
            livesImageObj[i].SetActive(true);
        }
    }
}