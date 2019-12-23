using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class SpellEffectsManager : MonoBehaviour
    {
        Dictionary<string, int> s_effects = new Dictionary<string, int>();

        public void UseSpellEffect(string id, StateManager states, EnemyStates eStates = null) {
            int index = GetEffect(id);

            if (index == -1)
            {
                Debug.Log("Spell effect doesnt exist");
                return;
            }
                

            switch (index) {
                case 0:
                    FireBreath(states);
                    break;
                case 1:
                    DarkShield(states);
                    break;
                case 2:
                    HealingSmall(states);
                    break;
                case 3:
                    Fireball(states);
                    break;
                case 4:
                    OnFire(states, eStates);
                    break;
            }
        }

        int GetEffect(string id) {
            int index = -1;

            if (s_effects.TryGetValue(id, out index)) {
                return index;
            }

            return index;
        }

        void FireBreath(StateManager states) {
            states.spellCast_start = states.inventoryManager.OpenBreathCollider;

            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
            states.spellCast_loop += states.SubstractFocusOverTime;

            states.spellCast_stop = states.inventoryManager.CloseBreathCollider;
        }

        void DarkShield(StateManager states)
        {
            states.spellCast_start = states.inventoryManager.OpenBlockCollider;

            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
            states.spellCast_loop += states.SubstractFocusOverTime;
            states.spellCast_loop += states.EffectBlocking;

            states.spellCast_stop = states.inventoryManager.CloseBlockCollider;
            states.spellCast_stop += states.StopEffectBlocking;

        }

        void HealingSmall(StateManager states) {
            states.spellCast_loop = states.AddHealth;
        }

        void Fireball(StateManager states)
        {
            states.spellCast_loop = states.inventoryManager.EmitSpellParticle;
        }

        void OnFire(StateManager states, EnemyStates eStates) {
            if (states != null) {

            }

            if (eStates != null) {
                eStates.spellEffect_loop = eStates.OnFire;
            }
        }

        public static SpellEffectsManager singleton;
        void Awake()
        {
            singleton = this;

            s_effects.Add("firebreath", 0);
            s_effects.Add("darkshield", 1);
            s_effects.Add("healingSmall", 2);
            s_effects.Add("fireball", 3);
            s_effects.Add("onFire", 4);

        }


    }
}

