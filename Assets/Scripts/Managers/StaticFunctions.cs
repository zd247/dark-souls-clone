using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {
    public static class StaticFunctions //For Database transfer in Inventory
    {
        //********************Weapons****************************
        //(1)
        public static void DeepCopyWeapon(Weapon from, Weapon to) {
            //Item
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;

            //Weapon
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;
            to.itemType = from.itemType;

            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                DeepCopyActionToAction(a, from.actions[i]);
                to.actions.Add(a);
            }

            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action a = new Action();
                DeepCopyActionToAction(a, from.two_handedActions[i]);
                to.two_handedActions.Add(a);
            }

            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.l_model_pos = from.l_model_pos;
            to.l_model_eulers = from.l_model_eulers;
            to.r_model_pos = from.r_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.model_scale = from.model_scale;
            to.weaponStats = new WeaponStats();
            DeepCopyWeaponStats(from.weaponStats, to.weaponStats);
        }
        // (1.1)
        public static void DeepCopyActionToAction(Action a, Action w_a) {
            a.input = w_a.input;
            a.targetAnim = w_a.targetAnim;
            a.audio_ids = w_a.audio_ids;
            a.type = w_a.type;
            a.spellClass = w_a.spellClass;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.canParry = w_a.canParry;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.staminaCost = w_a.staminaCost;
            a.fpCost = w_a.fpCost;

            DeepCopyStepsList(w_a, a);
        }

        public static void DeepCopyStepsList(Action from, Action to) {
            to.steps = new List<ActionSteps>();
            for (int i = 0; i < from.steps.Count; i++)
            {
                ActionSteps step = new ActionSteps();
                DeepCopySteps(from.steps[i], step);
                to.steps.Add(step);
            }
        }

        public static void DeepCopySteps(ActionSteps from, ActionSteps to) {
            to.branches = new List<ActionAnim>();
            for (int i = 0; i < from.branches.Count; i++)
            {
                ActionAnim a = new ActionAnim();
                a.input = from.branches[i].input;
                a.targetAnim = from.branches[i].targetAnim;
                a.audio_ids = from.branches[i].audio_ids;
                to.branches.Add(a);
            }
        }

        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lightning = from.lightning;
            to.fire = from.fire;
            to.dark = from.dark;
        }

        
        //***********************Spells***************************

        public static void DeepCopySpell(Spell from, Spell to) {
            //Item
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;

            //Spell
            to.itemType = from.itemType;
            to.spellType = from.spellType;
            to.spellClass = from.spellClass;
            to.projectile = from.projectile;
            to.spell_effect = from.spell_effect;
            to.particle_prefab = from.particle_prefab;

            to.actions = new List<SpellAction>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                SpellAction a = new SpellAction();
                DeepCopySpellAction(from.actions[i], a );
                to.actions.Add(a);
            }
        }

        public static void DeepCopySpellAction(SpellAction from, SpellAction to)
        {
            to.input = from.input;
            to.targetAnim = from.targetAnim;
            to.throwAnim = from.throwAnim;
            to.castTime = from.castTime;
            to.staminaCost = from.staminaCost;
            to.focusCost = from.focusCost;
        }

        //***********************Consumables***************************

        public static void DeepCopyConsumable(Consumable to, Consumable from) {
            to.itemName = from.itemName;
            to.icon = from.icon;
            to.itemDescription = from.itemDescription;

            to.itemType = from.itemType;
            to.consumableEffect = from.consumableEffect;
            to.targetAnim = from.targetAnim;
            to.audio_id = from.audio_id;
            to.itemPrefab = from.itemPrefab;
            to.model_scale = from.model_scale;
            to.r_model_eulers = from.r_model_eulers;
            to.r_model_pos = from.r_model_pos;

        }



        //----------------------------------------------For ActionManager to StateManager--------------------------------------------------------------------------
        //(2)
        public static void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assign, List<Action> actionList, bool isLeftHand = false)
        {
            Action a = GetAction(assign, actionList);
            Action w_a = w.GetAction(w.actions, inp);
            if (w_a == null)
                return;
            DeepCopyStepsList(w_a, a);
            a.type = w_a.type;
            a.targetAnim = w_a.targetAnim;
            a.audio_ids = w_a.audio_ids;
            a.spellClass = w_a.spellClass;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.canParry = w_a.canParry;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;
            a.staminaCost = w_a.staminaCost;
            a.fpCost = w_a.fpCost;

            if (isLeftHand)
            {
                a.mirror = true;
            }
        }

        // private Getter (Get actionSlots)
        public static Action GetAction(ActionInput inp, List<Action> actionSlots)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == inp)
                    return actionSlots[i];
            }
            return null;
        }


    }
}

