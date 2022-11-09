using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    
    public float defaultLength = 5.0f;
    public GameObject tip;
    public VRInput vrInput;
    private LineRenderer lineRenderer;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        float pointerLength = 0.0f;

        PointerEventData data = vrInput.GetData();
        if (data.pointerCurrentRaycast.distance == 0)
        {
            pointerLength = defaultLength;
        }
        else
        {
            pointerLength = data.pointerCurrentRaycast.distance;
        }

        //Create Raycast
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, pointerLength);

        Vector3 endPosition = transform.position + (transform.forward * pointerLength);

        if (hit.collider != null)
        {
            endPosition = hit.point;
        }

        tip.transform.position = endPosition;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

}
