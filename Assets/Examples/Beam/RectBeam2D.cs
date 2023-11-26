

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;



public class RectBeam2D : MonoBehaviour
{
    [SerializeField] private Transform el1;
    [SerializeField] private Transform el2;

    Matrix<float> k;

    [Min(1f)]
    public float ratio = 1;
    public float height;

    const float E = 2; // 200 * 10^9 

    Matrix<float> initialPlacement;


    Matrix<float> CurrentPlacement
    {
        get
        {
            return Matrix<float>.Build.DenseOfArray(new float[6, 1]
            {
            { el1.transform.position.x },
            { el1.transform.position.y },
            { el1.transform.rotation.eulerAngles.z },
            { el2.transform.position.x },
            { el2.transform.position.y },
            { el2.transform.rotation.eulerAngles.z },
            });
        }
    }


    private void Start()
    {

        initialPlacement = CurrentPlacement;

        float I = height* height / 12f;
        float A =  height;
        float L =  Vector2.Distance(el1.transform.position, el2.transform.position);

        float ael = A * E / L;
        float eil12 = 12f * E * I / (L * L * L);
        float eil6 = 6f * E * I / (L * L);
        float eil2 = 2f * E * I / L;
        float eil4 = 4f * E * I / L;

        k = Matrix<float>.Build.DenseOfArray(new float[6, 6]
        {
            { ael, 0f, 0f, -ael, 0f, 0f },
            { 0f, eil12, eil6 , 0f, -eil12, eil6 },
            { 0f, eil6, eil4 , 0f, -eil6, eil2 },
            { -ael, 0f, 0f, -ael, 0f, 0f },
            { 0f, -eil12, -eil6 , 0f, eil12, -eil6 },
            { 0f, eil6, eil2 , 0f, -eil6, eil4 },
        });
    }

    private void FixedUpdate()
    {
        var nodalDisplacement = CurrentPlacement - initialPlacement;
        var nodalForces = k * nodalDisplacement;
#if FEM_DEBUG
        DebugNodalForces(nodalForces);
#endif
    }

#if FEM_DEBUG 

    void DebugNodalForces(Matrix<float> nodalForces)
    {
        Debug.Log("Fx1 " + nodalForces[0, 0]);
        Debug.Log("Fy1 " + nodalForces[1, 0]);
        Debug.Log("Mz1 " + nodalForces[2, 0]);
        Debug.Log("Fx2 " + nodalForces[3, 0]);
        Debug.Log("Fy3 " + nodalForces[4, 0]);
        Debug.Log("Mz5 " + nodalForces[5, 0]);
    }

#endif
}



