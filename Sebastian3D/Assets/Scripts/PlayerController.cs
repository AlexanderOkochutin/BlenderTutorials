using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float walkSpeed = 2;
	public float runSpeed = 6;
	public float gravity = -12;
	public float jumpHeight = 1;
	[Range(0,1)]
	public float airContentPercent;

	public float turnSmoothTime = 0.2f;
	float turnSmootVelocity;

	public float speedSmoothTime = 0.1f;
	float speedSmoothVelocity;
	float currentSpeed;
	float velocityY;

	Animator animator;
	Transform cameraT;
	CharacterController characterController;
	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		cameraT = Camera.main.transform;
		characterController = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		Vector2 inputDir = input.normalized;

		if (Input.GetKeyDown (KeyCode.Space)) {
			Jump();
		}
		if (inputDir != Vector2.zero) {
			float targetRotation = Mathf.Atan2 (inputDir.x, inputDir.y) * Mathf.Rad2Deg  + cameraT.eulerAngles.y;
			transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmootVelocity, GetModifiedSmoothTime(turnSmoothTime));
		}

		bool running = Input.GetKey (KeyCode.LeftShift);
		float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		velocityY += Time.deltaTime * gravity;
		Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

		characterController.Move (velocity * Time.deltaTime);
		currentSpeed = new Vector2 (characterController.velocity.x, characterController.velocity.z).magnitude;

		if (characterController.isGrounded)
				velocityY = 0;

		float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
		animator.SetFloat ("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
	}
	void Jump(){
		if (characterController.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * gravity * jumpHeight);
			velocityY = jumpVelocity;
		}
	}

	float GetModifiedSmoothTime(float smoothTime){
		if(characterController.isGrounded){
			return smoothTime;
		}

		if (airContentPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / airContentPercent;
	}
}
