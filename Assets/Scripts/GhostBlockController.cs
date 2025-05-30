using UnityEngine;

public class GhostBlockController : MonoBehaviour
{
    private float currentRotation = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentRotation -= 90f;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentRotation += 90f;
            transform.rotation = Quaternion.Euler(0, 0, currentRotation);
        }
    }

    public Quaternion GetCurrentRotation()
    {
        return Quaternion.Euler(0, 0, currentRotation);
    }
}