
using System.Collections.Generic;
using UnityEngine;

public class BoidSimulationControl : MonoBehaviour
{
    [SerializeField] GameObject foodPrefab;
    [SerializeField] GameObject rockPrefab;

   
    public GameObject boidPrefab = null;
    public GameObject targetObject = null;
  


    public List<GameObject> rocks = null; // List of obstacles
    public int boidsToSpawn = 10;
    bool enableObstacleAvoidance = true;

    public enum ControlMode
    {
        seek,
        Pursue,
        Food,
        Obstacle
    }
    public ControlMode controlMode = ControlMode.seek;


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


            spawnedBoid.transform.localScale *= Random.Range(.9f, 3f); // Random size
            Boid boidComponet = spawnedBoid.GetComponent<Boid>();

            boidComponet.maxSpeed = Random.Range(1f, 5f);
            boidComponet.AccelMax = Random.Range(1f, 10f);
            boids.Add(boidComponet);// Add the new object to the list

            spawnedBoid.transform.localScale *= Random.Range(0.5f, 1.5f);
        }
       
        
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
                Debug.DrawRay(boids[i].transform.position, -accel, Color.green);
            }
        }
    }
    private void SpawnFood()
    {
        Instantiate(foodPrefab, targetObject.transform.position, Random.rotation); // spawn food at targetObject position
    }
    private void SpawnObstacle()
    {
        Vector3 spawnPos = targetObject.transform.position; // Get the position of the target object
        spawnPos.y -= 0.5f; // Set the y position to 0.5 to avoid spawning below the ground
        GameObject spawnedRock = Instantiate(rockPrefab, spawnPos, Random.rotation); // spawn obstacle at targetObject position
        rocks.Add(spawnedRock); // Add the spawned rock to the list of rocks
    }
    private void FoodArrivalBehaviour()
    {
        //boids seeking the food
        for (int i = 0; i < boids.Count; i++)
        {
            float foodSeekRadius = 0.8f; // Radius to seek food
            Collider[] colliders = Physics.OverlapSphere(boids[i].transform.position, foodSeekRadius);

            Food closestFood = null; // Variable to store the closest food
            float closestFoodDistance = float.PositiveInfinity; // Initialize closest food distance to infinity

            foreach (Collider collider in colliders)
            {
                Food food = collider.GetComponent<Food>(); // Check if the collider has a Food component
                if (food != null) // seeks food if not null
                {
                    float distanceToFood = Vector3.Distance(food.transform.position, boids[i].transform.position); // Calculate distance to food
                    if (distanceToFood < closestFoodDistance) // If left mouse button is pressed
                    {
                        closestFoodDistance = distanceToFood; // Set the closest food
                        closestFood = food; // Set the closest food
                    }
                }
            }

            if (closestFood != null)
            {
                // If a food is found, apply the seek acceleration
                Vector3 accel = boids[i].Arrive(closestFood.transform.position, boids[i].AccelMax, 0.05f, 0.38f);
                boids[i].currentLinearAcceleration += accel;
                Debug.DrawRay(boids[i].transform.position, boids[i].currentLinearAcceleration, Color.green); // Draw acceleration towards food

                // Check if the boid is close enough to the food
                if (closestFoodDistance < 0.02f)
                {
                    // If the boid is close enough to the food, destroy the food object
                    Destroy(closestFood.gameObject); // Destroy the food object after consumption
                }
            }
        }
    }

    private void ResetSimulation()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            Destroy(boids[i].gameObject); // Destroy all boids
        }
        boids.Clear(); // Clear the list of boids

        for (int i = 0; i < rocks.Count; i++)
        {
            Destroy(rocks[i]); // Destroy all obstacles
        }
        rocks.Clear(); // Clear the list of obstacles

        Start(); // Restart the simulation by calling Start method
    }


    public void Update()
    {
         // Number Keys to switch between modes
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
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetSimulation();
        }

        switch (controlMode)
        {
            case ControlMode.Food:
                if (Input.GetMouseButtonDown(0)) // If left mouse button is pressed
                    SpawnFood(); // Spawn food when in Food mode
                break;
            case ControlMode.Obstacle:
                if (Input.GetMouseButtonDown(0)) // If left mouse button is pressed
                    SpawnObstacle(); // Spawn obstacle when in Obstacle mode
                break;
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].currentLinearAcceleration = Vector3.zero; // Reset acceleration for each boid each cycle
        }

        // Disable obstacle avoidance while mouse is held or in Food mode
        // enableObstacleAvoidance = !(Input.GetMouseButton(0) || controlMode == ControlMode.Food);

        // if (enableObstacleAvoidance)
        // {
        foreach (Boid boid in boids)
        {
            // Obstacle avoidance
            boid.currentLinearAcceleration = boid.ObstacleAvoidance(0.55f, boid.AccelMax);
        }
        //}

        switch (controlMode)
        {
            case ControlMode.seek:
                seekMode();
                break;
            case ControlMode.Pursue:
                pursueMode();
                break;
        }

        FoodArrivalBehaviour();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo; // Will store information about the intersection if any
        bool didHit = Physics.Raycast(ray, out hitInfo, 100);

        if (didHit)
        {
            targetObject.transform.position = hitInfo.point; // Move the target to the hit point
        }

        // Enforce acceleration limit and apply velocity change
        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].currentLinearAcceleration = Vector3.ClampMagnitude(boids[i].currentLinearAcceleration, boids[i].AccelMax);

            boids[i].Rig.velocity += boids[i].currentLinearAcceleration * Time.fixedDeltaTime;
        }
    }


}


