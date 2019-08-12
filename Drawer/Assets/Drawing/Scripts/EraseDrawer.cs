using System.Collections.Generic;
using UnityEngine;

public class EraseDrawer : MonoBehaviour {
    List<List<GameObject>> m_canvas = new List<List<GameObject>>();
    private float m_startx;
    private float m_starty;
    private float m_scale;
    private Vector3 m_lastPos;
	// Use this for initialization
	void Start ()
    {
        var src = Resources.Load("Plane");
        for(int i = 0; i < 100; i++)
        {
            m_canvas.Add(new List<GameObject>());
            for(int j = 0; j < 100; j++)
            {
                var go = GameObject.Instantiate(src) as GameObject;
                m_canvas[i].Add(go);
                go.name = i + "-" + j;
                TransAndScaleGo(i, j, go);
            }
        }
	}

    private float m_fixedPosz = 10;
    private void TransAndScaleGo(int i , int j, GameObject go)
    {
        var camera = Camera.main;
        var delta = camera.transform.position.z - m_fixedPosz;
        var tan = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2);
        var height = Mathf.Abs(delta * tan * 2);
        m_scale = height / 100;
        m_startx = -m_scale * 50;
        m_starty = m_startx;
        var pos = new Vector3(i * m_scale + m_startx, j * m_scale + m_starty, m_fixedPosz);
        go.transform.position = pos;
        go.transform.localEulerAngles = new Vector3(-90, 0, 0);
        go.transform.localScale = Vector3.one * 0.1f * m_scale;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(!Input.GetMouseButton(0))
        {
            m_lastPos = Vector3.zero;
            return;
        }
        int i, j;
        if((Input.mousePosition - m_lastPos).magnitude < m_scale)
        {
            return;
        }
        if(m_lastPos == Vector3.zero)
        {
            DrawerUtility.ScreenToCanvas(Input.mousePosition, m_fixedPosz, m_startx, m_starty, m_scale, out i, out j);
            m_canvas[i][j].SetActive(false);
        }
        else
        {
            var list = DrawerUtility.GetNodesPath(m_lastPos, Input.mousePosition, m_fixedPosz, m_startx, m_starty, m_scale);
            foreach(var item in list)
            {
                m_canvas[item.i][item.j].SetActive(false);
            }
        }
        m_lastPos = Input.mousePosition;
    }
}
