using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData {

    // The global position of the chunk. ie, (16, 16) NOT (1, 1). We want to be able to
    // access it as a Vector2Int, but Vector2Int's are not serialized so we won't be able
    // to save them. So we'll store them as ints.
    int x;
    int y;
    public Vector2Int position {

        get { return new Vector2Int(x, y); }
        set {
            x = value.x;
            y = value.y;
        }
    }

    [HideInInspector] // Displaying lots of data in the inspector slows it down even more so hide this one.
    public VoxelState[,,] map = new VoxelState[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    // Constructors take in position data to ensure we never have ChunkData without a position.
    public ChunkData (Vector2Int pos) { position = pos; }
    public ChunkData (int x, int z) { position = new Vector2Int(x, z); }

	public void Populate () {
		
		for (int y = 0; y < VoxelData.ChunkHeight; y++) {
			for (int x = 0; x < VoxelData.ChunkWidth; x++) {
				for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                    map[x, y, z] = new VoxelState(World.Instance.GetVoxel(new Vector3(x + position.x, y, z + position.y)));
    
				}
			}
		}
        World.Instance.worldData.AddToModifiedChunkList(this);
    }

}
