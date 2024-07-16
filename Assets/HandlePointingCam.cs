using UnityEngine;

public class HandlePointingCam : MonoBehaviour
{
    [SerializeField] private GraplingMovement graplingMovement;
    [SerializeField] private string pickableTag = "Pickable";

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 is for left mouse button
        {
            HandleRaycast();
        }
    }

    void HandleRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag(pickableTag))
            {
                Debug.Log("Pickable object hit at position: " + hit.transform.position);
                graplingMovement.SetGrapplePoint(hit.point);
            }
        }
    }
}
