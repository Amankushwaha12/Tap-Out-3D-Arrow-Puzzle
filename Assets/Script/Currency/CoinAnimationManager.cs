using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.InputSystem;
using TMPro;

public class CoinAnimationManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The UI Coin Prefab (Must have a RectTransform)")]
    public GameObject coinPrefab;
    [Tooltip("The topmost Canvas so coins render over everything")]
    public Transform mainCanvas; 
    [Tooltip("The UI Coin Counter it should fly to")]
    public Transform targetUIElement; 

    [Header("Animation Settings")]
    public float moveDuration = 0.8f;
    public Ease moveEase = Ease.InOutBack;

    [Header("Animation Settings")]
    [Tooltip("Total time the loss animation will take.")]
    public float animDuration = 1.0f;
    [Tooltip("How violently the icon shakes.")]
    public float shakeStrength = 15f;
    [Tooltip("The warning color the text turns into (e.g., Red).")]
    public Color lossColor = Color.red;

    public RectTransform coin;
    public TextMeshProUGUI coinText;



    public GameObject FromObject;

    public void Update()
    {
        if(Keyboard.current.lKey.wasPressedThisFrame)
        {
            SpawnAndAnimateCoin(FromObject.transform.position, targetUIElement.transform.position);
        }
        if(Keyboard.current.kKey.wasPressedThisFrame)
        {
            AnimateCoinLoss(coin, coinText, 500, 0);
        }
    }


    /// <summary>
    /// Instantiates a coin at a specific screen position and flies it to the target.
    /// </summary>
    public void SpawnAndAnimateCoin(Vector3 startScreenPosition, Vector3 targetScreenPosition, Action onComplete=null)
    {
        // 1. Instantiate the coin and parent it to the Main Canvas
        GameObject newCoin = Instantiate(coinPrefab, mainCanvas);

        // 2. Set its starting position to exactly where the event happened
        newCoin.transform.position = startScreenPosition;

        // 3. Start at scale zero for the pop-in effect
        newCoin.transform.localScale = Vector3.zero;
        newCoin.transform.DOScale(Vector3.one, 0.2f).OnComplete(() =>
        {
            // 4. Animate the movement to the UI counter
            newCoin.transform.DOMove(targetScreenPosition, moveDuration)
                .SetEase(moveEase)
                .OnComplete(() =>
                {
                    // Trigger the score update and clean up
                    onComplete?.Invoke();
                    Destroy(newCoin);
                });

            // 5. MATH: Calculate the timing for the last 10% of the journey
            float shrinkDuration = moveDuration * 0.1f; // The final 10% of time
            float shrinkDelay = moveDuration * 0.9f;    // Wait for 90% of the time to pass

            // 6. Start the scale-down animation, delayed until the coin is almost there
            newCoin.transform.DOScale(Vector3.zero, shrinkDuration)
                .SetDelay(shrinkDelay)
                .SetEase(Ease.InSine); // A smooth curve for disappearing
        });
    }

    /// <summary>
    /// Animates the coin icon shaking and the text counting down while turning red.
    /// </summary>
    public void AnimateCoinLoss(RectTransform coinIcon, TextMeshProUGUI coinText, int startValue, int endValue)
    {
        // 1. Store the original text color so we can revert back to it later
        Color originalColor = coinText.color;

        // 2. Shake the UI Icon
        // Parameters: duration, strength, vibrato, randomness, snapping
        coinIcon.DOShakeAnchorPos(animDuration, shakeStrength, 10, 90, false);

        // 3. Create a Sequence for the color change (Red -> Wait -> Original)
        Sequence colorSequence = DOTween.Sequence();
        colorSequence.Append(coinText.DOColor(lossColor, animDuration * 0.2f));
        colorSequence.AppendInterval(animDuration * 0.6f);
        colorSequence.Append(coinText.DOColor(originalColor, animDuration * 0.2f));

        // 4. Tween the actual number value down smoothly
        int currentValue = startValue;
        DOTween.To(() => currentValue, x => 
        {
            currentValue = x;
            coinText.text = currentValue.ToString(); // Update text every frame
        }, endValue, animDuration).SetEase(Ease.OutQuad);
    }
}