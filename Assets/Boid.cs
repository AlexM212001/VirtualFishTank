using UnityEngine;


public class Boid : MonoBehaviour
{
    public Rigidbody Rig;
    public float maxSpeed = 5f;
    public float AccelMax = 10f;

    private void FixedUpdate()
    {
        float speed = Rig.velocity.magnitude;
        if (speed > maxSpeed)
        {
            Rig.velocity = Rig.velocity.normalized * maxSpeed;
        }
    }

    private void Awake()
    {
        Rig = GetComponent<Rigidbody>();
        Rig.velocity = Random.insideUnitSphere;

    }
    public void Update()
    {
        AlignToVolocity();
        Debug.DrawRay(transform.position, Rig.velocity , Color.red);

    }

    public void AlignToVolocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, Rig.velocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime, 100);
    }

    public Vector3 seek (Vector3 target , float acceleration)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 accel = toTargetNormalized * acceleration;

        return accel;

    }

    public Vector3 pursue(Vector3 target, float acceleration, float desierdSpeed)
    {
      Vector3 toTarget = target - transform.position;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 DesierdVelocity = toTargetNormalized * desierdSpeed;
        Vector3 deltaVEl = DesierdVelocity - Rig.velocity;
        Vector3 accel = deltaVEl.normalized * acceleration;

        return accel;



    }
}



