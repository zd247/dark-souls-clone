using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
	public class ParryCollider : MonoBehaviour {
		StateManager states;
		EnemyStates eStates;

		public float maxTimer = 0.6f;
		float timer;

		public void InitPlayer(StateManager st){
			states = st;
		}
			
		public void InitEnemy(EnemyStates eSt){
			eStates = eSt;
		}

		void Update(){
			if (states) {
				timer += states.delta;

				if (timer > maxTimer) {
					timer = 0;
					gameObject.SetActive (false);
				}
			}
		}

		void OnTriggerEnter (Collider other){
//			DamageCollider dc = other.GetComponent<DamageCollider> ();
//			if (dc == null)
//				return;

			if (states) {
				EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates> ();

				if (e_st != null) {
					e_st.CheckForParry (transform.root, states);
				}
			}

			if (eStates) {
				//check for player
			}

		}

	}
}

