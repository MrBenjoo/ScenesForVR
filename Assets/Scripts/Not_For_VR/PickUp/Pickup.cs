﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup: MonoBehaviour
{
    public Transform onHand;

    void OnMouseDown()
    {
        GetComponent<Rigidbody>().useGravity = false;
        this.transform.position = onHand.transform.position;
        this.transform.parent = GameObject.Find("PlayerCamera").transform;
    }

    void OnMouseUp()
    {
        this.transform.parent = null;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
