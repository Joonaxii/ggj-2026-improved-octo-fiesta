using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BloodDropUI : MonoBehaviour
{
    [SerializeField] private Image _bloodFill;
    [SerializeField] private TextMeshProUGUI _partyGoerCount;

    public void InitializeValues(int newPartyGoersCount = 0)
    {
        _bloodFill.fillAmount = 0;
        UpdateAlivePartyGoers(newPartyGoersCount);
    }
    
    // When player kills party goers
    public void UpdateAlivePartyGoers(int newCount)
    {
        _partyGoerCount.text = newCount.ToString();
    }

    public void UpdateBloodFill(float value)
    {
        // 0 -> 1
        _bloodFill.fillAmount = value;
    } 
}
