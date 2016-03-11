using UnityEngine;

public static class SampleUtils
{
    // Compute a scale factor for a perfect fit between the level editor elements
    // and our unity scene to guarrantee the exact same layout on all devices,
    // whatever screen ratio or pixel density is used.
    public static float ComputeScaleFactor()
    {
        Camera camera = Camera.main;

        // the quad mesh we use takes one "camera size unit" at scale 1.0.
        // the size of an orthographic camera corresponds to half its height.
        float cameraPixelRatio = (float)camera.pixelWidth / (float)camera.pixelHeight;
        // we compute the width from the known height and the width / height ratio
        float cameraOrthoWidth = camera.orthographicSize * 2.0f * cameraPixelRatio;

        // we compute the scale factor so that a block will perfectly fit in the screen width
        return cameraOrthoWidth / SampleConstants.LEVEL_EDITOR_HORIZONTAL_CELL_COUNT;
    }

    // Create a quad mesh asset
    public static Mesh BuildQuadMesh(float width, float height)
    {
        Mesh mesh = new Mesh();

        // Setup vertices
        Vector3[] newVertices = new Vector3[4];
        float halfHeight = height * 0.5f;
        float halfWidth = width * 0.5f;
        newVertices[0] = new Vector3(-halfWidth, -halfHeight, 0);
        newVertices[1] = new Vector3(-halfWidth, halfHeight, 0);
        newVertices[2] = new Vector3(halfWidth, -halfHeight, 0);
        newVertices[3] = new Vector3(halfWidth, halfHeight, 0);

        // Setup UVs
        Vector2[] newUVs = new Vector2[newVertices.Length];
        newUVs[0] = new Vector2(0, 0);
        newUVs[1] = new Vector2(0, 1);
        newUVs[2] = new Vector2(1, 0);
        newUVs[3] = new Vector2(1, 1);

        // Setup triangles
        int[] newTriangles = new int[] { 0, 1, 2, 3, 2, 1 };

        // Setup normals
        Vector3[] newNormals = new Vector3[newVertices.Length];
        for (int i = 0; i < newNormals.Length; i++)
        {
            newNormals[i] = Vector3.forward;
        }

        // Create quad
        mesh.vertices = newVertices;
        mesh.uv = newUVs;
        mesh.triangles = newTriangles;
        mesh.normals = newNormals;

        // Recalculate bounds of the mesh, for the renderer's sake
        mesh.RecalculateBounds();

        return mesh;
    }

    // Achieve the movement of the pivot by moving the transform position in the specified direction
    // and then moving all vertices of the mesh in the opposite direction back to where they were in world-space
    //
    // Parameters:
    //        obj = game object to process (must contain a Mesh component)
    //        newPivotPoint =  new pivot value withint the range [-1..1], calculated from Mesh bounds
    public static void MoveQuadPivotPoint(GameObject obj, Vector2 newPivotPoint, bool adjustCollider)
    {
        // retrieve the mesh from the game object
        var meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
        Debug.Assert(meshFilter != null, "No MeshFilter found in the given object");

        var mesh = meshFilter.mesh;
        Debug.Assert(mesh != null, "No Mesh found in the given object");

        // Look at the object's transform position in comparison to the center of its mesh bounds and calculate the pivot values for xyz
        Bounds b = mesh.bounds;
        Vector3 offset = -1 * b.center;
        Vector2 oldPivotPoint = new Vector3(offset.x / b.extents.x, offset.y / b.extents.y);

        // Move object position by taking localScale into account
        Vector3 diff = Vector3.Scale(mesh.bounds.extents, new Vector3(oldPivotPoint.x - newPivotPoint.x, oldPivotPoint.y - newPivotPoint.y, 0.0f));
        obj.transform.position -= Vector3.Scale(diff, obj.transform.localScale);

        // Iterate over all vertices and move them in the opposite direction of the object position movement
        Vector3[] verts = mesh.vertices;
        for (int i = 0; i < verts.Length; i++)
        {
            verts[i] += diff;
        }
        mesh.vertices = verts; // Assign the vertex array back to the mesh
        mesh.RecalculateBounds(); // Recalculate bounds of the mesh, for the renderer's sake

        if (adjustCollider)
        {
            // The 'center' parameter of certain colliders needs to be adjusted when the transform position is modified
            var col = obj.GetComponent(typeof(Collider)) as Collider;
            if (col)
            {
                if (col is BoxCollider)
                {
                    ((BoxCollider)col).center += diff;
                }
                else if (col is CapsuleCollider)
                {
                    ((CapsuleCollider)col).center += diff;
                }
                else if (col is SphereCollider)
                {
                    ((SphereCollider)col).center += diff;
                }
            }

            var col2d = obj.GetComponent(typeof(Collider2D)) as Collider2D;
            if (col2d)
            {
                if (col2d is BoxCollider2D)
                {
                    ((BoxCollider2D)col2d).offset += new Vector2(diff.x, diff.y);
                }
            }
        }
    }
}
