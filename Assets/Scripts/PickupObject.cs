using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupDistance = 3f;
    private GameObject heldObject;

    void Update()
    {
        // When left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            TryPickup();
        }

        // When left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            Drop();
        }

        // If holding an object, keep it at the hold point
        if (heldObject != null)
        {
            heldObject.transform.position = holdPoint.position;
        }
    }

    void TryPickup()
    {
        // Ray from the center of the screen
        Ray ray = Camera.main.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.CompareTag("Vial"))
            {
                heldObject = hit.collider.gameObject;

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true; // freeze physics while holding
                }
            }
        }
    }

    void Drop()
    {
        if (heldObject != null)
        {
            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // unfreeze physics when dropped
            }

            heldObject = null;
        }
    }
}
