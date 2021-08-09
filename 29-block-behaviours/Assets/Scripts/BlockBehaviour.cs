using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockBehaviour {

    public static bool Active (VoxelState voxel) {

        switch (voxel.id) {

            case 3: // Grass
                if ((voxel.neighbours[0] != null && voxel.neighbours[0].id == 5) ||
                    (voxel.neighbours[1] != null && voxel.neighbours[1].id == 5) ||
                    (voxel.neighbours[4] != null && voxel.neighbours[4].id == 5) ||
                    (voxel.neighbours[5] != null && voxel.neighbours[5].id == 5)) {
                    return true;
                }
                break;

        }

        // If we get here, the block either isn't active or doesn't have a behaviour. Just return false.
        return false;

    }

    public static void Behave (VoxelState voxel) {

        switch (voxel.id) {

            case 3: // Grass
                if (voxel.neighbours[2] != null && voxel.neighbours[2].id != 0) {
                    voxel.chunkData.chunk.RemoveActiveVoxel(voxel);
                    voxel.chunkData.ModifyVoxel(voxel.position, 5, 0);
                    return;
                }

                List<VoxelState> neighbours = new List<VoxelState>();
                if ((voxel.neighbours[0] != null && voxel.neighbours[0].id == 5)) neighbours.Add(voxel.neighbours[0]);
                if ((voxel.neighbours[1] != null && voxel.neighbours[1].id == 5)) neighbours.Add(voxel.neighbours[1]);
                if ((voxel.neighbours[4] != null && voxel.neighbours[4].id == 5)) neighbours.Add(voxel.neighbours[4]);
                if ((voxel.neighbours[5] != null && voxel.neighbours[5].id == 5)) neighbours.Add(voxel.neighbours[5]);

                if (neighbours.Count == 0) return;

                int index = Random.Range(0, neighbours.Count);
                neighbours[index].chunkData.ModifyVoxel(neighbours[index].position, 3, 0);

                break;

        }

    }

}
