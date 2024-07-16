using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraplingMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform grapplePoint; // Optional in case you want to assign it in the Inspector
    [SerializeField] private float grappleSpeed = 10f; // Speed at which the player is pulled towards the grapple point
    [SerializeField] private float pullForce = 10f; // The force applied to pull the player towards the grapple point
    [SerializeField] private LayerMask grappleableLayer; // Layer mask to define grappleable objects

    private bool isGrappling = false;
    private SpringJoint joint;
    private Coroutine grapplingCoroutine;

    void Awake()
    {
        if (grapplePoint == null)
        {
            // Create a new GameObject to act as the grapple point if none is assigned
            GameObject grapplePointObject = new GameObject("GrapplePoint");
            grapplePoint = grapplePointObject.transform;
        }
    }

    void Update()
    {
        if (isGrappling)
        {
            DrawRope();
        }

        // Stop grappling when the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }
    }

    public void SetGrapplePoint(Vector3 point)
    {
        grapplePoint.position = new Vector3(point.x, point.y, 0);

        joint = gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint.position;

        float distanceFromPoint = Vector3.Distance(transform.position, grapplePoint.position);

        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lineRenderer.positionCount = 2;
        isGrappling = true;

        // Apply initial impulse force towards the grapple point
        Vector3 direction = (grapplePoint.position - transform.position).normalized;
        rb.velocity = Vector3.zero;
        rb.AddForce(direction * pullForce, ForceMode.Impulse);

        if (grapplingCoroutine != null)
        {
            StopCoroutine(grapplingCoroutine);
        }
        grapplingCoroutine = StartCoroutine(ApplyGrappleForce());
    }

    void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.positionCount = 0;
        Destroy(joint);

        if (grapplingCoroutine != null)
        {
            StopCoroutine(grapplingCoroutine);
        }
    }

    void DrawRope()
    {
        if (!joint) return;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint.position);
    }

    private IEnumerator ApplyGrappleForce()
    {
        while (isGrappling)
        {
            Vector3 direction = (grapplePoint.position - transform.position).normalized;
            rb.AddForce(direction * grappleSpeed * Time.deltaTime, ForceMode.VelocityChange);
            Debug.Log($"RB velocity: {rb.velocity}");
            Debug.Log($"GrapleForce: {grappleSpeed * Time.deltaTime}");
            yield return null;
        }
    }
}
