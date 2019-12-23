using UnityEngine;
using System.Collections;

/// <summary>
/// WORLD SPACE CANVAS FACES PLAYER AT ALL TIMES
/// </summary>
public class CameraFacingBillboard : MonoBehaviour
{
    private Camera m_Camera;

    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);
    }
}