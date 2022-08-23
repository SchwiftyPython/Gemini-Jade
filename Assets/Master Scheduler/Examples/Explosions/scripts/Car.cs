using UnityEngine;
using System.Collections;

/// <summary>

/// The car class

/// </summary>

/// <seealso cref="MonoBehaviour"/>

public class Car : MonoBehaviour
{
    /// <summary>
    /// The use queue
    /// </summary>
    public bool bUseQueue = true;
    /// <summary>
    /// The explosion radius
    /// </summary>
    public float explosionRadius = 200f;
    /// <summary>
    /// The explosion power
    /// </summary>
    public float explosionPower = 500f;
    /// <summary>
    /// The should explode
    /// </summary>
    public bool bShouldExplode = false;

    /// <summary>
    /// The has exploded
    /// </summary>
    [HideInInspector]
    public bool bHasExploded = false;
    /// <summary>
    /// The self transform
    /// </summary>
    Transform selfTransform;
    /// <summary>
    /// The rb
    /// </summary>
    Rigidbody rb;
    /// <summary>
    /// The tmp car
    /// </summary>
    Car tmpCar;

    // Use this for initialization
    /// <summary>
    /// Awakes this instance
    /// </summary>
    void Awake()
    {
        selfTransform = transform;
        rb = GetComponent<Rigidbody>();
        
    }

    /// <summary>
    /// Starts this instance
    /// </summary>
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
    /// <summary>
    /// Tags this instance
    /// </summary>
    public void Tag()
    {
        //this flag must be set immediatly to avoid queueing the explosion job multiple times
        bHasExploded = true;        
    }

    /// <summary>
    /// Explodes this instance
    /// </summary>
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

    /// <summary>
    /// Requests the explosion
    /// </summary>
    public void RequestExplosion()
    {
        QueueHub.AddJobToQueue("Explosion", gameObject, Explode);
    }

    /// <summary>
    /// Initials the explosion
    /// </summary>
    /// <returns>The enumerator</returns>
    IEnumerator InitialExplosion()
    {
        yield return new WaitForSeconds(1.5f);
        Explode();
    }
}
