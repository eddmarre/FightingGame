using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] private Fighter myFighter;

    private void OnTriggerEnter(Collider other)
    {
        myFighter.GroundCheck();
    }

    private void OnTriggerExit(Collider other)
    {
        myFighter.GroundCheck();
    }
}