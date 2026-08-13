using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GoogleAuthManager : MonoBehaviour
{
    public static GoogleAuthManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGoogle();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGoogle()
    {
        // 1. Just activate the platform
        PlayGamesPlatform.Activate();
        
        // 2. Start the authentication process immediately
        TrySilentLogin();
    }

    private void TrySilentLogin()
    {
        // In v2, simply call Authenticate. 
        // It automatically handles the "silent" check first.
        PlayGamesPlatform.Instance.Authenticate((status) => 
        {
            if (status == SignInStatus.Success)
            {
                Debug.Log("Signed in successfully!");
            }
            else
            {
                Debug.Log("Login failed (Status: " + status + ").");
            }
        });
    }
}