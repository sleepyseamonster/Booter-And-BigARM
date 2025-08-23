using System;
using System.Collections.Generic;
using UnityEngine;

namespace BooterAndBigARM.World
{
    [Serializable]
    public class ChunkData
    {
        public Vector2Int chunkCoord;
        public int size;

        public int[,] tileHeights; // simple elevation map
        public List<EntityData> entities = new();

        public ChunkData(Vector2Int coord, int size)
        {
            chunkCoord = coord;
            this.size = size;
            tileHeights = new int[size, size];
        }
    }

    [Serializable]
    public class EntityData
    {
        public string prefabId;
        public Vector3 position;
        public Quaternion rotation;
    }
}
