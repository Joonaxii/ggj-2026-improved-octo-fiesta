using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private BloodDropUI _bloodDrop;
    [SerializeField] private SuspicionUI _suspicicion;
    [SerializeField] private TimerUI _timer;

    public void Open()
    {
        // Initialize/reset values for elements here
        _timer.ResetVisuals();
        _suspicicion.ResetVisuals();
        _bloodDrop.InitializeValues();
    }

    public void UpdateBloodDrop(int alive, int total)
    {
        _bloodDrop.UpdateAlivePartyGoers(alive);
        _bloodDrop.UpdateBloodFill(1.0f - alive / (float)total);
    }

    public void UpdateTimer(float time)
    {
        _timer.UpdateTimeWheelRotation(time);
    }

    public void UpdateScore(int score)
    {

    }
}
