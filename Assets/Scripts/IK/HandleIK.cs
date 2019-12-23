using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class HandleIK : MonoBehaviour
    {
        Animator anim;

        Transform handHelper;
        Transform bodyHelper;
        Transform headHelper;
        Transform shoulderHelper;
        Transform headTrans;

        public float weight;

        public IKSnapShot[] ikSnapShots;
        public Vector3 defaultHeadPos;

        IKSnapShot GetSnapShot(IKSnapShotType type) {
            for (int i = 0; i < ikSnapShots.Length; i++)
            {
                if (ikSnapShots[i].type == type) {
                    return ikSnapShots[i];
                }
            }
            return null;
        }
        
        public void Init(Animator a) {
            anim = a;

            headHelper = new GameObject().transform;
            headHelper.name = "head_helper";
            handHelper = new GameObject().transform;
            handHelper.name = "hand_helper";
            bodyHelper = new GameObject().transform;
            bodyHelper.name = "body_helper";
            shoulderHelper = new GameObject().transform;
            shoulderHelper.name = "shoulder_helper";

            shoulderHelper.parent = transform.parent;
            shoulderHelper.localPosition = Vector3.zero;
            shoulderHelper.localRotation = Quaternion.identity;
            headHelper.parent = shoulderHelper;
            bodyHelper.parent = shoulderHelper;
            handHelper.parent = shoulderHelper;

            headTrans = anim.GetBoneTransform(HumanBodyBones.Head);

            
        }

        public void UpdateIKTargets(IKSnapShotType type, bool isLeft) {
            IKSnapShot snap = GetSnapShot(type);
        
            handHelper.localPosition = snap.handPos;
            handHelper.localEulerAngles = snap.hand_eulers;
            bodyHelper.localPosition = snap.bodyPos;

            if (snap.overwriteHeadPos)
                headHelper.localPosition = snap.headPos;
            else
                headHelper.localPosition = defaultHeadPos;
        }

        public void IKTick(AvatarIKGoal goal, float w) {
            weight = Mathf.Lerp(weight, w, Time.deltaTime * 5);

            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);

            anim.SetIKPosition(goal, handHelper.position);
            anim.SetIKRotation(goal, handHelper.rotation);
            

            anim.SetLookAtWeight(weight, 0.8f, 1, 1, 1);
            anim.SetLookAtPosition(bodyHelper.position);
        }

        
        public void OnAnimatorMoveTick(bool isLeft) {
            Transform shoulder = anim.GetBoneTransform(
                (isLeft) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);

            shoulderHelper.transform.position = shoulder.position;

        }

        public void LateTick() {
            if (headTrans == null || headHelper == null)
                return;

            Vector3 direction = headHelper.position - headTrans.position;
            if (direction == Vector3.zero)
                direction = headTrans.forward;

            Quaternion targetRot = Quaternion.LookRotation(direction);
            Quaternion curRot = Quaternion.Slerp(headTrans.rotation, targetRot, weight);
            headTrans.rotation = curRot;
        }
    }

    public enum IKSnapShotType {
        breath_r, breath_l, shield_r, shield_l
    }


    [System.Serializable]
    public class IKSnapShot {
        public IKSnapShotType type;
        public Vector3 handPos;
        public Vector3 hand_eulers;
        public Vector3 bodyPos;

        public bool overwriteHeadPos;
        public Vector3 headPos;
        
    }
}

