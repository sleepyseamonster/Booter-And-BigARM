using UnityEngine;

namespace BooterAndBigARM.World
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "BABA/World Config")]
    public class WorldConfig : ScriptableObject
    {
        [System.Serializable]
        public struct ScaleSettings
        {
            [Min(2)] public int chunkSize;        // tiles per side (start with 16)
            [Min(0.1f)] public float tileSize;    // 1 unit = 1 foot
            public int VerticesPerSide => chunkSize + 1;
            public float ChunkWorldSize => chunkSize * tileSize;
        }

        [System.Serializable]
        public struct StreamingSettings
        {
            [Min(1)] public int viewRadius;       // chunks each direction (3 => 7x7)
            [Min(0)] public int colliderRings;    // 0 = center only
            [Min(0)] public int propRings;        // usually >= colliderRings
            public bool usePooling;
            [Min(8)] public int chunkPoolCapacity;
        }

        [System.Serializable]
        public struct GenerationSettings
        {
            public int worldSeed;
            [Range(0.001f, 1f)] public float elevationFreq;
            [Range(0f, 10f)] public float elevationScale; // feet
            [Range(0.001f, 1f)] public float biomeFreq;
            public ScriptableObject biomeConfig; // optional later
        }

        [System.Serializable]
        public struct SaveSettings
        {
            public bool saveDeltasOnly;
            [Min(4)] public int regionSize;       // chunks per region file
            public string saveFolder;             // under persistentDataPath
            public bool compress;
            [Min(0f)] public float autosaveInterval; // seconds, 0 = off
        }

        [System.Serializable]
        public struct PhysicsSettings
        {
            public bool addMeshColliderInInnerRing;
            public bool recalcNormalsOnHeightChange;
        }

        [System.Serializable]
        public struct RenderingSettings
        {
            public Material terrainMaterial;      // URP Lit/Unlit
            public bool srpBatcherExpected;
            public bool gpuInstancingForProps;
        }

        [Header("Scale")]
        public ScaleSettings scale = new() { chunkSize = 16, tileSize = 1f };

        [Header("Streaming")]
        public StreamingSettings streaming = new()
        {
            viewRadius = 3,
            colliderRings = 1,
            propRings = 2,
            usePooling = true,
            chunkPoolCapacity = 128
        };

        [Header("Generation")]
        public GenerationSettings generation = new()
        {
            worldSeed = 12345,
            elevationFreq = 0.06f,
            elevationScale = 0.3f,
            biomeFreq = 0.12f,
            biomeConfig = null
        };

        [Header("Saving")]
        public SaveSettings saving = new()
        {
            saveDeltasOnly = true,
            regionSize = 16,
            saveFolder = "WorldSaves",
            compress = true,
            autosaveInterval = 60f
        };

        [Header("Physics")]
        public PhysicsSettings physics = new()
        {
            addMeshColliderInInnerRing = true,
            recalcNormalsOnHeightChange = false
        };

        [Header("Rendering")]
        public RenderingSettings rendering = new()
        {
            terrainMaterial = null,
            srpBatcherExpected = true,
            gpuInstancingForProps = true
        };
    }
}
