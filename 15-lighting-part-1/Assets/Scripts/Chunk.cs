using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class Chunk {

    public ChunkCoord coord;

    GameObject chunkObject;
	MeshRenderer meshRenderer;
	MeshFilter meshFilter;

	int vertexIndex = 0;
	List<Vector3> vertices = new List<Vector3> ();
	List<int> triangles = new List<int> ();
    List<int> transparentTriangles = new List<int>();
    Material[] materials = new Material[2];
	List<Vector2> uvs = new List<Vector2> ();
    List<Color> colors = new List<Color>();

    public Vector3 position;

	public byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    World world;
    
    private bool _isActive;
    private bool isVoxelMapPopulated = false;
    private bool threadLocked = false;

    public Chunk (ChunkCoord _coord, World _world, bool generateOnLoad) {

        coord = _coord;
        world = _world;
        isActive = true;

        if (generateOnLoad)
            Init();

    }

    public void Init () {

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        //materials[0] = world.material;
        //materials[1] = world.transparentMaterial;
        meshRenderer.material = world.material;

        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.z;
        position = chunkObject.transform.position;

        Thread myThread = new Thread(new ThreadStart(PopulateVoxelMap));
        myThread.Start();
        
    }

	void PopulateVoxelMap () {
		
		for (int y = 0; y < VoxelData.ChunkHeight; y++) {
			for (int x = 0; x < VoxelData.ChunkWidth; x++) {
				for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
    
				}
			}
		}

        _updateChunk();
        isVoxelMapPopulated = true;

	}

    public void UpdateChunk () {

        Thread myThread = new Thread (new ThreadStart(_updateChunk));
        myThread.Start();

    }

	private void _updateChunk () {

        threadLocked = true;

        while (modifications.Count > 0) {

            VoxelMod v = modifications.Dequeue();
            Vector3 pos = v.position -= position;
            voxelMap[(int)pos.x, (int)pos.y, (int)pos.z] = v.id;

        }

        ClearMeshData();

		for (int y = 0; y < VoxelData.ChunkHeight; y++) {
			for (int x = 0; x < VoxelData.ChunkWidth; x++) {
				for (int z = 0; z < VoxelData.ChunkWidth; z++) {

                    if (world.blocktypes[voxelMap[x,y,z]].isSolid)
					    UpdateMeshData (new Vector3(x, y, z));

				}
			}
		}

        lock (world.chunksToDraw) {
            world.chunksToDraw.Enqueue(this);
        }

        threadLocked = false;

	}

    void ClearMeshData () {

        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        transparentTriangles.Clear();
        uvs.Clear();
        colors.Clear();

    }

    public bool isActive {

        get { return _isActive; }
        set {

            _isActive = value;
            if (chunkObject != null)
                chunkObject.SetActive(value);

        }

    }

    public bool isEditable {

        get {

            if (!isVoxelMapPopulated || threadLocked)
                return false;
            else
                return true;

        }

    }

    bool IsVoxelInChunk (int x, int y, int z) {

        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;

    }

    public void EditVoxel (Vector3 pos, byte newID) {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        _updateChunk();

    }

    void UpdateSurroundingVoxels (int x, int y, int z) {

        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++) {

            Vector3 currentVoxel = thisVoxel + VoxelData.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z)) {

                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();

            }

        }

    }

	bool CheckVoxel (Vector3 pos) {

		int x = Mathf.FloorToInt (pos.x);
		int y = Mathf.FloorToInt (pos.y);
		int z = Mathf.FloorToInt (pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.CheckIfVoxelTransparent(pos + position);

		return world.blocktypes[voxelMap [x, y, z]].isTransparent;

	}

    public byte GetVoxelFromGlobalVector3 (Vector3 pos) {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(position.x);
        zCheck -= Mathf.FloorToInt(position.z);

        return voxelMap[xCheck, yCheck, zCheck];

    }

	void UpdateMeshData (Vector3 pos) {

        byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

        bool isTransparent = world.blocktypes[blockID].isTransparent;

		for (int p = 0; p < 6; p++) { 

			if (CheckVoxel(pos + VoxelData.faceChecks[p])) {

				vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [p, 0]]);
				vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [p, 1]]);
				vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [p, 2]]);
				vertices.Add (pos + VoxelData.voxelVerts [VoxelData.voxelTris [p, 3]]);

                AddTexture(world.blocktypes[blockID].GetTextureID(p));

                float lightLevel;

                int yPos = (int)pos.y + 1;
                bool inShade = false;
                while (yPos < VoxelData.ChunkHeight) {
                    if (voxelMap[(int)pos.x, yPos, (int)pos.z] != 0) {
                        inShade = true;
                        break;
                    }

                    yPos++;
                }

                if (inShade)
                    lightLevel = 0.5f;
                else
                    lightLevel = 0f;

                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));

                //if (!isTransparent) {
				    triangles.Add (vertexIndex);
				    triangles.Add (vertexIndex + 1);
				    triangles.Add (vertexIndex + 2);
				    triangles.Add (vertexIndex + 2);
				    triangles.Add (vertexIndex + 1);
				    triangles.Add (vertexIndex + 3);
                /*} else {
                    transparentTriangles.Add (vertexIndex);
				    transparentTriangles.Add (vertexIndex + 1);
				    transparentTriangles.Add (vertexIndex + 2);
				    transparentTriangles.Add (vertexIndex + 2);
				    transparentTriangles.Add (vertexIndex + 1);
				    transparentTriangles.Add (vertexIndex + 3);
                }*/

                vertexIndex += 4;

			}
		}

	}

	public void CreateMesh () {

		Mesh mesh = new Mesh ();
		mesh.vertices = vertices.ToArray ();

        //mesh.subMeshCount = 2;
        //mesh.SetTriangles(triangles.ToArray(), 0);
        //mesh.SetTriangles(transparentTriangles.ToArray(), 1);
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray ();
        mesh.colors = colors.ToArray();

		mesh.RecalculateNormals ();

		meshFilter.mesh = mesh;

	}

    void AddTexture (int textureID) {

        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));


    }

}

public class ChunkCoord {

    public int x;
    public int z;

    public ChunkCoord () {

        x = 0;
        z = 0;

    }

    public ChunkCoord (int _x, int _z) {

        x = _x;
        z = _z;

    }

    public ChunkCoord (Vector3 pos) {

        int xCheck = Mathf.FloorToInt(pos.x);
        int zCheck = Mathf.FloorToInt(pos.z);

        x = xCheck / VoxelData.ChunkWidth;
        z = zCheck / VoxelData.ChunkWidth;

    }

    public bool Equals (ChunkCoord other) {

        if (other == null)
            return false;
        else if (other.x == x && other.z == z)
            return true;
        else
            return false;

    }

}
