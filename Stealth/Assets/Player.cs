using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnReachedEndOfLevel;

    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;
    Vector3 velocity;

    Rigidbody rigidbody;
    bool disabled;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //<----Stopt Player wanneer Enemy hem ziet 
        Enemy.OnEnemyHasSpottedPlayer += Disable;
        //---->
    }


    // Update is called once per frame
    void Update()
    {
        //<----Stopt Player wanneer Enemy hem ziet 
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
        }
        //---->
        //Vector3 inputDirection = new Vector3(Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized;
        float inputMagnitetude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitetude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2 (inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitetude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;

        //Niet meer nodig door Rigidbody
        //transform.eulerAngles = Vector3.up * angle;
        //transform.Translate(transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);
    }

    //<----Stopt Player wanneer Enemy hem ziet 

    private void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }

    void Disable()
    {
        disabled = true;
    }
    //---->

    private void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    //<----Stopt Player wanneer Enemy hem ziet 
    void OnDestroy()
    {
        Enemy.OnEnemyHasSpottedPlayer -= Disable;
    }
    //---->
}
