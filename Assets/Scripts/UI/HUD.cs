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
}
