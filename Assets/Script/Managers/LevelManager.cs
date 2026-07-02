using System;
using System.Collections;
using UnityEngine;

// This script acts as the Master Conductor for the game flow.
public class LevelManager : MonoBehaviour
{
    [Header("Level Pipeline")]
    [Tooltip("Drag all your Level Prefabs here in order (Level_01, Level_02, etc.)")]
    [SerializeField] private GameObject[] _levelPrefabs;
    
    [Tooltip("The empty GameObject at 0,0,0 where levels will spawn")]
    [SerializeField] private Transform _levelContainer;

    [Header("Dependencies")]
    [SerializeField] private LineManager _lineManager;

    [Header("Pacing")]
    [SerializeField] private float _levelTransitionDelay = 1.5f;

    private int _currentLevelIndex = 0;
    private GameObject _currentLevelInstance;
    private bool _isTransitioning = false;

    public event Action<int> OnLevelStarted;
    public event Action OnLevelCompleted;
    public event Action OnLevelFailed;

    private void Start()
    {
        // Safety: find LineManager if not assigned in Inspector
        if (_lineManager == null) _lineManager = FindFirstObjectByType<LineManager>();
        
        // Listen for the Win Condition from the LineManager
        if (_lineManager != null)
        {
            _lineManager.OnAllLinesRemoved += HandleLevelCleared;
        }

        // Auto-start the first level
        LoadLevel(0);
    }

    private void OnDestroy()
    {
        if (_lineManager != null)
        {
            _lineManager.OnAllLinesRemoved -= HandleLevelCleared;
        }
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= _levelPrefabs.Length) return;

        _isTransitioning = false;

        // 1. Destroy old level
        if (_currentLevelInstance != null) Destroy(_currentLevelInstance);

        // 2. Reset Lives via the Singleton
        if (LivesManager.Instance != null) LivesManager.Instance.ResetLives();

        // 3. Spawn new level
        _currentLevelInstance = Instantiate(_levelPrefabs[index], _levelContainer);

        // 4. Initialize lines
        // if (_lineManager != null) _lineManager.InitializeLines(_levelContainer);

        // 5. Update UI
        _currentLevelIndex = index;
        OnLevelStarted?.Invoke(_currentLevelIndex + 1);
    }

    private void HandleLevelCleared()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        OnLevelCompleted?.Invoke();
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(_levelTransitionDelay);
        _currentLevelIndex++;
        if (_currentLevelIndex < _levelPrefabs.Length) LoadLevel(_currentLevelIndex);
    }

    public void RetryCurrentLevel()
    {
        LoadLevel(_currentLevelIndex);
    }
}