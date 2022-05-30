using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticleController : MonoBehaviour
{
    Transform splatHolder;
    public GameObject splatPrefab;

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
    ParticleSystem particle;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particle, other, collisionEvents);
        int count = collisionEvents.Count;
        for (int i = 0; i < count; i++)
        {
            Instantiate(splatPrefab, collisionEvents[i].intersection, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360f)), splatHolder);
        }
    }
}
