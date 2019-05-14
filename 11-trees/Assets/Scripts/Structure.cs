using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure {

    public static void MakeTree (Vector3 position, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight) {

        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(position.x, position.z), 250f, 3f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3(position.x, position.y + i, position.z), 6));

        for (int x = -3; x < 4; x++) {
            for (int y = 0; y < 7; y++) {
                for (int z = -3; z < 4; z++) {
                    queue.Enqueue(new VoxelMod(new Vector3(position.x + x, position.y + height + y, position.z + z), 11));
                }
            }
        }

    }

}
