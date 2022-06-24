using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private bool _isInHitRange;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // var _fighter = other.GetComponent<Fighter>();
            // var _body = other.GetComponent<Rigidbody>();
            _isInHitRange = true;
            //KnockBack(_fighter, _body);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInHitRange = false;
        }
    }

    private void KnockBack(Fighter fighter, Rigidbody rigidBody)
    {
        float knockBackForce = 1000f;
        if (fighter.CheckDirection() && fighter.CheckIfPlayerOne())
        {
            rigidBody.AddForce(Vector3.left * knockBackForce);
        }
        else if (fighter.CheckDirection() && !fighter.CheckIfPlayerOne())
        {
            rigidBody.AddForce(Vector3.right * knockBackForce);
        }
        else if (!fighter.CheckDirection() && fighter.CheckIfPlayerOne())
        {
            rigidBody.AddForce(Vector3.right * knockBackForce);
        }
        else if (!fighter.CheckDirection() && !fighter.CheckIfPlayerOne())
        {
            rigidBody.AddForce(Vector3.left * knockBackForce);
        }
    }

    public bool CanHit()
    {
        return _isInHitRange;
    }
}