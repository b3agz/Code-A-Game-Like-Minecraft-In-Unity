using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData {

	public static readonly int ChunkWidth = 16;
	public static readonly int ChunkHeight = 128;
    public static readonly int WorldSizeInChunks = 100;

    // Lighting Values
    public static float minLightLevel = 0.1f;
    public static float maxLightLevel = 0.9f;

    public static float unitOfLight {

       // Light is handled as float (0-1) but Minecraft stores light as a byte (0-15), so we need to how much
       // of that float a single light level represents.
       get { return 1f / 16f; }

    }

    public static float tickLength = 1f;

    public static int seed;

    public static int WorldCentre {

        get { return (WorldSizeInChunks * ChunkWidth) / 2; }

    }

    public static int WorldSizeInVoxels {

        get { return WorldSizeInChunks * ChunkWidth; }

    }

    public static readonly int TextureAtlasSizeInBlocks = 16;
    public static float NormalizedBlockTextureSize {

        get { return 1f / (float)TextureAtlasSizeInBlocks; }

    }

	public static readonly Vector3[] voxelVerts = new Vector3[8] {

		new Vector3(0.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 0.0f, 1.0f),
		new Vector3(1.0f, 1.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 1.0f),

	};

	public static readonly Vector3Int[] faceChecks = new Vector3Int[6] {

		new Vector3Int(0, 0, -1), // Back
		new Vector3Int(0, 0, 1), // Front
		new Vector3Int(0, 1, 0),
		new Vector3Int(0, -1, 0),
		new Vector3Int(-1, 0, 0),
		new Vector3Int(1, 0, 0)

	};

    public static readonly int[] revFaceCheckIndex = new int[6] { 1, 0, 3, 2, 5, 4 };

	public static readonly int[,] voxelTris = new int[6,4] {

        // Back, Front, Top, Bottom, Left, Right

		// 0 1 2 2 1 3
		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

	public static readonly Vector2[] voxelUvs = new Vector2[4] {

		new Vector2 (0.0f, 0.0f),
		new Vector2 (0.0f, 1.0f),
		new Vector2 (1.0f, 0.0f),
		new Vector2 (1.0f, 1.0f)

	};


}
