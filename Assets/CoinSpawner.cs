using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public AudioSource CoinSound;
    public int objectCount = 30;
    
    public Vector3 center;
    public Vector3 size;
    public float spawnCollisionCheckRadius;

    private int loop = 0;

    // Start is called before the first frame update 

    void Start()
    {
        while (loop < objectCount)
        {
            Vector3 spawnPoint = center + new Vector3(Random.Range(-size.x /2 , size.x / 2), 0 ,Random.Range(-size.z /2 , size.z / 2));
            if (!Physics.CheckSphere(spawnPoint, spawnCollisionCheckRadius))
            {
                Instantiate(prefabToSpawn, spawnPoint, Quaternion.Euler(0, 0, 90)); 
                loop++;
            }
           
        }
        
    }

    public void SpawnCoin()
    {
        Vector3 spawnPoint = center + new Vector3(Random.Range(-size.x /2 , size.x / 2), 0 ,Random.Range(-size.z /2 , size.z / 2));

        if (!Physics.CheckSphere(spawnPoint, spawnCollisionCheckRadius))
        {
            Instantiate(prefabToSpawn, spawnPoint, Quaternion.Euler(0, 0, 90));
            CoinSound.Play();
        }
        else
        {
            SpawnCoin();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
