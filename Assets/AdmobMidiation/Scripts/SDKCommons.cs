using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDKCommons : MonoBehaviour
{
    public static SDKCommons Instance;
    public enum GameMode { None = 0, Development = 1, Production = 2 };
    public GameMode Game_Mode = GameMode.Development;

    public bool IsSdkTestingEnvironment = true;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        
    }
    public void Start()
    {
        if (Game_Mode == GameMode.Production)
        {
            IsSdkTestingEnvironment = false;
        }
    }
}
