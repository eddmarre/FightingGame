using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField] private Fighter player1, player2;
    [SerializeField] private Transform spawn1, spawn2;
    private Fighter _player1, _player2;

    private void Awake()
    {
    }

    private void Start()
    {
        _player1 = Instantiate(player1, spawn1.position, spawn1.rotation);
        _player1.name = "player1";
        _player2 = Instantiate(player2, spawn2.position, spawn2.rotation);
        _player2.name = "player2";

        _player1.SetOtherFighter(player2);
        _player2.SetOtherFighter(player1);

        _player1.IsPlayerOne(true);
        _player2.IsPlayerOne(false);

        _player2.IsFacingLeft();
    }

    private void Update()
    {
        _player1.FaceRightWay(_player2);
        _player2.FaceLeftWay(_player1);
        if (Input.GetMouseButtonDown(1))
        {
            _player2.GetComponent<Animator>().SetTrigger("isAttacking");
            try
            {
                _player2.Attack(true);
                StartCoroutine(resetAttackTimer());
            }
            catch (Exception e)
            {
            }
        }
    }

    IEnumerator resetAttackTimer()
    {
        yield return new WaitForSeconds(1f);
        _player2.Attack(false);
    }
}