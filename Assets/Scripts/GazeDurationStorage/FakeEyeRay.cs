using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class FakeEyeRay: MonoBehaviour
{
    public float maxDistance;           // reach threshold for the casting
    public LayerMask mask;              // only the object specified by the mask (in inspector) will be detected by the RayCasting.

    /* Miscellanous Stuff */
    public GameObject target;
    private GameObject player;
    private Ray ray;
    private SaveGazeDuration saveGazeDuration; 

    void Start()
    {
        saveGazeDuration = gameObject.GetComponent<SaveGazeDuration>();
        player = GameObject.Find("PlayerCamera");
        ray = new Ray();
    }

    void Update()
    {
        rayCast();                   
    }

    private void rayCast()
    {
        maxDistance = Vector3.Distance(player.transform.position, target.transform.position);
        ray.direction = transform.forward;
        ray.origin = transform.position;

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 10, mask))
        {
            saveGazeDuration.onObjectGazeFixation(hitInfo.transform.gameObject);
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
        }
        else
        {
            saveGazeDuration.onObjectGazeFixationEnd();
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100);
        }
    }


}
