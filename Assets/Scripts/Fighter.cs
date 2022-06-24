using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Fighter : MonoBehaviour
{
    //vvvremove in favor of an action eventvvvv
    [SerializeField] private Fighter _otherFighter;
    [SerializeField] private HitDetection lightHitBox;

    [SerializeField] private float _movementSpeed = 20000f;
    [SerializeField] private float timeToWaitBetweenAttacks = .5f;
    [SerializeField] private float timeToSpawnLightHitBoxStart = .25f;
    [SerializeField] private float timeToSpawnLightHitBoxEnd = .25f;
    [SerializeField] private bool _isPlayerOne;
    [SerializeField] private bool isFacingRight = true;

    private Rigidbody _rigidbody;
    private Animator _animator;

    private float _jumpForce = 400f;
    private float _health = 100f;

    private bool _isTouchingGround = true;

    private Quaternion facingRight;
    private Quaternion facingLeft;


    private bool _isAttacking;
    private bool _isBlocking;
    private bool _canAttack = true;

    private WaitForSeconds _attackTimer;
    private WaitForSeconds _lightHitBoxTimerStart;
    private WaitForSeconds _lightHitBoxTimerEnd;

    private BaseCharacterState _currentState;
    private BaseCharacterState _blockState = new BlockState();

    public BaseCharacterState _regularState = new RegularState();

    public GameObject _otherHitBox;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _attackTimer = new WaitForSeconds(timeToWaitBetweenAttacks);
        _lightHitBoxTimerStart = new WaitForSeconds(timeToSpawnLightHitBoxStart);
        _lightHitBoxTimerEnd = new WaitForSeconds(timeToSpawnLightHitBoxEnd);

        lightHitBox.gameObject.SetActive(false);
    }

    private void Start()
    {
        InitPlayerDirections();
        _currentState = _regularState;
    }

    private void InitPlayerDirections()
    {
        facingRight = transform.rotation;
        transform.Rotate(Vector3.up, 180f);
        facingLeft = transform.rotation;
        transform.Rotate(Vector3.up, 180f);
    }

    private void Update()
    {
        _currentState.UpdateState(this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            _otherHitBox = other.gameObject;
            SwitchState(_blockState);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HitBox"))
        {
            Block(false);
            SwitchState(_regularState);
        }
    }

    public void BlockInput()
    {
        Block(true);
    }

    public void MovementInput()
    {
        HandleRunning();

        HandleJump();

        HandleCrouch();
    }

    private void HandleRunning()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //don't walk if in air
            _rigidbody.AddForce(Vector3.left * Time.deltaTime * _movementSpeed);
            if (isFacingRight)
            {
                //if enemy is attacking block
                SetWalkingBack(true);
            }

            if (!isFacingRight)
            {
                SetWalkingForward(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            if (isFacingRight)
                SetWalkingBack(false);
            if (!isFacingRight)
                SetWalkingForward(false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            //don't walk if in air
            _rigidbody.AddForce(Vector3.right * Time.deltaTime * _movementSpeed);
            if (isFacingRight)
            {
                SetWalkingForward(true);
            }

            if (!isFacingRight)
            {
                //if enemy is attacking block
                SetWalkingBack(true);
            }
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            if (isFacingRight)
                SetWalkingForward(false);
            if (!isFacingRight)
                SetWalkingBack(false);
        }
    }

    private void SetWalkingForward(bool value)
    {
        _animator.SetBool("isWalkingF", value);
    }

    private void SetWalkingBack(bool value)
    {
        _animator.SetBool("isWalkingB", value);
    }


    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Crouch(true);
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Crouch(false);
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!_isTouchingGround)
                return;
            _rigidbody.AddForce(Vector3.up * _jumpForce);
        }
    }

    public void AttackInput()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            //standing light attack
            Attack(true);
            //turn off standing L hit box
            _canAttack = false;
            StartCoroutine(WaitForAttackTimeDelay());
            //check range
            if (lightHitBox.CanHit())
            {
                _otherFighter.TakeDamage(10f);
            }
            //if is crouch crouched light attack
        }
    }

    private IEnumerator WaitForAttackTimeDelay()
    {
        yield return _attackTimer;
        Attack(false);
        _canAttack = true;
    }

    public void Block(bool value)
    {
        _isBlocking = value;

        _animator.SetBool("isBlocking", value);
    }

    public void Attack(bool value)
    {
        _isAttacking = value;

        if (!value)
        {
            return;
        }

        StartCoroutine(WaitForLightHitBoxTimeDelay());
        _animator.SetTrigger("isAttacking");
    }


    private IEnumerator WaitForLightHitBoxTimeDelay()
    {
        yield return _lightHitBoxTimerStart;
        lightHitBox.gameObject.SetActive(true);
        yield return _lightHitBoxTimerEnd;
        lightHitBox.gameObject.SetActive(false);
    }

    private void Crouch(bool value)
    {
        _animator.SetBool("isCrouching", value);
    }

    public void FaceRightWay(Fighter otherFighter)
    {
        var position = otherFighter.transform.position - transform.position;
        position.Normalize();
        if (position.x >= 0f)
        {
            isFacingRight = true;
            transform.rotation = facingRight;
        }
        else
        {
            isFacingRight = false;
            transform.rotation = facingLeft;
        }

        _animator.SetBool("isWalkingB", false);
        _animator.SetBool("isWalkingF", false);
    }

    //faces player 2 the correct way
    public void FaceLeftWay(Fighter otherFighter)
    {
        var position = otherFighter.transform.position - transform.position;
        position.Normalize();
        if (position.x >= 0f)
        {
            isFacingRight = false;
            transform.rotation = facingLeft;
        }
        else
        {
            isFacingRight = true;
            transform.rotation = facingRight;
        }

        _animator.SetBool("isWalkingB", false);
        _animator.SetBool("isWalkingF", false);
    }

    public void GroundCheck()
    {
        _isTouchingGround = !_isTouchingGround;
    }

    public void TakeDamage(float damageAmount)
    {
        _health -= damageAmount;
    }

    public void SetOtherFighter(Fighter otherFighter)
    {
        _otherFighter = otherFighter;
    }

    public void IsFacingLeft()
    {
        isFacingRight = false;
    }

    public void IsPlayerOne(bool value)
    {
        _isPlayerOne = value;
    }

    public void SwitchState(BaseCharacterState newState)
    {
        _currentState = newState;
    }

    public bool CheckDirection()
    {
        return isFacingRight;
    }

    public bool CheckIfPlayerOne()
    {
        return _isPlayerOne;
    }
}

public abstract class BaseCharacterState
{
    public abstract void UpdateState(Fighter fighter);
}

public class RegularState : BaseCharacterState
{
    public override void UpdateState(Fighter fighter)
    {
        if (fighter.CheckIfPlayerOne())
        {
            fighter.MovementInput();
            fighter.AttackInput();
        }
    }
}

public class BlockState : BaseCharacterState
{
    public override void UpdateState(Fighter fighter)
    {
        //block mechanic
        fighter.BlockInput();
        //if there isn't a hit box than don't react
        if (fighter._otherHitBox != null && !fighter._otherHitBox.activeSelf)
        {
            fighter.Block(false);
            fighter.SwitchState(fighter._regularState);
        }
    }
}