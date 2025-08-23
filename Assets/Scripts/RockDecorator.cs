using System.Collections.Generic;
using UnityEngine;
using BooterAndBigARM.World;

namespace BooterAndBigARM.World.Decor
{
    public class RockDecorator : MonoBehaviour, IChunkDecorator
    {
        [Header("References")]
        public WorldConfig config;          // drag your WorldConfig asset here
        public GameObject rockPrefab;       // any simple mesh prefab with MeshRenderer

        public GameObject rockPrefab2;      // optional second rock prefab for variety

        [Header("Rules")]
        [Min(0)] public int rocksPerChunk = 20;
        [Min(0f)] public float minSpacing = 2f;   // meters between rocks
        public float yOffset = 0f;                // lift/sink a bit if needed

        [Header("Uniform Settings")]
        public Vector2 uniformScaleRange = new Vector2(0.8f, 1.4f);
        public Vector2 uniformRotationRange = new Vector2(0f, 360f);

        [Header("Non-Uniform Settings")]
        public bool useNonUniformScale = false;
        public Vector2 scaleXRange = new Vector2(0.8f, 1.4f);
        public Vector2 scaleYRange = new Vector2(0.8f, 1.4f);
        public Vector2 scaleZRange = new Vector2(0.8f, 1.4f);

        public bool useNonUniformRotation = false;
        public Vector2 randomYRotationRange = new Vector2(0f, 360f);
        public Vector2 randomXRotationRange = new Vector2(0f, 0f);
        public Vector2 randomZRotationRange = new Vector2(0f, 0f);

        public void Decorate(Chunk chunk)
        {
            // Debug.Log($"RockDecorator.Decorate called for chunk at {chunk.Coord}");

            if (!config)
            {
                // Debug.LogWarning("RockDecorator missing WorldConfig reference.");
                return;
            }
            if (!rockPrefab)
            {
                // Debug.LogWarning("RockDecorator missing rockPrefab reference.");
                return;
            }
            if (chunk == null)
            {
                // Debug.LogWarning("RockDecorator received null chunk.");
                return;
            }
            if (chunk.Data == null)
            {
                // Debug.LogWarning("RockDecorator received chunk with null Data.");
                return;
            }

            // chunk placement (already done inside Chunk.Initialize) gives us origin on XZ plane
            Vector3 origin = chunk.transform.position;
            int size = chunk.Data.size;         // tiles per side
            float tileSize = 1f;                // your mesh currently uses 1 unit per tile 
            float worldSize = size * tileSize;  // meters

            // deterministic seed from (worldSeed, chunkCoord)
            int seed = config.generation.worldSeed
                       ^ (chunk.Coord.x * 73856093)
                       ^ (chunk.Coord.y * 19349663);        // stable across runs 
            var rng = new System.Random(seed == 0 ? 1 : seed);

            // simple spacing (reject points that are too close)
            var placed = new List<Vector2>(rocksPerChunk);

            for (int i = 0; i < rocksPerChunk; i++)
            {
                // try a few times to find a spaced spot
                for (int tries = 0; tries < 8; tries++)
                {
                    float x = (float)rng.NextDouble() * worldSize;
                    float z = (float)rng.NextDouble() * worldSize;
                    var p = new Vector2(x, z);

                    bool ok = true;
                    for (int k = 0; k < placed.Count; k++)
                    {
                        if (Vector2.Distance(p, placed[k]) < minSpacing) { ok = false; break; }
                    }
                    if (!ok) continue;

                    placed.Add(p);

                    Vector3 posWS = origin + new Vector3(p.x, 0f, p.y) + Vector3.up * yOffset;

                    float randomYRotation;
                    float randomXRotation;
                    float randomZRotation;

                    if (!useNonUniformRotation)
                    {
                        float uniformRot = Mathf.Lerp(uniformRotationRange.x, uniformRotationRange.y, (float)rng.NextDouble());
                        randomXRotation = uniformRot;
                        randomYRotation = uniformRot;
                        randomZRotation = uniformRot;
                    }
                    else
                    {
                        randomYRotation = Mathf.Lerp(randomYRotationRange.x, randomYRotationRange.y, (float)rng.NextDouble());
                        randomXRotation = Mathf.Lerp(randomXRotationRange.x, randomXRotationRange.y, (float)rng.NextDouble());
                        randomZRotation = Mathf.Lerp(randomZRotationRange.x, randomZRotationRange.y, (float)rng.NextDouble());
                    }
                    Quaternion randomRotation = Quaternion.Euler(randomXRotation, randomYRotation, randomZRotation);

                    // Randomly choose between rockPrefab and rockPrefab2 if assigned
                    GameObject prefabToUse = rockPrefab;
                    if (rockPrefab2 != null && rng.NextDouble() < 0.5)
                    {
                        prefabToUse = rockPrefab2;
                    }

                    var go = Instantiate(prefabToUse, posWS, randomRotation, chunk.transform);

                    float s = Mathf.Lerp(uniformScaleRange.x, uniformScaleRange.y, (float)rng.NextDouble());

                    if (useNonUniformScale)
                    {
                        float sx = Mathf.Lerp(scaleXRange.x, scaleXRange.y, (float)rng.NextDouble());
                        float sy = Mathf.Lerp(scaleYRange.x, scaleYRange.y, (float)rng.NextDouble());
                        float sz = Mathf.Lerp(scaleZRange.x, scaleZRange.y, (float)rng.NextDouble());
                        go.transform.localScale = new Vector3(sx, sy, sz);
                    }
                    else
                    {
                        go.transform.localScale = Vector3.one * s;
                    }

                    // help URP batching a bit
                    var mr = go.GetComponentInChildren<MeshRenderer>();
                    if (mr && config.rendering.gpuInstancingForProps)
                        mr.sharedMaterial.enableInstancing = true;

                    break; // next rock
                }
            }
        }
    }
}