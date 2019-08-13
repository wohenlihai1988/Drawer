using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RTBrushDrawer : BrushDrawer
{
    private RenderTexture m_renderTex;
    public RawImage m_rawImage;
    public Text m_text;
    public MeshRenderer m_renderer;
    public Material m_material;
    private CameraEffect m_cameraEffect;

    protected override void Start()
    {
        base.Start();
        m_cameraEffect = m_camera.gameObject.GetComponent<CameraEffect>();
        m_cameraEffect.enabled = true;
        Init();
    }

    private void Init()
    {
        m_material.SetTexture("_MaskTex", CameraEffect.Instance.m_renderTexture);
        m_renderer.sharedMaterial.SetTexture("_MainTex", CameraEffect.Instance.m_renderTexture);
        //m_rawImage.texture = CameraEffect.Instance.m_renderTexture;
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        m_cameraEffect.CheckGrade((grade) =>
        {
            m_text.text = grade.ToString();
        });
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Clear()
    {
        Debug.Log("RTBrushDrawer.Clear");
        ClearData();
        StartCoroutine(WaitAndClear());
    }

    private IEnumerator WaitAndClear()
    {
        m_renderer.enabled = false;
        yield return null;
        m_renderer.enabled = true;
    }
}
