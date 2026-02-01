using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBG : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _sprites;
    [SerializeField, Range(0, 1)] private float _alpha = 0.75f;

    public void SetColor(Color color)
    {
        color.a *= _alpha;
        for (int i = 0; i < _sprites.Length; i++)
        {
            _sprites[i].color = color;
        }
    }
}
