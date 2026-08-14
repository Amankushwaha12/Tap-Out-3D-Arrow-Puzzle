using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelObject : ScriptableObject
{
    [Header("Level Information")]
    public int levelNumber;
    
    [Header("Game Settings")]
    public float timeLimit;

    public string SceneName;
}
