using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
	public class EnemyTarget : MonoBehaviour {
		public int index;
        public bool isLockOn;
        public List<Transform> targets = new List<Transform>();
		public List<HumanBodyBones> h_bones = new List<HumanBodyBones> ();

		public EnemyStates eStates;

		Animator anim;
		//-----------------------------------------------------------------------

		public void Init(EnemyStates st){
			eStates = st;
			anim = eStates.anim;
			if (anim.isHuman == false)
				return;

            // populate the targets[];
            for (int i = 0; i < h_bones.Count; i++)
            {
                targets.Add(anim.GetBoneTransform(h_bones[i]));
            }

            EnemyManager.singleton.enemyTargets.Add(this);
        }

		// needs  to be clarified. (4)
		public Transform GetTarget(bool negative = false){
			// if there's nothing in the List of targets, use the EnemyTarget's transform position.
			if (targets.Count == 0)
				return transform;
			// go through the list of targets.
			if (negative == false) { //when h > 0
				
				if (index < targets.Count - 1) {
					index++;
				} else {
					index = 0;
				}
			}
			//when (h < 0)
			else {
				if (index == 0) {
					index = targets.Count - 1;
				} else
					index--;
			}

			index = Mathf.Clamp (index, 0, targets.Count);

			return targets [index];
		}

	}
}
