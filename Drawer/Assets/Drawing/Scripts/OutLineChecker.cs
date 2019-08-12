using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLineChecker : MonoBehaviour
{
    class Bound
    {
        public int Mid;
        public int HalfRange;
    }

    class CanvasNode
    {
        public void Show()
        {

        }

        public void Hide()
        {

        }
    }

    private Vector2 m_start;
    private Vector2 m_size;
    private List<Bound> m_xbounds;
    private List<Bound> m_ybounds;
    private int m_CanvasWidth;
    private int m_CanvasHeight;
    private List<List<CanvasNode>> m_canvasNodes = new List<List<CanvasNode>>();

    public void Int()
    {

    }

    public void SetPoints(List<Vector3> points)
    {
        
    }

    private void PointToArea(float x, float y, out int i, out int j)
    {
        i = (int)((x - m_start.x) / m_size.x);
        j = (int)((y - m_start.y) / m_size.y);
    }

    private void DrawBounds(List<Bound> xbounds, List<Bound> ybounds)
    {
        for(int i = 0; i < m_CanvasWidth; i++)
        {
            for(int j = 0; j < m_CanvasHeight; j++)
            {
                if(GetOffset(i, xbounds[j]) <= 0 && GetOffset(j, ybounds[i]) < 0)
                {
                    m_canvasNodes[i][j].Show();
                }
                else
                {
                    m_canvasNodes[i][j].Hide();
                }
            }
        }
    }
    
    private void CheckBoundOffset(int i, int j)
    {
        var xoffset = GetOffset(i, m_xbounds[j]);
        var yoffset = GetOffset(j, m_ybounds[i]);
    }

    private int GetOffset(int val, Bound bound)
    {
        var offset = Mathf.Abs((val - bound.Mid)) - bound.HalfRange;
        return offset;
    }
}
