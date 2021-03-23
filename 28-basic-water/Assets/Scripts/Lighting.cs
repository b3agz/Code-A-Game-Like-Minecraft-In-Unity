using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Lighting {

    public static void RecalculateNaturaLight (ChunkData chunkData) {

        for (int x = 0; x < VoxelData.ChunkWidth; x++) {
            for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                CastNaturalLight(chunkData, x, z, VoxelData.ChunkHeight - 1);

            }
        }

    }

    // Propogates natural light straight down from at the given x,z coords starting from the startY value.
    public static void CastNaturalLight (ChunkData chunkData, int x, int z, int startY) {

        // Little check to make sure we don't try and start from above the world.
        if (startY > VoxelData.ChunkHeight - 1) {
            startY = VoxelData.ChunkHeight - 1;
            Debug.LogWarning("Attempted to cast natural light from above world.");
        }

        // Bool to keep check of whether the light has hit a block with opacity.
        bool obstructed = false;

        // Loop from top to bottom of chunk.
        for (int y = startY; y > -1; y--) {

            // Cache current voxel so I don't have to type as much.
            VoxelState voxel = chunkData.map[x, y, z];

            // If light has been obstructed, all blocks below that point are set to 0.
            if (obstructed) {
                voxel.light = 0;
            // Else if block has opacity, set light to 0 and obstructed to true.
            } else if (voxel.properties.opacity > 0) {
                voxel.light = 0;
                obstructed = true;
            // Else set light to 15.
            } else {
                voxel.light = 15;
            }
        }
    }

}
