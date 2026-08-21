using UnityEngine;

public class RouletteManager : MonoBehaviour
{
    public GameObject Handle;
    [Header("Oscillator Settings")]
    [Tooltip("How fast the pointer sweeps back and forth.")]
    public float speed = 3f;
    [Tooltip("How far the pointer travels from the center.")]
    public float distance = 4f;

    [Header("Reward Zones (Distance from Center)")]
    [Tooltip("If stopped within this distance, award x5")]
    public float centerZoneBoundary = 0.5f; 
    [Tooltip("If stopped within this distance, award x3")]
    public float midZoneBoundary = 1.5f;    

    private bool isMoving = false;
    private Vector3 startPos;
    private float oscillationTime = 0f;

    private void Awake()
    {
        // Store the exact center starting position
        startPos = Handle.transform.localPosition;
    }

    private void Update()
    {
        // The pointer only moves if the isMoving flag is true
        if (isMoving)
        {
            // Advance our custom time variable
            oscillationTime += Time.deltaTime * speed;
            
            // Calculate the smooth wave
            float wave = Mathf.Sin(oscillationTime);
            float newX = startPos.x + (wave * distance);
            
            // Apply movement
            Handle.transform.localPosition = new Vector3(newX, startPos.y, startPos.z);
        }
    }

    // --- FUNCTION CALLS --- //

    /// <summary>
    /// Call this function to start the roulette movement.
    /// </summary>
    public void StartRoulette()
    {
        if(!isMoving)
        {
            isMoving = true;
            oscillationTime = 0f; // Reset time so it always starts from the center
            Debug.Log("[Roulette] Spinning started!");
        }
        else
        {
            StopAndCalculateReward();
        }
    }

    /// <summary>
    /// Call this function from your InputManager when the screen is tapped.
    /// </summary>
    public void StopAndCalculateReward()
    {
        if (!isMoving) return; // Prevent double-tapping

        isMoving = false;
        
        // Find out exactly how far the pointer is from the absolute center
        float distanceFromCenter = Mathf.Abs(Handle.transform.localPosition.x - startPos.x);
        
        // Pass that distance into our calculator function
        int finalMultiplier = CalculateMultiplier(distanceFromCenter);
        
        Debug.Log($"[Roulette] Stopped at distance: {distanceFromCenter}. Reward: x{finalMultiplier}");
        
        // TODO: Apply the finalMultiplier to the player's score or currency here!
    }

    /// <summary>
    /// Evaluates the distance and returns the correct multiplier.
    /// </summary>
    private int CalculateMultiplier(float distance)
    {
        // Check from the center outwards
        if (distance <= centerZoneBoundary)
        {
            return 5; // Bullseye! Premium x5 reward
        }
        else if (distance <= midZoneBoundary)
        {
            return 3; // Medium x3 reward
        }
        else
        {
            return 2; // Outer edges x2 reward
        }
    }
}