using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimuliRayCasting : MonoBehaviour
{
    /* RayCasting Parameters */
    public float castRadius;            // set the radius for the sphere when using SphereCasting
    public float maxDistance;           // reach threshold for the casting
    public LayerMask mask;              // only the object specified by the mask (in inspector) will be detected by the RayCasting.
    public GameObject currentHitObject; // object hit by beam
    private Vector3 origin;             // the start position of the beam
    private Vector3 direction;          // direction of the beam
    private float currentHitDistance;   // distance from origin to the hit object

    /* Miscellanous Stuff */
    public ShaderBlink blink;           // has method for turning off the blinking effect on the stimuli
    public float targetTime;            // used only for testing...
    private Vector3 stimuliPosition;    // used only for testing...
    public GameObject target;
    public Vector3 targetVector;
    private GameObject player;

    float gazeDuration = 0f;
    private float s1 = 0f;
    private bool onObjectGaze = false;


    void Start()
    {
        player = GameObject.Find("PlayerCamera");
    }

    /* Update is called once per frame */
    void Update()
    {
        updateRayCastVector();
        updateMaxDistance();
        targetTime -= Time.deltaTime;   // used only for testing...

        rayCast();                    // tiny laser beams
        //sphereCast();                   // wider laser beams
    }

    private void updateRayCastVector()
    {
        origin = transform.position;
        direction = transform.forward;
    }

    private void updateMaxDistance()
    {
        maxDistance = Vector3.Distance(player.transform.position, target.transform.position);
    }


    private void rayCast()
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 10, mask))
        {
            OnObjectHit(hitInfo);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            OnObjectNotHit();
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100);
        }
    }

    private void OnObjectHit(RaycastHit hitInfo)
    {
        currentHitObject = hitInfo.transform.gameObject;
        currentHitDistance = hitInfo.distance;

        if (currentHitObject.CompareTag("Stimuli"))
        {
            Debug.Log("if (currentHitObject.CompareTagStimuli)");
            if(!onObjectGaze)
            {
                Debug.Log("if(!onObjectGaze)");
                onObjectGaze = true;
                s1 = Time.time;
                
            }
            //stimuliPosition = hitInfo.transform.position;
            //currentHitObject.SetActive(false);
            //blink.turnOffBlink();
        } 
    }

    private void OnObjectNotHit()
    {
        currentHitDistance = maxDistance;
        /*if (targetTime <= 0.0f)
        {
            //currentHitObject = null;
            //currentHitObject.SetActive(true);
            blink.turnOnBlink();
        }*/

        if (onObjectGaze)
        {
            onObjectGaze = false;
            gazeDuration += Time.time - s1;

            Debug.Log(gazeDuration);
        }
    }


    private void sphereCast()
    {
        RaycastHit hitInfo;

        if (Physics.SphereCast(origin, castRadius, direction, out hitInfo, maxDistance, mask))
            OnObjectHit(hitInfo);
        else
            OnObjectNotHit();
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + direction * currentHitDistance);
        Gizmos.DrawWireSphere(origin + direction * currentHitDistance, castRadius);
    }
    */

}
