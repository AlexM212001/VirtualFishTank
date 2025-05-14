
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

    public int numBoidsToSpawn = 10;
    public List<GameObject> boids = null;

    private void Start()
    {

        for (int i =0; i<= numBoidsToSpawn;i++)
        {
        Vector3 position = new Vector3(Random.Range(-1.4f, 1.4f), Random.Range(0f, 1.4f), Random.Range(-0.9f, 0.9f));
        Quaternion rotation = Quaternion.identity;
            GameObject spawnedBoid = Instantiate(boidPrefab, position, rotation);
          spawnedBoid.GetComponent<Renderer>().material.SetColor("_BaseColor", Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 1));
            spawnedBoid.transform.localScale *= Random.Range(0.5f, 1.5f);
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

   


}

