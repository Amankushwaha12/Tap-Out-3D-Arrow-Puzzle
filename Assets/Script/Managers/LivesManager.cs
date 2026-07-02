using System;
using System.Collections;
using UnityEngine;

// This script acts as the Master Conductor for the game flow.
public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance;

    [Header("Values")]
    public int MaxLives;

    [Header("Variables")]
    public int lives;

    void Awake()
    {
        Instance = this;
    }

    public void ResetLives()
    {
        lives = MaxLives;
    }
}