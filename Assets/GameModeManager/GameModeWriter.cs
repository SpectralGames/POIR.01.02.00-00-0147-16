using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeWriter : MonoBehaviour
{
    public const string Prefs_ID = "GameMode";
    public static event Action<GameMode> OnModeChange = null;

    [SerializeField] private GameMode _current = GameMode.Normal;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(Prefs_ID))
            _current = (GameMode)PlayerPrefs.GetInt(Prefs_ID);
        else
            PlayerPrefs.SetInt(Prefs_ID, (int)(_current));
    }

    public void SetMode(int gameMode)
    {
        SetMode((GameMode)gameMode);
    }

    public void SetMode(GameMode gameMode)
    {
        if (gameMode != _current && OnModeChange != null)
            OnModeChange(_current = gameMode);
        PlayerPrefs.SetInt(Prefs_ID, (int)(_current));
    }
}
