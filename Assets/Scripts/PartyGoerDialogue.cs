using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PartyGoerDialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _chatText;
    [SerializeField] private Vector2 _chatterCooldownRange;
    [SerializeField] private float _showTime = 1;
    
    [SerializeField] private List<string> _socializingComments = new List<string>();
    [SerializeField] private List<string> _bathroomComments = new List<string>();
    [SerializeField] private List<string> _drinkComments = new List<string>();
    
    [SerializeField] private List<string> _chatter = new List<string>();

    private float _currentCooldown = 0;
    private float _cooldownTimer = 0;
    private float _showTimer = 0; 
    private bool _chatterMode = false;
    private bool _showing = false;

    public void ShowSocializingComment() => UpdateText(_socializingComments[Random.Range(0, _socializingComments.Count)]);
    public void ShowBathroomComment() => UpdateText(_bathroomComments[Random.Range(0, _bathroomComments.Count)]);
    public void ShowDrinkComment() => UpdateText(_drinkComments[Random.Range(0, _drinkComments.Count)]);

    private void UpdateText(string newText)
    {
        _chatText.text = newText;
        _showTimer = 0;
        _showing = true;
    }

    public void ChatterModeToggle(bool newState)
    {
        _chatterMode = newState;
        _showing = false;
        _chatText.text = "";
        
        _showTimer = 0;
        _cooldownTimer = 0;
        _currentCooldown = Random.Range(_chatterCooldownRange.x, _chatterCooldownRange.y);
    } 
    
    private void Update()
    {
        if (_chatterMode)
        {
            UpdateChatterMode();
            return;
        }
        UpdateRegularText();
    }

    private void UpdateChatterMode()
    {
        if (!_showing)
        {
            _cooldownTimer += Time.deltaTime;

            if (_cooldownTimer < _currentCooldown)
            {
                return;
            }

            _chatText.text = _chatter[Random.Range(0, _chatter.Count)];
            _showing = true;
            _showTimer = 0;
        }
        
        if (_showing)
        {
            _showTimer += Time.deltaTime;
            
            if (_showTimer < _showTime)
            {
                return;
            }
            
            _chatText.text = "";
            _showing = false;
            _cooldownTimer = 0;
            _currentCooldown = Random.Range(_chatterCooldownRange.x, _chatterCooldownRange.y);
        }
    }

    private void UpdateRegularText()
    {
        if (_showing)
        {
            _showTimer += Time.deltaTime;

            if (_showTimer < _showTime)
            {
                return;
            }
            
            _chatText.text = "";
            _showing = false;
        }
    }
}
