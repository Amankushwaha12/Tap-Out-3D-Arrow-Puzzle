using System;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    // The registry of all lines currently on the board
    public List<LineController> _activeLines = new List<LineController>();

    // This is required by the LineAnimation scripts provided earlier
    public Vector3ArrayPool _vector3ArrayPool;

    // This event signals to your LevelManager that the level is cleared
    public event Action OnAllLinesRemoved;

    public void InitializeLines(Transform levelRoot)
    {
        _activeLines.Clear();
        
        // Find all Lines in the new level container
        LineController[] lines = levelRoot.GetComponentsInChildren<LineController>(true);
        
        foreach (LineController line in lines)
        {
            _activeLines.Add(line);
            line.Initialize(); // Pass 'this' so the line knows which manager it belongs to
        }
    }

    public void RegisterLine(LineController line)
    {
        if (!_activeLines.Contains(line))
        {
            _activeLines.Add(line);
        }
    }
    public void UnregisterLine(LineController line)
    {
        if (_activeLines.Contains(line))
        {
            _activeLines.Remove(line);
            
            // Check if we just cleared the very last line
            if (_activeLines.Count == 0)
            {
                OnAllLinesRemoved?.Invoke();
            }
        }
    }

    public void Cleanup()
    {
        foreach (var line in _activeLines)
        {
            // if (line != null) line.Cleanup();
        }
        _activeLines.Clear();
    }
}