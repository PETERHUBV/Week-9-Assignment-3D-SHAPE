using JetBrains.Annotations;
using UnityEngine;

public class LineGen : MonoBehaviour
{
    public Material material;
    public float cubeSize;
    public Vector2 cubePos;
    public float zPos;

    private void OnPostRender()
    {
        DrawLine();
    }

    public void DrawLine()
    {
        if (material == null)
        {
            Debug.LogError("You need to add a material");
            return;
        }
        GL.PushMatrix();
        material.SetPass(0);
        GL.Color(material.color);
        GL.Begin(GL.LINES);
        

        var frontSquare = GetCube();
        var frontZ = PerspectiveCamera.Instance.GetPerspective(zPos + cubeSize * .5f);
        var backSquare = GetCube();
        var backZ = PerspectiveCamera.Instance.GetPerspective(zPos - cubeSize * .5f);

        var computedFront = RenderSquare(frontSquare, frontZ);
        var computedBack = RenderSquare(backSquare, backZ);

        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(computedFront[i]);
            GL.Vertex(computedBack[i]);
        }

        // DRAW ADDITIONAL SHAPES

        // Pyramid (square base)
        DrawPyramid(new Vector3(3, 0, 0), 1f, 1.5f);

        // Cylinder (more than 5 segments)
        DrawCylinder(new Vector3(-3, 0, 0), 0.5f, 1.5f, 12);

        // Rectangular Column
        DrawRectangularColumn(new Vector3(0, 0, 3), 1f, 2f, 1f);

        // Sphere (more than 5 segments)
        DrawSphere(new Vector3(0, 0, -3), 0.75f, 12, 12);






        GL.End();
        GL.PopMatrix();
    }








    public Vector2[] GetCube()
    {
        var faceArray = new Vector2[]
        {
            new Vector2 (1, 1f),
            new Vector2 (-1f, 1f),
            new Vector2 (-1f, -1f),
            new Vector2 (1f, -1f),
        };

        for(var i = 0; i < faceArray.Length; i++)
        {
            faceArray[i] = new Vector2(cubePos.x + faceArray[i].x, cubePos.y + faceArray[i].y) * cubeSize;
        }

        return faceArray;
        
    }

    private Vector2[] RenderSquare(Vector2[] squareElements, float perspective)
    {
        var computedSquare = new Vector2[squareElements.Length];
        for(var i = 0; i < squareElements.Length; i++)
        {
            computedSquare[i] = squareElements[i] * perspective;
            GL.Vertex(squareElements[i] * perspective);
            GL.Vertex(squareElements[(i + 1) % squareElements.Length] * perspective);
        }
        return computedSquare;
    }


    // Pyramid
   
    private void DrawPyramid(Vector3 pos, float size, float height)
    {
        Vector3[] baseVerts = new Vector3[]
        {
            pos + new Vector3(-size, 0, -size),
            pos + new Vector3(size, 0, -size),
            pos + new Vector3(size, 0, size),
            pos + new Vector3(-size, 0, size)
        };
        Vector3 apex = pos + new Vector3(0, height, 0);

        // Base square
        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(baseVerts[i]);
            GL.Vertex(baseVerts[(i + 1) % 4]);
        }

        // Sides
        for (int i = 0; i < 4; i++)
        {
            GL.Vertex(baseVerts[i]);
            GL.Vertex(apex);
        }
    }

    
    // Cylinder
  
    private void DrawCylinder(Vector3 pos, float radius, float height, int segments)
    {
        float halfHeight = height / 2f;
        Vector3[] top = new Vector3[segments];
        Vector3[] bottom = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float angle = (2 * Mathf.PI / segments) * i;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            top[i] = pos + new Vector3(x, halfHeight, z);
            bottom[i] = pos + new Vector3(x, -halfHeight, z);

           // side lines
            GL.Vertex(top[i]);
            GL.Vertex(bottom[i]);

            //  top circle
            GL.Vertex(top[i]);
            GL.Vertex(top[(i + 1) % segments]);

            //  bottom circle
            GL.Vertex(bottom[i]);
            GL.Vertex(bottom[(i + 1) % segments]);
        }
    }

    
    // Rectangular Column

    private void DrawRectangularColumn(Vector3 pos, float width, float height, float depth)
    {
        Vector3 half = new Vector3(width / 2f, height / 2f, depth / 2f);
        Vector3[] verts = new Vector3[8];
        verts[0] = pos + new Vector3(-half.x, -half.y, -half.z);
        verts[1] = pos + new Vector3(half.x, -half.y, -half.z);
        verts[2] = pos + new Vector3(half.x, -half.y, half.z);
        verts[3] = pos + new Vector3(-half.x, -half.y, half.z);
        verts[4] = pos + new Vector3(-half.x, half.y, -half.z);
        verts[5] = pos + new Vector3(half.x, half.y, -half.z);
        verts[6] = pos + new Vector3(half.x, half.y, half.z);
        verts[7] = pos + new Vector3(-half.x, half.y, half.z);

        int[,] edges = new int[,]
        {
            {0,1},{1,2},{2,3},{3,0},
            {4,5},{5,6},{6,7},{7,4},
            {0,4},{1,5},{2,6},{3,7}
        };

        for (int i = 0; i < edges.GetLength(0); i++)
        {
            GL.Vertex(verts[edges[i, 0]]);
            GL.Vertex(verts[edges[i, 1]]);
        }
    }

    
    // Sphere

    private void DrawSphere(Vector3 pos, float radius, int segments, int rings)
    {
        for (int i = 0; i <= rings; i++)
        {
            float lat0 = Mathf.PI * (-0.5f + (float)(i - 1) / rings);
            float z0 = Mathf.Sin(lat0);
            float zr0 = Mathf.Cos(lat0);

            float lat1 = Mathf.PI * (-0.5f + (float)i / rings);
            float z1 = Mathf.Sin(lat1);
            float zr1 = Mathf.Cos(lat1);

            for (int j = 0; j <= segments; j++)
            {
                float lng = 2 * Mathf.PI * (float)(j - 1) / segments;
                float x = Mathf.Cos(lng);
                float y = Mathf.Sin(lng);

                Vector3 v0 = pos + new Vector3(x * zr0, z0, y * zr0) * radius;
                Vector3 v1 = pos + new Vector3(x * zr1, z1, y * zr1) * radius;

                GL.Vertex(v0);
                GL.Vertex(v1);
            }
        }
    }
}

