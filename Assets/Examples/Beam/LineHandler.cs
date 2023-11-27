using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineHandler : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [Min(2)]
    [SerializeField] private int numOfPositions = 100;
    [SerializeField] private float influenceMagnitude = 1f;

    private void Start()
    {
        lineRenderer.positionCount = numOfPositions;
    }
    Transform _el1, _el2;

    public void Init(Transform el1, Transform el2)
    {
        _el1 = el1;
        _el2 = el2;
    }

    public void UpdatePlacements()
    {
        var a = (Vector2)_el1.position;
        var b = (Vector2)_el2.position;
        var an = (Vector2)_el1.transform.right * influenceMagnitude;
        var bn = -1f * (Vector2)_el2.transform.right * influenceMagnitude;
        Debug.DrawLine(a, a + an);
        Debug.DrawLine(b, b + bn);

        for (int i = 0; i < numOfPositions; i++)
        {
            var pos = GetPosition(a, a + an, b , b + bn, i);
            lineRenderer.SetPosition(i, pos);
        }      
    }

    float GetT(int index) => (float)(index + 1) / (float)numOfPositions;
    float GetINVT(int index) => 1f - (float)(index + 1) / (float)numOfPositions;

    public void SetNodalStresses(float[,] nodalStresses, float maxBeamStress)
    {
        var totalStress = Mathf.Abs(nodalStresses[0, 0]) + Mathf.Abs(nodalStresses[1, 0]) 
            + Mathf.Abs(nodalStresses[2, 0]);
        var color = Color.Lerp(Color.green, Color.red, totalStress / maxBeamStress);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    Vector3 GetPosition(Vector2 a, Vector2 anp, Vector2 b, Vector2 bnp, int index)
    {
        var t = GetT(index);
        var invT = GetINVT(index);
        return invT * invT * invT * a + 3 * invT * invT * t * anp + 3 * invT * t * t * bnp + t * t * t * b;
    }
}
