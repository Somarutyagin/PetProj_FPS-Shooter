using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    private const float speed = 100f;
    private const float maxDistance = 100f;

    private Vector3 direction;
    private float traveledDistance = 0f;

    public void Initialize(Vector3 startPosition, Vector3 fireDirection)
    {
        transform.position = startPosition;
        direction = fireDirection.normalized;
        traveledDistance = 0f;
    }

    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        transform.Translate(direction * moveDistance, Space.World);
        traveledDistance += moveDistance;

        if (traveledDistance >= maxDistance)
        {
            gameObject.SetActive(false);
        }
    }
}