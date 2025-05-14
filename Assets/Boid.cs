using UnityEngine;


public class Boid : MonoBehaviour
{
    public Rigidbody Rig;
    private void Awake()
    {
        Rig = GetComponent<Rigidbody>();
        Rig.velocity = Random.insideUnitSphere;

    }
    public void Update()
    {
        AlignToVolocity();
    }

    public void AlignToVolocity()
    {
        transform.forward = Vector3.RotateTowards(transform.forward, Rig.velocity.normalized, Mathf.Deg2Rad * 1800 * Time.deltaTime, 100);
    }


}



