using System.Collections.Generic;
using UnityEngine;

public class LineDrawer : MonoBehaviour {

    Mesh m_mesh;
    List<Vector3> m_verts = new List<Vector3>();
    Vector3 m_lastPos;
    GameObject m_bursh;
    Material m_material;
    bool m_start;
    public float m_threshold = 5f;

    float m_maxx;
    float m_minx;
    float m_miny;
    float m_maxy;
    
	// Use this for initialization
	void Start ()
    {
        m_mesh = new Mesh();
        UpdateMesh();
        Application.targetFrameRate = 10;
	}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_verts.Clear();
            m_start = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            m_start = false;
            DrawerUtility.OptimizedGenerateShape(m_mesh, m_verts);
        }
        if(!m_start)
        {
            return;
        }
        if (Input.mousePosition == m_lastPos)
        {
            return;
        }
        if((Input.mousePosition - m_lastPos).magnitude < m_threshold)
        {
            return;
        }
        m_lastPos = Input.mousePosition;
        var vertPos = ConvertSceenPosToWorld();
        m_verts.Add(vertPos);
        UpdateMesh();
    }

    Vector3 ConvertSceenPosToWorld()
    {
        var screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        return Camera.main.ScreenToWorldPoint(screenPos);
    }

    void DrawLines()
    {
        var array = new int[(m_verts.Count - 1) * 2];
        for (int i = 1; i < array.Length + 1; i += 2)
        {
            array[i - 1] = (i) / 2;
            array[i] = (i + 1) / 2;
        }
        m_mesh.Clear(false);
        m_mesh.SetVertices(m_verts);

        m_mesh.SetIndices(array, MeshTopology.Lines, 0);
    }

    void DrawTriangleLines()
    {
        
    }

    void UpdateMesh()
    {
        if(m_verts.Count > 1)
        {
            if(null != m_material)
            {
                m_material.color = Color.white;
            }
            DrawLines();
       }
        else
        {
            if(null != m_material)
            {
                m_material.color = Color.clear;
            }
        }
        //Graphics.DrawMesh(m_mesh, Vector3.zero, Quaternion.identity, mat, 0);
        if (null == m_bursh)
        {
            m_material = Resources.Load("Paper") as Material;
            m_bursh = new GameObject();
            var mf = m_bursh.AddComponent<MeshFilter>();
            var renderer = m_bursh.AddComponent<MeshRenderer>();
            mf.sharedMesh = m_mesh;
            renderer.sharedMaterial = m_material;
        }
   }
}
