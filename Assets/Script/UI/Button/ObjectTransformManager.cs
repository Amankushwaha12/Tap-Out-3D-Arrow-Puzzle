using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ObjectTransformManager : MonoBehaviour
{
    [System.Serializable]
    public class TransformEntry
    {
        public RectTransform targetRect;
        public Vector2 pressedOffset;
        public float pressedScale = 0.9f;
        public float duration = 0.1f;
    }

    public List<TransformEntry> managedObjects = new List<TransformEntry>();

    public void OnPressed()
    {
        foreach (var entry in managedObjects)
        {
            if (entry.targetRect == null) continue;
            entry.targetRect.DOKill();
            entry.targetRect.DOAnchorPos(entry.targetRect.anchoredPosition + entry.pressedOffset, entry.duration);
            entry.targetRect.DOScale(entry.pressedScale, entry.duration);
        }
    }

    public void OnReleased()
    {
        foreach (var entry in managedObjects)
        {
            if (entry.targetRect == null) continue;
            entry.targetRect.DOKill();
            // Return to start (Assuming you have a saved 'original' state)
            entry.targetRect.DOAnchorPos(Vector2.zero, entry.duration); 
            entry.targetRect.DOScale(Vector3.one, entry.duration);
        }
    }
}