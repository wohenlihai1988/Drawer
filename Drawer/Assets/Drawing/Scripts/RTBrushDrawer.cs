using UnityEngine;
using UnityEngine.UI;

public class RTBrushDrawer : BrushDrawer
{
    private RenderTexture m_renderTex;
    public RawImage m_rawImage;
    public MeshRenderer m_renderer;

    protected override void Start()
    {
        base.Start();
        Init();
    }

    private void Init()
    {
        m_rawImage.texture = CameraEffect.Instance.m_renderTexture;
        m_renderer.sharedMaterial.SetTexture("_MainTex", CameraEffect.Instance.m_renderTexture);
    }

    protected override void Clear()
    {
        ClearData();
    }
}
