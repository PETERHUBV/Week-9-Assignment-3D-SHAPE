using JetBrains.Annotations;
using UnityEngine;

public class THREEDSHAPEDRAW : MonoBehaviour
{
    public Material material;
    public float size = 1f;
    public int segments = 12; //cylinder/sphere 
    public float height = 2f;

    private void OnPostRender()
    {
        if (material == null)
        {
            Debug.LogError("You need to assign a material.");
            return;
        }

        GL.PushMatrix();
        material.SetPass(0);
        GL.Begin(GL.LINES);

      
        DrawPyramid(Vector3.left * 4, size);
        DrawRectangularColumn(Vector3.zero, size, height);
        DrawCylinder(Vector3.right * 4, size * 0.5f, height, segments);
        DrawSphere(Vector3.forward * 4, size * 0.75f, segments);

        GL.End();
        GL.PopMatrix();
    }

    
    // PYRAMID
    
    private void DrawPyramid(Vector3 center, float baseSize)
    {
        float half = baseSize * 0.5f;
        Vector3 top = center + Vector3.up * baseSize;
        Vector3[] baseVerts = new Vector3[4]
        {
            center + new Vector3(-half, 0, -half),
            center + new Vector3(half, 0, -half),
            center + new Vector3(half, 0, half),
            center + new Vector3(-half, 0, half)
        };

        // Base
        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(baseVerts[i]);
            GL.Vertex(baseVerts[(i + 1) % 4]);
        }

        // Sides
        foreach (var v in baseVerts)
        {
            GL.Vertex(v);
            GL.Vertex(top);
        }
    }

   
    // RECTANGULAR COLUMN 
  
    private void DrawRectangularColumn(Vector3 center, float width, float height)
    {
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        Vector3[] bottom = new Vector3[4]
        {
            center + new Vector3(-halfW, -halfH, -halfW),
            center + new Vector3(halfW, -halfH, -halfW),
            center + new Vector3(halfW, -halfH, halfW),
            center + new Vector3(-halfW, -halfH, halfW)
        };

        Vector3[] top = new Vector3[4]
        {
            center + new Vector3(-halfW, halfH, -halfW),
            center + new Vector3(halfW, halfH, -halfW),
            center + new Vector3(halfW, halfH, halfW),
            center + new Vector3(-halfW, halfH, halfW)
        };

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(bottom[i]);
            GL.Vertex(bottom[(i + 1) % 4]);

            GL.Vertex(top[i]);
            GL.Vertex(top[(i + 1) % 4]);

            GL.Vertex(bottom[i]);
            GL.Vertex(top[i]);
        }
    }

   
    // CYLINDER
 
    private void DrawCylinder(Vector3 center, float radius, float height, int segments)
    {
        float halfH = height * 0.5f;
        Vector3[] top = new Vector3[segments];
        Vector3[] bottom = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            bottom[i] = center + new Vector3(x, -halfH, z);
            top[i] = center + new Vector3(x, halfH, z);
        }

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            GL.Vertex(bottom[i]);
            GL.Vertex(bottom[next]);

            GL.Vertex(top[i]);
            GL.Vertex(top[next]);

            GL.Vertex(bottom[i]);
            GL.Vertex(top[i]);
        }
    }

   
    // SPHERE
    
    private void DrawSphere(Vector3 center, float radius, int segments)
    {
        int rings = segments;
        for (int i = 0; i <= rings; i++)
        {
            float lat = Mathf.PI * i / rings;
            float y = Mathf.Cos(lat) * radius;
            float ringR = Mathf.Sin(lat) * radius;

            for (int j = 0; j < segments; j++)
            {
                float lon = Mathf.PI * 2f * j / segments;
                float nextLon = Mathf.PI * 2f * (j + 1) / segments;

                Vector3 p1 = center + new Vector3(Mathf.Cos(lon) * ringR, y, Mathf.Sin(lon) * ringR);
                Vector3 p2 = center + new Vector3(Mathf.Cos(nextLon) * ringR, y, Mathf.Sin(nextLon) * ringR);
                GL.Vertex(p1);
                GL.Vertex(p2);
            }
        }
    }
}