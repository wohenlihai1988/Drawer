using System.Collections.Generic;
using UnityEngine;

public class FitToScreen : MonoBehaviour
{
    public Camera m_camera;
    public GameObject m_plane;

    void Start()
    {
        var mesh = new Mesh();
        m_plane.GetComponent<MeshFilter>().sharedMesh = mesh;
        mesh.SetVertices(new List<Vector3>
        {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
        });
        mesh.SetIndices(new int[]
        {
            0, 1, 2,
            0, 2, 3,
        }, MeshTopology.Triangles, 0);
        mesh.SetUVs(0, new List<Vector2>
        {
            new Vector2(),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
        });

        var tmp = transform.position;
        tmp.x = m_camera.transform.position.x;
        tmp.y = m_camera.transform.position.y;
        transform.position = tmp;
    }

    void Update()
    {
        var distance = transform.position.z - m_camera.transform.position.z;
        var height = Mathf.Tan(m_camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * distance;
        var width = height * m_camera.aspect;
        m_plane.transform.localScale = new Vector3(width, height, 1);
    }
}
