﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Voxel Mesh Data", menuName = "MinecraftTutorial/Voxel Mesh Data")]
public class VoxelMeshData : ScriptableObject {

    public string blockName;
    public FaceMeshData[] faces; // 6 faces using our established winding order.

}

[System.Serializable]
public class VertData {

    public Vector3 position; // Position relative to the voxel's origin point.
    public Vector2 uv; // Texture UV relative to the origin as defined by BlockTypes.

    public VertData (Vector3 pos, Vector2 _uv) {

        position = pos;
        uv = _uv;

    }

    public Vector3 GetRotatedPosition (Vector3 angles) {
        
        Vector3 centre = new Vector3(0.5f, 0.5f, 0.5f); // The centre of the block that we are pivoting around.
        Vector3 direction = position - centre; // Get the direction from the centre to the current vertice.
        direction = Quaternion.Euler(angles) * direction; // Rotate the direction by angles specified in the function parameters.
        return direction + centre; // Add the modified direction to the center to get our new position and return.

    }

}

[System.Serializable]
public class FaceMeshData {

    // Because all of the verts in this face are facing the same direction, we can store a single normal value
    // for each face and use that for each vert in the face.

    public string direction; // Purely to make things easier to read in the inspector.
    public VertData[] vertData;
    public int[] triangles;

    public VertData GetVertData(int index) {

        return vertData[index];

    }

}