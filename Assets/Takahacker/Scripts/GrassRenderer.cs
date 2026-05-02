using UnityEngine;

public class GrassRenderer : MonoBehaviour
{
    public Mesh grassMesh;       // quad simples (plane ou custom mesh)
    public Material grassMaterial;
    public int count = 10000;
    public float spread = 50f;

    private ComputeBuffer argsBuffer;
    private ComputeBuffer positionsBuffer;

    void Start()
    {
        Vector4[] positions = new Vector4[count];
        for (int i = 0; i < count; i++)
            positions[i] = new Vector4(
                Random.Range(-spread, spread), 0,
                Random.Range(-spread, spread), 1f // w = radius p/ displacement
            );

        positionsBuffer = new ComputeBuffer(count, sizeof(float) * 4);
        positionsBuffer.SetData(positions);
        grassMaterial.SetBuffer("_Positions", positionsBuffer);

        uint[] args = { grassMesh.GetIndexCount(0), (uint)count, 0, 0, 0 };
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        argsBuffer.SetData(args);
    }

    void Update()
    {
        Graphics.DrawMeshInstancedIndirect(grassMesh, 0, grassMaterial,
            new Bounds(Vector3.zero, Vector3.one * 200f), argsBuffer);
    }

    void OnDestroy()
    {
        positionsBuffer?.Release();
        argsBuffer?.Release();
    }
}