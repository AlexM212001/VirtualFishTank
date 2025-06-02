
using System.Collections.Generic;
using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{


    public enum ControlMode
    {
        seek,
        Pursue,
        Food,
        Obstacle
    }
    public ControlMode controlMode = ControlMode.seek;

    public GameObject boidPrefab = null;
    public GameObject targetObject = null;
    public GameObject foodPrefab = null;

    public int numBoidsToSpawn = 10;

    public List<Boid> boids = new List<Boid>();


    private void Start()
    {


        targetObject = GameObject.Find("TargetObject");
        boids = new List<Boid>();


        for (int i = 0; i < numBoidsToSpawn; i++)
        {
            Vector3 position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0f, 1.4f), Random.Range(-0.9f, 0.9f));
            Quaternion rotation = Quaternion.identity;
            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);

            Boid boidComponet = spawnedBoid.GetComponent<Boid>();

            boidComponet.maxSpeed = Random.Range(1f, 5f);
            boidComponet.AccelMax = Random.Range(1f, 10f);
            boids.Add(boidComponet);// Add the new object to the list

            spawnedBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1));

            spawnedBoid.transform.localScale *= Random.Range(0.5f, 1.5f);
        }
        switch (controlMode)
        {
            case ControlMode.seek:
                {
                    seekMode();
                    break;
                }
            case ControlMode.Pursue:
                {
                    pursueMode();
                    break;
                }
            case ControlMode.Food:
                {

                    break;

                }
            case ControlMode.Obstacle:
                Debug.Log("Obstacle Mode");
                break;
        }
    }



    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            controlMode = ControlMode.seek;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            controlMode = ControlMode.Pursue;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            controlMode = ControlMode.Food;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            controlMode = ControlMode.Obstacle;
        }



    }

    private void FixedUpdate()
    {




        //camera tracking the mouse 

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool didhit = Physics.Raycast(ray, out hitInfo, 100);

        if (didhit)
        {
            targetObject.transform.position = hitInfo.point;
        }


        seekMode();

    }

    private void seekMode()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 accel = boids[i].seek(targetObject.transform.position, boids[i].AccelMax);

            if (Input.GetMouseButton(0))
            {
                boids[i].Rig.velocity += accel * Time.deltaTime;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green);
            }
            else if (Input.GetMouseButton(1))
            {
                boids[i].Rig.velocity -= accel * Time.deltaTime;
                Debug.DrawRay(boids[i].transform.position, accel, Color.red);
            }
        }
    }

    private void pursueMode()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            Vector3 accel = boids[i].pursue(targetObject.transform.position, boids[i].AccelMax, boids[i].maxSpeed);

            if (Input.GetMouseButton(0)) // Left mouse button: pursue
            {
                boids[i].Rig.velocity += accel * Time.deltaTime;
                Debug.DrawRay(boids[i].transform.position, accel, Color.green);
            }
            else if (Input.GetMouseButton(1)) // Right mouse button: evade
            {
                boids[i].Rig.velocity -= accel * Time.deltaTime;
                Debug.DrawRay(boids[i].transform.position, -accel, Color.red);
            }
        }
    }
}

