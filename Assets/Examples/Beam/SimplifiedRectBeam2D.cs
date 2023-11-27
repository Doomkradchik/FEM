

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;


[RequireComponent(typeof(LineHandler))]
public class SimplifiedRectBeam2D : MonoBehaviour
{
    [SerializeField] private Transform el1;
    [SerializeField] private Transform el2;


    public float maxBeamStress = 100f;

    Matrix<float> k;

    [Min(1f)]
    public float ratio = 1;
    public float height;

    float A;
    const float EC = 2; // 200 * 10^9 


    float L;

    LineHandler lineHandler;

    Matrix<float> stressConvertion;


    Vector2 initPlacementP;

    Matrix<float> NodalDisplacement
    {
        get
        {
            //var dir = ((Vector2)el2.transform.position - (Vector2)el1.transform.position).normalized;
            //var dest = (Vector2)el1.transform.position + dir * L;
            //Debug.DrawLine((Vector2)el1.transform.position, (Vector2)dest);

            var vec = (Vector2)el2.transform.position - (Vector2)el1.transform.position;

            var pOffset = vec - initPlacementP;
            var rOffset = Vector2.SignedAngle(el1.transform.right, el2.transform.right);




            return Matrix<float>.Build.DenseOfArray(new float[6, 1]
            {
                { pOffset.x },
                { pOffset.y },
                { rOffset },
                { -pOffset.x },
                { -pOffset.y },
                { -rOffset },
            });
        }
    }

    private void Awake()
    {
        lineHandler = GetComponent<LineHandler>();
        lineHandler.Init(el1, el2);
    }


    private void Start()
    {
        initPlacementP = (Vector2)el2.transform.position - (Vector2)el1.transform.position;

        A = height;
        L = Vector2.Distance(el1.transform.position, el2.transform.position);

        float E = EC * ratio;
        float I = height* height / 12f;
        float ael = A * E / L;
        float eil12 = 12f * E * I / (L * L * L);
        float eil6 = 6f * E * I / (L * L);
        float eil2 = 2f * E * I / L;
        float eil4 = 4f * E * I / L;

        stressConvertion = Matrix<float>.Build.DenseOfDiagonalArray(new float[6] {
         1f / A , 1f / A , height /( 2f * I ),   1f / A , 1f / A , height /( 2f * I )
        });

        k = Matrix<float>.Build.DenseOfArray(new float[6, 6]
        {
            { ael, 0f, 0f, -ael, 0f, 0f },
            { 0f, eil12, eil6 , 0f, -eil12, eil6 },
            { 0f, eil6, eil4 , 0f, -eil6, eil2 },
            { -ael, 0f, 0f, ael, 0f, 0f },
            { 0f, -eil12, -eil6 , 0f, eil12, -eil6 },
            { 0f, eil6, eil2 , 0f, -eil6, eil4 },
        });
    }

    float ConvertRotation(float rot) => rot >= 180f ? 360f - rot : rot;

    private void FixedUpdate()
    {
        var nodalDisplacement = NodalDisplacement;
        ///<summary>
        ///Beam
        /// V - shear force
        /// M - bending moment
        /// Qs = F / A;
        /// Qb = M * y / I; (assume that y is height / 2)
        /// </summary>

        var nodalForces = k * nodalDisplacement;
     
        var nodalStresses = stressConvertion *  nodalForces;



        lineHandler.UpdatePlacements();
        lineHandler.SetNodalStresses(nodalStresses.ToArray(), maxBeamStress);

#if FEM_DEBUG
        Debug.DrawLine(el1.transform.position, (Vector2)el1.transform.position + new Vector2(nodalForces[0, 0], nodalForces[1, 0]), Color.red);
        Debug.DrawLine(el1.transform.position, el1.transform.position + Vector3.forward * nodalForces[2, 0], Color.blue);
        Debug.DrawLine(el2.transform.position, (Vector2)el2.transform.position + new Vector2(nodalForces[3, 0], nodalForces[4, 0]), Color.red);
        Debug.DrawLine(el2.transform.position, el2.transform.position + Vector3.forward * nodalForces[5, 0], Color.blue);
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
        Debug.Log("Fy2 " + nodalForces[4, 0]);
        Debug.Log("Mz2 " + nodalForces[5, 0]);
    }

#endif
}



