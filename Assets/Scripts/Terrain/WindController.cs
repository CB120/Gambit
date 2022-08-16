using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    int particleRepeatRate;
    [SerializeField] private ParticleSystem wind;
    private float particleHeight;
    private void Start()
    {
        particleHeight = Random.Range(6.5f, 9.5f);
        particleRepeatRate = Random.Range(9, 14);
        InvokeRepeating("SpawnWind", 1, particleRepeatRate);
    }
    private void SpawnWind()
    {
        particleRepeatRate = Random.Range(4, 9);
        Cell cellToSpawnOn = GridController.GetRandomCell();
        if (cellToSpawnOn != null)
        {
            Instantiate(wind, new Vector3(cellToSpawnOn.transform.position.x, particleHeight, cellToSpawnOn.transform.position.z), Quaternion.Euler(-180f, -87f, -180f));
        }
    }
}
