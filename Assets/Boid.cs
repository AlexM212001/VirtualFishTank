using UnityEngine;


public class Boid : MonoBehaviour
{
    public Rigidbody Rig;
    public float speed = 0f; // Current speed of the boid
    public float maxSpeed = 5f;
    public float AccelMax = 10f;

    public Vector3 currentLinearAcceleration = Vector3.zero;


    private void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1));
        Rig = GetComponent<Rigidbody>();
        Rig.velocity = Random.insideUnitSphere;

    }
    public void Update()
    {
        AlignToVolocity();
        Debug.DrawRay(transform.position, Rig.velocity, Color.red);

    }

    private void FixedUpdate()
    {
        float speed = Rig.velocity.magnitude;
        if (speed > maxSpeed)
        {
            Rig.velocity = Rig.velocity.normalized * maxSpeed;
        }
    }
    public void AlignToVolocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, Rig.velocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime, 100);
    }

    public Vector3 seek(Vector3 target, float magnitude)
    {
        Vector3 toTarget = target - transform.position;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 accel = toTargetNormalized * magnitude;

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
    public Vector3 Arrive(Vector3 target, float acceleration, float arriveInnerRadius, float arriveOuterRadius)
    {
        Vector3 toTarget = target - transform.position;
        float distance = toTarget.magnitude;
        Vector3 toTargetNormalized = toTarget.normalized;
        Vector3 desiredVelocity = toTargetNormalized * maxSpeed;
        float distancePercent = 1;
        if (distance < arriveInnerRadius)
        {
            
            desiredVelocity = Vector3.zero;
            DebugDrawing.DrawCircle(transform.position, Quaternion.Euler(90, 0, 0), arriveInnerRadius, 8, Color.red, Time.fixedDeltaTime); // Draw inner radius circle
        }
        else if (distance < arriveOuterRadius)
        {
            // If outside the outer radius, scale down the desired velocity
            distancePercent = distance / arriveOuterRadius; // Calculate percentage of distance to outer radius
            desiredVelocity *= distancePercent;
            DebugDrawing.DrawCircleDotted(transform.position, Quaternion.Euler(90, 0, 0), arriveOuterRadius, 16, 0.1f, 0.05f, Color.red, Time.fixedDeltaTime); // Draw outer radius circle
        }

        // Change needed to reach desired velocity
        Vector3 deltaVel = desiredVelocity - Rig.velocity;

        // Calculate acceleration based velocity change needed
        Vector3 accel = deltaVel.normalized * acceleration * distancePercent; // Scale acceleration based on distance percent

        return accel;


    }
    public Vector3 ObstacleAvoidance(float lookaheadDistance, float acceleration)
    {
        Vector3 accelOut = Vector3.zero;

        // Create whiskers for obstacle avoidance
        Ray whiskerRight = new Ray(transform.position, Quaternion.AngleAxis(18, transform.up) * transform.forward); // Right whisker (forward-right)
        Ray whiskerLeft = new Ray(transform.position, Quaternion.AngleAxis(-18, transform.up) * transform.forward); // Left whisker (forward-left)
        Ray whiskerUp = new Ray(transform.position, Quaternion.AngleAxis(-18, transform.right) * transform.forward); // Up whisker (forward-up)
        Ray whiskerDown = new Ray(transform.position, Quaternion.AngleAxis(18, transform.right) * transform.forward); // Down whisker (forward-down)

        // Checks if the whiskers hit an obstacle
        RaycastHit hitInfoRight;
        RaycastHit hitInfoLeft;
        RaycastHit hitInfoUp;
        RaycastHit hitInfoDown;

        // Check if the whiskers hit an obstacle
        bool didHitRight = Physics.Raycast(whiskerRight, out hitInfoRight, lookaheadDistance);
        bool didHitLeft = Physics.Raycast(whiskerLeft, out hitInfoLeft, lookaheadDistance);
        bool didHitUp = Physics.Raycast(whiskerUp, out hitInfoUp, lookaheadDistance);
        bool didHitDown = Physics.Raycast(whiskerDown, out hitInfoDown, lookaheadDistance);

        if (didHitRight)
        {
            // Turn left
            accelOut = transform.right * acceleration; // Apply acceleration to the left

            Debug.DrawLine(whiskerRight.origin, hitInfoRight.point, Color.red); // Draw a line to the hit point
        }
        else
        {
            Debug.DrawRay(whiskerRight.origin, whiskerRight.direction * lookaheadDistance, Color.yellow); // Draw the right whisker
        }

        if (didHitLeft)
        {
            // Turn right
            accelOut = -transform.right * acceleration; // Apply acceleration to the right

            Debug.DrawLine(whiskerLeft.origin, hitInfoLeft.point, Color.red); // Draw a line to the hit point
        }
        else
        {
            Debug.DrawRay(whiskerLeft.origin, whiskerLeft.direction * lookaheadDistance, Color.yellow); // Draw the left whisker
        }

        if (didHitUp)
        {
            // Turn down
            accelOut = -transform.up * acceleration; // Apply acceleration downwards

            Debug.DrawLine(whiskerUp.origin, hitInfoUp.point, Color.red); // Draw a line to the hit point
        }
        else
        {
            Debug.DrawRay(whiskerUp.origin, whiskerUp.direction * lookaheadDistance, Color.yellow); // Draw the up whisker
        }

        if (didHitDown)
        {
            // Turn up
            accelOut = transform.up * acceleration; // Apply acceleration upwards

            Debug.DrawLine(whiskerDown.origin, hitInfoDown.point, Color.red); // Draw a line to the hit point
        }
        else
        {
            Debug.DrawRay(whiskerDown.origin, whiskerDown.direction * lookaheadDistance, Color.yellow); // Draw the down whisker
        }

        return accelOut; // No avoidance needed
    }

}   



