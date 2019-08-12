using System.Collections.Generic;
using UnityEngine;

    public class Location
    {
        public Location(int x, int y)
        {
            i = x; j = y;
        }
        public int i;
        public int j;
    }

public static class DrawerUtility
{
    public static List<Vector3> GetNodesPath(Vector2 start, Vector2 end, float z, float size)
    {
        var r = new List<Vector3>();
        var delta = end - start;
        var length = delta.magnitude;
        var dir = delta.normalized;

        for (int i = 0; i * size < length; i++)
        {
            var nextPos = start + dir * size * i;
            var pos = ScreenToCanvas(nextPos, z);
            r.Add(pos);
        }
        return r;
    }

    public static List<Location> GetNodesPath(Vector2 start, Vector2 end, float z, float startx, float starty, float size, int range)
    {
        var r = new List<Location>();
        var delta = end - start;
        var length = delta.magnitude;
        var dir = delta.normalized;
        var lastx = -1;
        var lasty = -1;
        for (int i = 0; i * size < length; i++)
        {
            var nextPos = start + dir * size * i;
            int x, y;
            ScreenToCanvas(nextPos, z, startx, starty, size, out x, out y, range);
            if (lastx == x && lasty == y)
            {
                continue;
            }
            lastx = x;
            lasty = y;
            r.Add(new Location(x, y));
        }
        return r;
    }

    public static Vector3 ScreenToCanvas(Vector3 mousePos, float z)
    {
        var screenPos = new Vector3(mousePos.x, mousePos.y, z - Camera.main.transform.position.z);
        var pos = Camera.main.ScreenToWorldPoint(screenPos);
        return pos;
    }


    public static void ScreenToCanvas(Vector3 mousePos, float z, float startx, float starty, float size, out int i, out int j, int range)
    {
        var screenPos = new Vector3(mousePos.x, mousePos.y, z - Camera.main.transform.position.z);
        var pos = Camera.main.ScreenToWorldPoint(screenPos);
        PosToCanvas(pos, startx, starty, size, out i, out j, range);
    }

    public static void PosToCanvas(Vector2 pos, float startx, float starty, float size, out int i, out int j, int range)
    {
        i = Mathf.FloorToInt((pos.x - startx) / size);
        i = Mathf.Max(0, i);
        i = Mathf.Min(range - 1, i);
        j = Mathf.FloorToInt((pos.y - starty) / size);
        j = Mathf.Max(0, j);
        j = Mathf.Min(range - 1, j);
    }

    private static Vector2 GetUV(Vector3 pos, float minx, float maxx, float miny, float maxy)
    {
        return new Vector2((pos.x - minx) / (maxx - minx), (pos.y - miny) / (maxy - miny));
    }

    private static Vector3 GetCenter(List<Vector3> verts, out float minx, out float maxx, out float miny, out float maxy)
    {
        maxx = float.MinValue;
        minx = float.MaxValue;
        maxy = float.MinValue;
        miny = float.MaxValue;
 
        if(verts.Count < 1)
        {
            return Vector3.zero;
        }
        float maxz = float.MinValue;
        float minz = float.MaxValue;
        for(int i = 0;i < verts.Count; i++)
        {
            var vert = verts[i];
            if(vert.x > maxx)
            {
                maxx = vert.x;
            }
            if(vert.x < minx)
            {
                minx = vert.x;
            }
            if(vert.y > maxy)
            {
                maxy = vert.y;
            }
            if(vert.y < miny)
            {
                miny = vert.y;
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
        
        return new Vector3((minx + maxx) / 2, (miny + maxy) / 2, (minz + maxz) / 2);
    }

    static bool IsClockWise(List<Vector3> verts)
    {
        if(verts.Count < 2)
        {
            return true;
        }
        var r = 0f;
        for(int i = 0; i < verts.Count - 1; i++)
        {
            r += verts[i].x * verts[i + 1].y;
            r -= verts[i].y * verts[i + 1].x;
        }
        r += verts[verts.Count - 1].x * verts[0].y;
        r -= verts[verts.Count - 1].y * verts[0].x;
        var isClockWise = r < 0;
        return isClockWise;
    }

    public static void OptimizedGenerateShape(Mesh mesh, List<Vector3> verts)
    {
        if(verts.Count < 2)
        {
            return;
        }
        float minx, miny, maxx, maxy;
        GetCenter(verts, out minx, out maxx, out miny, out maxy);
        var v = new Vector2[verts.Count];
        var uv = new List<Vector2>();
        int[] indices;
        if(IsClockWise(verts))
        {
            for(var i = 0; i < verts.Count; i++)
            {
                v[i] = new Vector2(verts[i].x, verts[i].y);
                uv.Add(GetUV(verts[i], minx, maxx, miny, maxy));
            }
        }
        else
        {
            for(var i = 0; i < verts.Count; i++)
            {
                var index = verts.Count - 1 - i;
                v[i] = new Vector2(verts[index].x, verts[index].y);
                uv.Add(GetUV(verts[index], minx, maxx, miny, maxy));
            }
        }
        Triangulator.Triangulator.Triangulate(v, Triangulator.WindingOrder.Clockwise, out v, out indices);
        //v2 to v3
        for(var i = 0; i < v.Length; i++)
        {
            var tmp = verts[i];
            tmp.x = v[i].x;
            tmp.y = v[i].y;
            verts[i] = tmp;
        }

        mesh.Clear();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uv);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);
    }
}
