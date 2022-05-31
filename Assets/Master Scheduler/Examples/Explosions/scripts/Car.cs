using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    public bool bUseQueue = true;
    public float explosionRadius = 200f;
    public float explosionPower = 500f;
    public bool bShouldExplode = false;

    [HideInInspector]
    public bool bHasExploded = false;
    Transform selfTransform;
    Rigidbody rb;
    Car tmpCar;

    // Use this for initialization
    void Awake()
    {
        selfTransform = transform;
        rb = GetComponent<Rigidbody>();
        
    }

    void Start()
    {
        if (bShouldExplode)
        {
            //If we're here then this car is the first to explode, which makes this a good place to create our explosion queue
            if(bUseQueue)
            QueueHub.CreateQueue("Explosion", 12, false);

            //delay the initial explosion
            StartCoroutine(InitialExplosion());
        }
    }

    //tells the car to not receive explosion requests
    public void Tag()
    {
        //this flag must be set immediatly to avoid queueing the explosion job multiple times
        bHasExploded = true;        
    }

    public void Explode()
    {
        if (rb != null)
            rb.AddExplosionForce(explosionPower, selfTransform.position + (Random.insideUnitSphere * 3), explosionRadius, 3);
        else Debug.Log("rb is null");
        Collider[] col = Physics.OverlapSphere(selfTransform.position, explosionRadius);
        foreach(Collider hit in col)
        {
            tmpCar = hit.GetComponent<Car>();
            if(tmpCar != null && !tmpCar.bHasExploded)
            {
                tmpCar.Tag();

                //join the explosion queue
                if(bUseQueue)
                    tmpCar.RequestExplosion();
                
                //execute the explosion immediatly
                else
                    tmpCar.Explode();
            }
        }
    }

    public void RequestExplosion()
    {
        QueueHub.AddJobToQueue("Explosion", gameObject, Explode);
    }

    IEnumerator InitialExplosion()
    {
        yield return new WaitForSeconds(1.5f);
        Explode();
    }
}
