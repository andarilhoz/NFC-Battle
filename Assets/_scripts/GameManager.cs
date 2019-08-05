using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
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
        if ( instance == null )
        {
            instance = this;
            Initialize();
        }
    }

    public async void Initialize()
    {
        await EnableFirebase();
        EnableGooglePlayService();
        LoginUsingGooglePlay();
    }

    public void EditAvatar()
    {
    }

    public void AddCreature()
    {
        SceneManager.LoadSceneAsync("AddCreatureScene");
    }

    private async Task<bool> EnableFirebase()
    {
        Debug.Log("Habilitando Firebase");
        var dependencyStatus  = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if ( dependencyStatus == Firebase.DependencyStatus.Available )
        {
            // Create and hold a reference to your FirebaseApp,
            // where app is a Firebase.FirebaseApp property of your application class.
            //   app = Firebase.FirebaseApp.DefaultInstance;

            // Set a flag here to indicate whether Firebase is ready to use by your app.
            Debug.Log("Firebase Habilitado");
            return true;
        }

        Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
            return false;
        
    }

    private void EnableGooglePlayService()
    {
        Debug.Log("Habilitando Google Play");
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .RequestServerAuthCode(false /* Don't force refresh */)
            .RequestEmail()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Debug.Log("Google Play Habilitado");
    }

    private void LoginUsingGooglePlay()
    {
        Debug.Log("Realizando login na Google Play");
        PlayGamesPlatform.Instance.Authenticate((error, message) =>
        {
            Debug.Log($"error: {error} message: {message}");
        });
        /*Social.localUser.Authenticate((bool success) => {
            // handle success or failure
            if ( !success )
            {
                Debug.Log("Erro ao fazer o login");
                return;
            }
            Debug.Log("Login realizado com sucesso!");
            var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            LoginInFirebase(authCode);
        });*/
    }

    private void LoginInFirebase(string authCode)
    {
        Debug.Log("Realizando Login no Firebase");
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential =
            Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
}
