using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    //public AudioSource CoinSound;
    private CoinSpawner coinSpawner;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(90*Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        PointsManager.instance.AddPoint();
        //CoinSound.Play();
        RemoveCoin();
    }

    private void RemoveCoin()
    {
        Destroy (gameObject);
        coinSpawner = GameObject.Find("CoinSpawner").GetComponent<CoinSpawner>();
        coinSpawner.SpawnCoin();
    }
}
