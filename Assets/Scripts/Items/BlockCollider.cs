using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class BlockCollider : MonoBehaviour
    {

        void OnTriggerEnter(Collider other)
        {
            DamageCollider dc = other.GetComponentInChildren<DamageCollider>();
            if (dc != null)
            {
                EnemyStates eStates = dc.GetComponentInParent<EnemyStates>();
                if (eStates != null)
                {
                    eStates.HandleBlocked();
                }
            }

        }
    }
}
