using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameModeReader : MonoBehaviour
{
    [SerializeField] private GameMode activeOnMode = GameMode.Normal;

    public UnityEvent OnCorrectModeIsActive = new UnityEvent();
    public UnityEvent OnCorrectModeIsNotActive = new UnityEvent();

    private void Start()
    {
        if (PlayerPrefs.HasKey(GameModeWriter.Prefs_ID))
            ValidateMode((GameMode)PlayerPrefs.GetInt(GameModeWriter.Prefs_ID));
        else
            ValidateMode(GameMode.Normal);

        GameModeWriter.OnModeChange += ValidateMode;
    }

    public void ValidateMode(GameMode mode)
    {
        if (activeOnMode == mode)
            OnCorrectModeIsActive.Invoke();
        else
            OnCorrectModeIsNotActive.Invoke();
    }

    private void OnDestroy()
    {
        GameModeWriter.OnModeChange -= ValidateMode;
    }
}
