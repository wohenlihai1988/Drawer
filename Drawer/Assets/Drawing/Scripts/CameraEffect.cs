using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffect : MonoBehaviour
{
    public static CameraEffect Instance;
    public RenderTexture m_renderTexture;
    public Material m_material;

	// Use this for initialization
	void Awake ()
    {
        Instance = this;
        m_renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, m_renderTexture, m_material);
        Graphics.Blit(source, destination, m_material);
    }

    public void Clear()
    {
    }
}
