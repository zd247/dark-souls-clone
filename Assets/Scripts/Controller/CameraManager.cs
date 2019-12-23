using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class CameraManager : MonoBehaviour {
		
		[Header ("States")]
		public bool lockOn;

		[Header ("Stats")]
		public float followSpeed = 9;
		public float mouseSpeed = 2;
		public float turnSmoothing = .1f;
		public float minAngle = -35;
		public float maxAngle = 35;

		[Header ("Sources")]
		public Transform target;
		public EnemyTarget lockOnTarget;
		public Transform lockOnTransform;

		[Header ("Others")]
		public Vector3 targetDir;
		public float lookAngle;
		public float tiltAngle;	


		[HideInInspector]
		public Transform pivot;
		[HideInInspector]
		public Transform camTrans;
		StateManager states;

		float smoothX;
		float smoothY;
		float smoothXvelocity;
		float smoothYvelocity;


		bool usedRightAxis;

        bool changeTargetLeft;
        bool changeTargetRight;



		//-----------------------------------------------------------------------

		// assigner
		public void Init(StateManager st){
			states = st;
			target = st.transform;

			camTrans = Camera.main.transform;
			pivot = camTrans.parent;

		}

		public void Tick(float d){
			float h = Input.GetAxis ("Mouse X");
			float v = Input.GetAxis ("Mouse Y");

			float targetSpeed = mouseSpeed;

            changeTargetLeft = Input.GetKeyUp(KeyCode.V);
            changeTargetRight = Input.GetKeyUp(KeyCode.B);


			// handle target selection for the camera (lockOnState)
			if (lockOnTarget != null) {
				// assigning initial targets (1)
				if (lockOnTransform == null) {
					lockOnTransform = lockOnTarget.GetTarget ();
					states.lockOnTransform = lockOnTransform;
				}
				// switching between different targets. (2)
				//if (Mathf.Abs (h) > 0.8f) {
				//	 /*because usedRightAxis is always set to false when <0.8
				//	 also, because when we use the right axis, it jumps to (>0.8f) and then falls back down to 0 quickly
				//	thus it will reset the h value and usedRightAxis value to default
				//	 */
				//	if (!usedRightAxis){
				//		lockOnTransform = lockOnTarget.GetTarget ((h > 0));
				//		states.lockOnTransform = lockOnTransform;
				//		usedRightAxis = true;
				//	}
				//}
            

                if (changeTargetLeft || changeTargetRight) {
                    lockOnTransform = lockOnTarget.GetTarget(changeTargetLeft);
                    states.lockOnTransform = lockOnTransform;
                }
			}

			//// turn off usedRightAxis when it's falling back down to 0 (only when it is active).
			//if (usedRightAxis) {
			//	if (Mathf.Abs (h) < 0.8f)
			//		usedRightAxis = false;
			//}

			FollowTarget (d);
			HandleRotations (d, v, h, targetSpeed);
		}

		void FollowTarget(float d){
			float speed = d * followSpeed;
			//delay follow
			Vector3 targetPosition = Vector3.Lerp (transform.position, target.position, speed);
			transform.position = targetPosition;
		}

		void HandleRotations (float d, float v, float h, float targetSpeed){
			// smoothness -> smoothX
			if (turnSmoothing > 0) {
				smoothX = Mathf.SmoothDamp (smoothX, h, ref smoothXvelocity, turnSmoothing);
				smoothY = Mathf.SmoothDamp (smoothY, v, ref smoothYvelocity, turnSmoothing);
			} else {
				smoothX = h;
				smoothY = v;
			}

			// tiltness (tilt up and down)
			tiltAngle -= smoothY * targetSpeed;
			tiltAngle = Mathf.Clamp (tiltAngle, minAngle, maxAngle);
			pivot.localRotation = Quaternion.Euler (tiltAngle, 0, 0);

			// handle directions of the camera (lockOnState) (3)
			if (lockOn && lockOnTarget != null && states.run == false) {
				//offset
				targetDir = lockOnTransform.position - transform.position;
				targetDir.Normalize ();
				//targetDir.y = 0;

				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion targetRot = Quaternion.LookRotation (targetDir);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, d * 9);
				// so the lookAngle wont reset after exiting the lockOn mode.
				lookAngle = transform.eulerAngles.y;
				return;
			}

			// rotate mainCamera
			lookAngle += smoothX * targetSpeed;
			transform.rotation = Quaternion.Euler (0, lookAngle, 0);


		}
			
		public static CameraManager singleton;
		void Awake(){
			singleton = this;
		}
	}
}
