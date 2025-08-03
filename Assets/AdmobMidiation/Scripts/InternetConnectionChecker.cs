using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InternetConnectionChecker : MonoBehaviour
{
    public static InternetConnectionChecker Instance;
    // URL for testing connectivity
    private const string TestURL = "https://www.google.com";

    // Time to wait for a response in seconds
    private const float Timeout = 3f;

    /// <summary>
    /// Starts the internet connection check.
    /// </summary>
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Checks if there is a valid working internet connection.
    /// </summary>
    /// <param name="callback">Action to handle the result (true for connected, false otherwise).</param>
    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        // Step 1: Check basic reachability
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            callback(false);
            yield break;
        }

        // Step 2: Ping a server to verify external access
        using (UnityWebRequest pingRequest = UnityWebRequest.Get(TestURL))
        {
            pingRequest.timeout = (int)Timeout;
            yield return pingRequest.SendWebRequest();

            if (pingRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Ping test failed: " + pingRequest.error);
                callback(false);
                yield break;
            }
        }

        // Step 3: Download small data to ensure full internet functionality
        using (UnityWebRequest dataRequest = UnityWebRequest.Get(TestURL))
        {
            dataRequest.timeout = (int)Timeout;
            yield return dataRequest.SendWebRequest();

            if (dataRequest.result == UnityWebRequest.Result.Success)
            {
                callback(true);
            }
            else
            {
                Debug.LogWarning("Data test failed: " + dataRequest.error);
                callback(false);
            }
        }
    }
}
