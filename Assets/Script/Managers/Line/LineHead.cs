using UnityEngine;

public class LineHead : MonoBehaviour
{
    [SerializeField] private LineController owner;

    private void OnTriggerEnter2D(Collider2D other)
    {
        LineController otherLine = other.GetComponentInParent<LineController>();

        if (otherLine == null)
            return;

        if (otherLine == owner)
            return;

        owner.Reverse();
    }
}