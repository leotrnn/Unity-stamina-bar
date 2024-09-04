/**
 * Triano Léo
 * 
 * 
 * basé sur https://www.youtube.com/watch?v=rTfCZcLsS9s&list=PLS7jk2aVN8G6s4KM7TV0EJz8_04KquJbX
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rigidbody;
    public Transform head;
    public Camera camera;

    [Header("Configurations")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpSpeed;

    [Header("Runtime")]
    Vector3 newVelocity;
    bool isGrounded = false;
    bool isJumping = false;

    [Header("Stamina")]
    public Image staminaBar;
    public float stamina, maxStamina;

    public float runCost;

    private Coroutine recharge;
    public float chargeRate;

    void Start()
    {
        Cursor.visible = false;

        // bloque la souris au centre de l'écran
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * 2f);


        // Garde la vélocité y en rejetant la vélocité x et z
        newVelocity = Vector3.up * rigidbody.velocity.y;

        float speed = walkSpeed;
        Debug.Log("stamina : " + stamina);

        if (stamina > 0)
        {
            // Si on maintient shift, la vitesse prend celle de run sinon walk
            speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            if (speed == runSpeed)
            {
                stamina -= speed == runSpeed ? runCost * Time.deltaTime : 0;

                if (stamina < 0)
                {
                    stamina = 0;
                }
                staminaBar.fillAmount = stamina / maxStamina;

                if(recharge != null) StopCoroutine(recharge);
                recharge = StartCoroutine(RechargeStamina());
            }
        }       

        // Horizontal prend les touches A et D
        newVelocity.x = Input.GetAxis("Horizontal") * speed;

        // Vertical prend les touches A et S
        newVelocity.z = Input.GetAxis("Vertical") * speed;

        if (isGrounded)
            if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
            {
                newVelocity.y = jumpSpeed;
                isJumping = true;
            }

        // transform.TransformDirection = applique les directions par rapport a l'angle de la caméra
        rigidbody.velocity = transform.TransformDirection(newVelocity);

    }

    private void FixedUpdate()
    {

    }

    private void LateUpdate()
    {
        Vector3 e = head.eulerAngles;

        e.x -= Input.GetAxis("Mouse Y") * 2f;
        e.x = RestrictAngle(e.x, -85f, 85f);
        head.eulerAngles = e;
    }

    // Prévient la caméra de faire des angles pas réalistes
    public static float RestrictAngle(float angle, float angleMin, float angleMax)
    {
        if(angle > 180)
            angle -= 360;
        else if(angle < -180)
            angle += 360;

        if(angle > angleMax)
            angle = angleMax;
        if ( angle < angleMin)
            angle = angleMin;
        

        return angle;
    }

    private void OnCollisionStay(Collision col)
    {
        isGrounded = true;
        isJumping = false;
    }

    void OnCollisionExit(Collision col)
    {
        isGrounded = false;
    }


    private IEnumerator RechargeStamina()
    {
        // attendre 1s
        yield return new WaitForSeconds(1f);
        while(stamina < maxStamina)
        {
            stamina += chargeRate / 10f;
            if(stamina > maxStamina)
            {
                stamina = maxStamina;
            }

            staminaBar.fillAmount = stamina / maxStamina;
            yield return new WaitForSeconds(.1f);
        }
    }
}
