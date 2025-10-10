using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSync : MonoBehaviour
{
    public static DataSync instance;
    private DatabaseReference dbRef;
    private FirebaseDatabase firebaseDatabase;
    private const string basePath = "PlayerIDList";

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                firebaseDatabase = FirebaseDatabase.GetInstance(app, "https://clusterdubas-default-rtdb.europe-west1.firebasedatabase.app/");
                dbRef = firebaseDatabase.RootReference;
                SetPlayerMood("player", 1);

                Debug.Log("Firebase initialisé avec succès !");
            }
            else
            {
                Debug.LogError("Firebase non disponible : " + task.Result);
            }
        });

    }


    public void SetPlayerMood(string playerId, int mood)
    {
        print("Task called");
        dbRef.Child("joueurs").Child("mood").SetValueAsync(mood).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log($"Mood de {playerId} mis à jour à {mood}");
            else
                Debug.LogError($"Échec MAJ mood de {playerId} : " + task.Exception?.Message);
        });
    }
}