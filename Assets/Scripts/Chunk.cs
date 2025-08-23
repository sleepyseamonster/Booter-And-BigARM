using UnityEngine;

namespace BooterAndBigARM.World
{
    public class Chunk : MonoBehaviour
    {
        public Vector2Int Coord { get; private set; }
        public ChunkData Data { get; private set; }

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        // Called by ChunkManager after instantiation
        public void Initialize(Vector2Int coord, ChunkData data)
        {
            Coord = coord;
            Data = data;
            name = $"Chunk_{coord.x}_{coord.y}";

            // Position chunk in X/Z plane (ground)
            transform.position = new Vector3(coord.x * data.size, 0, coord.y * data.size);

            // Ensure Mesh components exist
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshFilter == null) meshFilter = gameObject.AddComponent<MeshFilter>();
            if (meshRenderer == null) meshRenderer = gameObject.AddComponent<MeshRenderer>();

            GenerateVisual();
        }

        private void GenerateVisual()
        {
            // Build a flat grid mesh for this chunk
            Mesh mesh = new Mesh();
        
            int size = Data.size;
            float tileSize = 1f; // 1 unit = 1 foot
        
            Vector3[] vertices = new Vector3[(size + 1) * (size + 1)];
            int[] triangles = new int[size * size * 6];
            Vector2[] uv = new Vector2[vertices.Length];
        
            // Generate vertices
            int v = 0;
            for (int z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    int clampedX = Mathf.Min(x, size - 1);
                    int clampedZ = Mathf.Min(z, size - 1);
                    float height = Data.tileHeights[clampedX, clampedZ];
                    vertices[v] = new Vector3(x * tileSize, height, z * tileSize);
                    uv[v] = new Vector2((float)x / size, (float)z / size);
                    v++;
                }
            }
        
            // Generate triangles
            int t = 0;
            int vert = 0;
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    triangles[t++] = vert;
                    triangles[t++] = vert + size + 1;
                    triangles[t++] = vert + 1;
        
                    triangles[t++] = vert + 1;
                    triangles[t++] = vert + size + 1;
                    triangles[t++] = vert + size + 2;
        
                    vert++;
                }
                vert++;
            }
        
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uv;
            mesh.RecalculateNormals();
        
            // Debug.Log($"Mesh generated with {vertices.Length} vertices and {triangles.Length / 3} triangles.");
            // Debug.Log($"MeshFilter assigned: {meshFilter != null}");
            // Debug.Log($"MeshRenderer assigned: {meshRenderer != null}");
            // Debug.Log($"MeshRenderer material: {(meshRenderer != null && meshRenderer.sharedMaterial != null ? meshRenderer.sharedMaterial.name : "None")}");
        
            meshFilter.mesh = mesh;
        }
    }
}
