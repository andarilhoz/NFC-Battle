using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if ( !Application.isPlaying )
            {
                return null;
            }

            if ( instance != null )
            {
                return instance;
            }

            if ( instance == null )
            {
                var newObject = new GameObject("[GameManager]");
                DontDestroyOnLoad(newObject);
                instance = newObject.AddComponent<GameManager>();
                instance.Initialize();
            }

            return instance;
        }
    }

    #endregion

    private void Awake()
    {
        if ( instance == null ) instance = this;
    }

    public void Initialize()
    {
        
    }

    public void EditAvatar()
    {
    }

    public void AddCreature()
    {
        SceneManager.LoadSceneAsync("AddCreatureScene");
    }
}
