using UnityEngine;

public class TextSwayUI : MonoBehaviour
{
    private float _swayOffset;
    private RectTransform _rectTransform;
    private float _initialZRotation;

    private float _swayLimitter = 0.05f;

    void Awake()
    {
        _swayOffset = Random.Range(-10, 10);
        _rectTransform = GetComponent<RectTransform>();
        _initialZRotation = _rectTransform.localRotation.z;
    }
    
    
    void Update()
    {
        var zVal = Mathf.Sin((Time.fixedTime * 2) + _swayOffset);
        var newRotation = _rectTransform.rotation;
        newRotation.z = (zVal * _swayLimitter) + _initialZRotation;
        _rectTransform.rotation = newRotation;
    }
}
