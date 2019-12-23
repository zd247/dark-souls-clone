using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
    public class AnimatorHook : MonoBehaviour {

        Animator anim;
        StateManager states;
        EnemyStates eStates;
        Rigidbody rigid;


        public float rm_multi;
        bool rolling;
        float roll_t;
        float delta;
        AnimationCurve roll_curve;

        HandleIK ik_handler;
        public bool useIK;
        public AvatarIKGoal currentHand;

        public bool killDelta;

        //-----------------------------------------------------------------------

        public void Init(StateManager st, EnemyStates eSt) {
            states = st;
            eStates = eSt;

            if (st != null) {
                anim = st.anim;
                rigid = st.rigid;
                roll_curve = states.roll_curve;
                delta = states.delta;
            }
            if (eSt != null) {
                anim = eSt.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;
            }

            ik_handler = gameObject.GetComponent<HandleIK>();
            if (ik_handler != null)
                ik_handler.Init(anim);

        }

        public void InitForRoll() {
            rolling = true;
            roll_t = 0;

        }

        public void CloseRoll() {
            if (rolling == false)
                return;

            rm_multi = 1;
            rolling = false;
        }

        void OnAnimatorMove() {
            if (ik_handler != null)
            {
                ik_handler.OnAnimatorMoveTick((currentHand == AvatarIKGoal.LeftHand));
            }

            if (states == null && eStates == null)
                return;

            if (rigid == null)
                return;


            // => because when inAction is true, canMove is false, so when its true, exit the function.
            if (states != null) {
                if (states.onEmpty)
                    return;
                delta = states.delta;
            }

            if (eStates != null) {
                if (eStates.canMove)
                    return;
                delta = eStates.delta;
            }

            //there will be no drag when animation is played.
            rigid.drag = 0;

            // for better inAction based animations and calculations.
            if (rm_multi == 0)
                rm_multi = 1;


            //Handle physics factors base on active state.
            if (rolling == false) {
                
                //Gets the avatar delta position for the last evaluated frame.
                Vector3 delta2 = anim.deltaPosition;
                if (killDelta)
                {
                    killDelta = false;
                    delta2 = Vector3.zero;
                }

                Vector3 v = (delta2 * rm_multi) / delta;


                v.y = rigid.velocity.y;

                if (eStates)
                    eStates.agent.velocity = v;
                else
                    rigid.velocity = v;
            }
            else { // when rolling = true.
                roll_t += delta / 0.6f;
                if (roll_t > 1) {  
                    roll_t = 1;
                }

                if (states == null)
                    return;


                float zValue = states.roll_curve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_multi);

                v2.y = rigid.velocity.y;
                rigid.constraints = RigidbodyConstraints.FreezePositionY;
                rigid.velocity = v2 * 3.2f;
            }
        }

        void OnAnimatorIK() {
            if (ik_handler == null)
                return;

            if (!useIK)
            {
                if (ik_handler.weight > 0)
                {
                    ik_handler.IKTick(currentHand, 0);
                }
                else {
                    ik_handler.weight = 0;
                }
            }
            else {
                ik_handler.IKTick(currentHand, 1);
            }

        }

        void LateUpdate()
        {
            if (ik_handler != null)
                ik_handler.LateTick();
        }

        public void OpenAttack() {
            if (states)
                states.canAttack = true;
        }

        public void OpenCanMove() {
            if (states) {
                states.canMove = true;
            }
        }

        public void OpenDamageColliders() {
            if (states)
                states.inventoryManager.OpenAllDamageColliders();
            if (eStates)
                eStates.OpenDamageCollier();

            OpenParryFlag();
        }

        public void CloseDamageColliders() {
            if (states)
            {
                states.inventoryManager.CloseAllDamageColliders();
            }
            if (eStates)
            {
                eStates.CloseDamageCollider();
            }
            CloseParryFlag();
        }

        //parry flag
        public void OpenParryFlag() {
            if (states) {
                states.parryIsOn = true;
            }

            if (eStates) {
                eStates.parryIsOn = true;
            }

        }

        public void CloseParryFlag() {
            if (states) {
                states.parryIsOn = false;
            }

            if (eStates) {
                eStates.parryIsOn = false;
            }
        }

        //parry colliders
        public void OpenParryCollider() {
            if (states == null)
                return;

            states.inventoryManager.OpenParryCollider();
        }

        public void CloseParryCollider() {
            if (states == null)
                return;

            states.inventoryManager.CloseParryCollider();
        }

        //Spell particles

        public void CloseParticle() {
            if (states) {
                if (states.inventoryManager.currentSpell.currentParticle != null)
                    states.inventoryManager.currentSpell.currentParticle.SetActive(false);
            }
        }

        public void InitiateThrowForProjectile() {
            if (states) {
                states.ThrowProjectile();
            }
        }

        //IK

        public void InitIKForShield(bool isLeft) {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.shield_l : IKSnapShotType.shield_r, isLeft);
        }

        public void InitIKForBreathSpell(bool isLeft) {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.breath_l : IKSnapShotType.breath_r, isLeft);
        }

        public void OpenRotationControl()
        {
            if (states)
            {
                states.canRotate = true;
            }
            if (eStates)
                eStates.rotateToTarget = true;

        }

        public void CloseRotationControl() {
            if (states)
                states.canRotate = false;
            if (eStates)
                eStates.rotateToTarget = false;
        }


        //Consumables
        public void ConsumeCurrentItem()
        {
            if (states)
                if (states.inventoryManager.curConsumable)
                {
                    states.inventoryManager.curConsumable.itemCount--;
                    ItemEffectManager.singleton.CastEffect(states.inventoryManager.curConsumable.instance.consumableEffect, states);
                }
        }

        //Sound Effect
        public void PlaySoundEffect() {
            if (states) {
                states.audio_source.PlayOneShot(states.audio_clip);
            }
            if (eStates) {

            }
        }
	}
}
