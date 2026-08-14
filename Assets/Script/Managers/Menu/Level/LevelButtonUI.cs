using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelButtonUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private LevelObject data;
    public GameObject CompletedOverLay, LockedOverLay;
    public CustomButton InteractableButton;
    public Image ButtonColor;
    public SceneTransitionManager sceneTransitionManager;


    void OnClick()
    {
        Debug.Log("Loading Level: " + data.levelNumber);
        if(BootstrapManager.Instance != null)
        {
            sceneTransitionManager.LoadLevel(data.SceneName);
        }
        // Add your scene loading or game initiation logic here
    }

    public void Setup(LevelObject levelData, bool completed, bool avilable)
    {
        data = levelData;
        levelText.text = levelData.levelNumber.ToString();
        if(!avilable)
        {
            // If completed, maybe change the button color or icon
            InteractableButton.interactable = completed; 
            if(completed)ButtonColor.color = new Color(1,1,1,1);
            LockedOverLay.SetActive(!completed);
            CompletedOverLay.SetActive(completed);
            InteractableButton.onClick.AddListener(OnClick);
            Debug.Log(levelData.SceneName);
        }
        else
        {
            InteractableButton.interactable = true; 
            LockedOverLay.SetActive(false);
            CompletedOverLay.SetActive(false);
            InteractableButton.onClick.AddListener(OnClick);
            Debug.Log(levelData.SceneName);
        }
    }
}