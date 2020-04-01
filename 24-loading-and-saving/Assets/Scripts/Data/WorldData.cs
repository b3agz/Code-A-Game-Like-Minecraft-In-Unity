using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[HideInInspector]
[System.Serializable]
public class WorldData {

    public string worldName = "Prototype"; // Will be set by player eventually.
    public int seed;
    [System.NonSerialized]
    public Dictionary<Vector2Int, ChunkData> chunks = new Dictionary<Vector2Int, ChunkData>();

    [System.NonSerialized]
    public List<ChunkData> modifiedChunks = new List<ChunkData>();

    public WorldData (string _worldName, int _seed) {

        worldName = _worldName;
        seed = _seed;

    }

    public WorldData (WorldData wD) {

        worldName = wD.worldName;
        seed = wD.seed;

    }

    public ChunkData RequestChunk(Vector2Int coord, bool create) {

        ChunkData c;

        lock (World.Instance.ChunkListThreadLock) {

            if (chunks.ContainsKey(coord)) // If chunk is there, return it.
                c = chunks[coord];

            else if (!create) // If it's not and we haven't asked it to be created, return null.
                c = null;

            else { // If it's not and we asked it to be created, create the chunk then return it.
                LoadChunk(coord);
                c = chunks[coord];
            }

        }

        return c;
    }

    public void LoadChunk (Vector2Int coord) {

        // If the chunk is already loaded we don't need to do anything.
        if (chunks.ContainsKey(coord))
            return;

        // If not, we check if it is saved and if yes, get the data from there.
        ChunkData chunk = SaveSystem.LoadChunk(worldName, coord);
        if (chunk != null) {
            chunks.Add(coord, chunk);
            return;
        }

        // If not, add it to the list and populate it's voxels.
        chunks.Add(coord, new ChunkData(coord));
        chunks[coord].Populate();
       
    }

    public bool IsVoxelInWorld (Vector3 pos) {

        if (pos.x >= 0 && pos.x < VoxelData.WorldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.WorldSizeInVoxels)
            return true;
        else
            return false;

    }

    public void AddToModifiedChunkList(ChunkData chunk) {

        // Only add to list if ChunkData is not already in the list.
        if (!modifiedChunks.Contains(chunk))
            modifiedChunks.Add(chunk);

    }

    public void SetVoxel (Vector3 pos, byte value) {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (!IsVoxelInWorld(pos))
            return;

        // Find out the ChunkCoord value of our voxel's chunk.
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        // Then reverse that to get the position of the chunk.
        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        // Check if the chunk exists. If not, create it.
        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);
        
        // Then create a Vector3Int with the position of our voxel *within* the chunk.
        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));
        //Debug.Log(string.Format("{0}, {1}, {2}", voxel.x, voxel.y, voxel.z));
        // Then set the voxel in our chunk.

        chunk.map[voxel.x, voxel.y, voxel.z].id = value;

        AddToModifiedChunkList(chunk);

    }

    public VoxelState GetVoxel (Vector3 pos) {

        // If the voxel is outside of the world we don't need to do anything with it.
        if (!IsVoxelInWorld(pos))
            return null;

        // Find out the ChunkCoord value of our voxel's chunk.
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        // Then reverse that to get the position of the chunk.
        x *= VoxelData.ChunkWidth;
        z *= VoxelData.ChunkWidth;

        // Check if the chunk exists. If not, create it.
        ChunkData chunk = RequestChunk(new Vector2Int(x, z), true);

        // Then create a Vector3Int with the position of our voxel *within* the chunk.
        Vector3Int voxel = new Vector3Int((int)(pos.x - x), (int)pos.y, (int)(pos.z - z));

        // Then set the voxel in our chunk.
        return chunk.map[voxel.x, voxel.y, voxel.z];

    }

}
