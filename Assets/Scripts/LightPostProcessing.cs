using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class LightPostProcessing : MonoBehaviour
{
    public RenderTexture Texture
    {
        get => _texture;
        set => _texture = value;
    }

    private Camera _camera;
    private RenderTexture _texture;
    [SerializeField] private Material _material;
    [SerializeField, Range(0, 1)] private float _lightStrength = 1.0f;
    [SerializeField] private Color _mainColor = new Color(0.75f, 0.75f, 0.75f, 1.0f);

    private void OnEnable()
    {
        _camera = gameObject.GetComponent<Camera>();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(_material == null || _camera == null) 
        {
            Graphics.Blit(source, destination);
            return; 
        }

        _material.SetTexture("_LightTex", _texture);
        _material.SetFloat("_LightStr", _lightStrength);
        _material.SetColor("_DarkColor", _mainColor);
        Graphics.Blit(source, destination, _material);
    }

}
