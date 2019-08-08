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
            OptimizedGenerateShape();
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

    private Vector2 GetUV(Vector3 pos)
    {
        return new Vector2((pos.x - m_minx) / (m_maxx - m_minx), (pos.y - m_miny) / (m_maxy - m_miny));
    }

    Vector3 GetCenter()
    {
        if(m_verts.Count < 1)
        {
            return Vector3.zero;
        }
        m_maxx = float.MinValue;
        m_minx = float.MaxValue;
        m_maxy = float.MinValue;
        m_miny = float.MaxValue;
        float maxz = float.MinValue;
        float minz = float.MaxValue;
        for(int i = 0;i < m_verts.Count; i++)
        {
            var vert = m_verts[i];
            if(vert.x > m_maxx)
            {
                m_maxx = vert.x;
            }
            if(vert.x < m_minx)
            {
                m_minx = vert.x;
            }
            if(vert.y > m_maxy)
            {
                m_maxy = vert.y;
            }
            if(vert.y < m_miny)
            {
                m_miny = vert.y;
            }
            if(vert.z < minz)
            {
                minz = vert.z;
            }
            if(vert.z > maxz)
            {
                maxz = vert.z;
            }
        }
        
        return new Vector3((m_minx + m_maxx) / 2, (m_miny + m_maxy) / 2, (minz + maxz) / 2);
    }

    void OptimizedGenerateShape()
    {
        if(m_verts.Count < 2)
        {
            return;
        }
        GetCenter();
        var v = new Vector2[m_verts.Count];
        var uv = new List<Vector2>();
        int[] indices;
        if(IsClockWise())
        {
            for(var i = 0; i < m_verts.Count; i++)
            {
                v[i] = new Vector2(m_verts[i].x, m_verts[i].y);
                uv.Add(GetUV(m_verts[i]));
            }
        }
        else
        {
            for(var i = 0; i < m_verts.Count; i++)
            {
                var index = m_verts.Count - 1 - i;
                v[i] = new Vector2(m_verts[index].x, m_verts[index].y);
                uv.Add(GetUV(m_verts[index]));
            }
        }
        Triangulator.Triangulator.Triangulate(v, Triangulator.WindingOrder.Clockwise, out v, out indices);
        for(var i = 0; i < v.Length; i++)
        {
            var tmp = m_verts[i];
            tmp.x = v[i].x;
            tmp.y = v[i].y;
            m_verts[i] = tmp;
        }
        m_mesh.SetVertices(m_verts);
        var uvs = new List<Vector3>();
        m_mesh.SetUVs(0, uv);
        m_mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    }

    bool IsClockWise()
    {
        if(m_verts.Count < 2)
        {
            return true;
        }
        var r = 0f;
        for(int i = 0; i < m_verts.Count - 1; i++)
        {
            r += m_verts[i].x * m_verts[i + 1].y;
            r -= m_verts[i].y * m_verts[i + 1].x;
        }
        r += m_verts[m_verts.Count - 1].x * m_verts[0].y;
        r -= m_verts[m_verts.Count - 1].y * m_verts[0].x;
        var isClockWise = r < 0;
        return isClockWise;
    }

    void UpdateMesh()
    {
        if(m_verts.Count > 1)
        {
            if(null != m_material)
            {
                m_material.color = Color.white;
            }
            var array = new int[(m_verts.Count - 1) * 2];
            for(int i = 1; i < array.Length + 1; i += 2)
            {
                array[i - 1] = (i) / 2;
                array[i] = (i + 1) / 2;
            }
            m_mesh.Clear(false);
            m_mesh.SetVertices(m_verts);
            m_mesh.SetIndices(array, MeshTopology.Lines, 0);
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
