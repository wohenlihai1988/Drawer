using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushDrawer: MonoBehaviour
{
    List<Vector3> m_dots = new List<Vector3>();
    public GameObject m_brush;
    List<GameObject> m_brushInstance = new List<GameObject>();
    bool m_bstart;
    Vector3 m_lastPos;
    public float m_threshold = 1;
    public float m_fixedz = 20;
    public GameObject m_shape;
    private MeshFilter m_meshFilter;
    private List<Vector3> m_pathNodes = new List<Vector3>();
    private int m_lastTouchCount;
    public Camera m_camera;

    protected virtual void Start()
    {
        m_meshFilter = m_shape.GetComponent<MeshFilter>();
        m_meshFilter.sharedMesh = new Mesh();
    }

    bool InputDown()
    {
        var r = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && m_lastTouchCount == 0);
        return r;
    }

    bool InputUp()
    {
#if UNITY_EDITOR 
        return Input.GetMouseButtonUp(0);
#else
        var r = Input.touchCount < 1 && m_lastTouchCount > 0;
        return r;
#endif
    }

    Vector3 InputPos()
    {
        if(Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }

    bool CancelClick()
    {
        return Input.GetMouseButtonDown(1) || Input.touchCount > 1;
    }

    void Compose(ref List<Vector3> nodes)
    {
        Debug.LogFormat("node count : {0}", nodes.Count);
        int maxCount = 128;
        var step = nodes.Count / maxCount;
        if(nodes.Count <= maxCount)
        {
            return;
        }
        var tmp = new List<Vector3>();
        tmp.Clear();
        for(int i =0; i < nodes.Count; i += step)
        {
            tmp.Add(nodes[i]);
        }
        if( nodes.Count % step != 0)
        {
            tmp.Add(nodes[nodes.Count - 1]);
        }
        nodes = tmp;
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if(InputDown())
        {
            Clear();
            m_bstart = true;
        }
        if (InputUp())
        {
            OnEnd();
        }
        if(CancelClick())
        {
            Clear();
        }
        if(m_bstart)
        {
            if((Input.mousePosition - m_lastPos).magnitude < m_threshold)
            {
                return; 
            }
            m_dots.Add(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_fixedz));
            UpdateCanvas();
            m_lastPos = Input.mousePosition;
        }
        m_lastTouchCount = Input.touchCount;
	}

    protected virtual void OnEnd()
    {
            Compose(ref m_pathNodes);
            DrawerUtility.OptimizedGenerateShape(m_meshFilter.sharedMesh, m_pathNodes);
            m_pathNodes.Clear();
            m_bstart = false;
    }

    virtual protected void Clear()
    {
        ClearData();
        ResetCamera();
    }

    protected void ClearData()
    {
        if(null != m_meshFilter.sharedMesh)
        {
            m_meshFilter.sharedMesh.Clear();
        }
        m_lastPos = Vector3.zero;
        m_dots.Clear();
        foreach(var go in m_brushInstance)
        {
            go.SetActive(false);
        }
    }

    protected void ResetCamera()
    {
        m_camera.clearFlags = CameraClearFlags.Color;
        m_camera.backgroundColor = Color.white;
        StartCoroutine(CoroutineClear());
    }

    IEnumerator CoroutineClear()
    {
        yield return null;
        m_camera.clearFlags = CameraClearFlags.Nothing;
    }

    void UpdateCanvas()
    {
        if(null == m_brush)
        {
            return;
        }
        if (m_dots.Count < 2)
        {
            return;
        }
        var path = DrawerUtility.GetNodesPath(m_dots[m_dots.Count - 2], m_dots[m_dots.Count - 1], m_fixedz, m_threshold);
        if(m_pathNodes.Count < 1)
        {
            m_pathNodes.Add(path[0]);
        }
        m_pathNodes.Add(path[path.Count - 1]);
        for(int i = 0; i < m_brushInstance.Count; i++)
        {
            m_brushInstance[i].SetActive(false);
        }
        for(var i = 0; i < path.Count; i++)
        {
            if(i > m_brushInstance.Count - 1)
            {
                m_brushInstance.Add(Instantiate(m_brush));
            }
            m_brushInstance[i].SetActive(true);
            m_brushInstance[i].transform.position = path[i];
        }
    }
}
