using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    //public AudioSource CoinSound;
    private AmmoSpawner ammoSpawner;
    private GameManager gameManager;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(90*Time.deltaTime, 0, 0);

        //needs fixing...
        gameManager.updateAmmoSlider();
    }

    private void OnTriggerEnter(Collider other)
    {
        //PointsManager.instance.AddPoint();
        //CoinSound.Play();
        gameManager.EnableShooting();
        RemoveAmmo();
    }

    private void RemoveAmmo()
    {
        Destroy (gameObject);
        ammoSpawner = GameObject.Find("CoinSpawner").GetComponent<AmmoSpawner>();
        ammoSpawner.SpawnAmmo();
    }
}
