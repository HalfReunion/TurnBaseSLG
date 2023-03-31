using UnityEngine;

public class BulletProjectTile : MonoBehaviour
{
    private Vector3 targetPos;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVfx;

    public void Setup(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 dir = (this.targetPos - transform.position).normalized;
        float moveSpeed = 100f;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPos);

        transform.position += moveSpeed * dir * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPos);
        if (distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = targetPos;
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
            Instantiate(bulletHitVfx, targetPos, Quaternion.identity);
        }
    }
}