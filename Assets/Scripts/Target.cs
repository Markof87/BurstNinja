using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float minSpeed = 12f;
    public float maxSpeed = 16f;
    public float maxTorque = 10f;
    public float xRange = 4f;
    public float ySpawnPos = -6f;

    public int pointValue;

    public ParticleSystem explosionParticle;

    public AudioClip explosionClip;
    public AudioClip pointClip;
    public AudioClip sensorClip;

    private Rigidbody targetRb;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        targetRb = GetComponent<Rigidbody>();
        targetRb.AddForce(RandomUpwardForce(), ForceMode.Impulse);
        transform.position = RandomSpawnPos();

        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
    }

    Vector3 RandomUpwardForce()
    {
        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        return Vector3.up * randomSpeed;
    }

    Vector3 RandomSpawnPos()
    {
        float randomXPos = Random.Range(-xRange, xRange);
        return new Vector3(randomXPos, ySpawnPos);
    }

    float RandomTorque()
    {
        float randomTorque = Random.Range(0, maxTorque);
        return randomTorque;
    }

    private void OnMouseOver()
    {
        if (!gameManager.isPaused && gameManager.isClicking && gameManager.lives > 0)
        {
            gameManager.UpdateScore(pointValue * gameManager.difficultyGame);
            Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            AudioSource.PlayClipAtPoint(explosionClip, Camera.main.transform.position);
            AudioSource.PlayClipAtPoint(pointClip, Camera.main.transform.position);

            if (gameObject.CompareTag("Bad"))
                gameManager.UpdateLives();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sensor") && gameManager.lives > 0)
        {
            if (gameObject.CompareTag("Food"))
            {
                gameManager.UpdateLives();
                AudioSource.PlayClipAtPoint(sensorClip, Camera.main.transform.position);
            }

            Destroy(gameObject);
        }
    }
}
