using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFollower : MonoBehaviour
{

    [SerializeField] private Transform el1;
    [SerializeField] private Transform el2;
    [SerializeField] private LineRenderer lineRenderer;

    private void Update()
    {
        lineRenderer.SetPosition(0, el1.transform.position);
        lineRenderer.SetPosition(1, el2.transform.position);
    }
}
