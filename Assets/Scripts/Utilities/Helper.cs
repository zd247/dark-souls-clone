using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class Helper : MonoBehaviour {
		[Range(-1,1)] public float vertical;
		[Range(-1,1)] public float horizontal;


		public string animName;
		public bool playAnim;
		public bool useItem;
		public bool interacting;
		public bool lockon;

		public bool twoHanded;
		public bool enableRM;

		public string[] oh_attacks;
		public string[] th_attacks;

		Animator anim;
		void Start () {
			anim = GetComponent<Animator> ();
		}



		void Update () {
			enableRM = !anim.GetBool ("canMove");
			anim.applyRootMotion = enableRM;

			interacting = anim.GetBool ("interacting");


			if (enableRM)
				return;


			if (useItem) {
				anim.Play("use_item");
				useItem = false;
			}
			if (interacting) {
				playAnim = false;
				vertical = Mathf.Clamp (vertical, 0, 0.5f);
			}


			if (!lockon) {
				horizontal = 0;
				vertical = Mathf.Clamp01(vertical);
			}
			anim.SetBool ("lockon", lockon);
				

			anim.SetBool ("two_handed", twoHanded);


			// if playAnim is pressed
			if (playAnim) {
				string targetAnim;
				if (!twoHanded) {
					int r = Random.Range (0, oh_attacks.Length);
					targetAnim = oh_attacks [r];
					if (vertical > 0.5)
						targetAnim = "oh_attack_3";
				} else {
					int r = Random.Range (0, oh_attacks.Length);
					targetAnim = th_attacks [r];
					if (vertical > 0.5)
						targetAnim = "oh_attack_3";
				}

				vertical = 0;

				//play animation based on the name.
				anim.CrossFade (targetAnim, 0.2f);
				playAnim = false;
			}
			// linker
			anim.SetFloat ("vertical", vertical);
			anim.SetFloat ("horizontal", horizontal);

		}
	}
}

