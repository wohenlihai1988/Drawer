using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffect : MonoBehaviour
{
    public static CameraEffect Instance;
    public RenderTexture m_renderTexture;
    public Material m_material;
    private bool m_set;
    private int m_interval = 100;
    private int m_currentFrame;
	// Use this for initialization
	void Awake () {
        Instance = this;
        m_renderTexture = new RenderTexture(Screen.width, Screen.height, 16);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material);
        m_currentFrame++;
        if(m_currentFrame % m_interval == 0)
        {
            Graphics.Blit(source, m_renderTexture, m_material);
        }
    }
}
