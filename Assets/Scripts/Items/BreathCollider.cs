using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class BreathCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.GetComponentInChildren<EnemyStates>();
            if (eStates != null) {
                eStates.DoDamageSpell();
                SpellEffectsManager.singleton.UseSpellEffect("onFire", null, eStates);
            }
        }

    }
}

