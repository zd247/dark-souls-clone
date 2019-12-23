using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
	public class DamageCollider : MonoBehaviour {
        StateManager states;
        EnemyStates eStates;

        public void InitPlayer(StateManager st) {
            states = st;
            gameObject.layer = 9;
            gameObject.SetActive(false);
        }

        public void InitEnemy(EnemyStates st) {
            eStates = st;
            gameObject.layer = 9;
            gameObject.SetActive(false);
        }

		void OnTriggerEnter(Collider other){
            if (states) {
                EnemyStates es = other.transform.GetComponentInParent<EnemyStates>();

                if (es != null)
                {
                    // do damage
                    es.DoDamage();
                }
                return;
            }

            if (eStates)
            {
                StateManager st = other.transform.GetComponentInParent<StateManager>();

                if (st != null)
                {
                    st.DoDamage(eStates.GetCurrentAttack());
                }
                return;
            }
            

			

		}

	}
}

