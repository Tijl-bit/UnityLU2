using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    public float minX = 0f, maxX = 200f, minY = 0f, maxY = 200f;

    void Update()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minX, maxX),
            Mathf.Clamp(transform.position.y, minY, maxY),
            transform.position.z
        );
    }
}
