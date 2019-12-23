using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
    public class ActionManager : MonoBehaviour {
        public int actionIndex;
        public List<Action> actionSlots = new List<Action>();

        StateManager states;

        //***********Declaration*****************


        // Populate ActionSlots by going through the list of enums and asign the corresponded action with that selected enum
        ActionManager() {
            for (int i = 0; i < 4; i++) {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }

        }

        public void Init(StateManager st) {
            states = st;
            UpdateActionsOneHanded(); //Only update at the start of the game, therefore in InventoryManager/EquipWeapon needs to call this everytime the User changes Weapon.
        }

        void EmptyAllSlots() {
            for (int i = 0; i < 4; i++) {
                Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
                a.targetAnim = null;
                a.audio_ids = null;
                a.steps = null;
                a.mirror = false;
                a.type = ActionType.attack;
                a.canBeParried = true;
                a.changeSpeed = true;
                a.animSpeed = 1;
                a.canBackStab = false;
            }
        }

        // public targetAnim setter
        public void UpdateActionsOneHanded() {
            EmptyAllSlots();

            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);

            if (states.inventoryManager.hasLeftHandWeapon) {
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlots, true);
                StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlots, true);
            } else {
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlots);
                StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlots);
            }

            /*if (states.inventoryManager.hasLeftHandWeapon) {
				UpdateActionsWithLeftHand ();
				return;
			}
			Weapon w = states.inventoryManager.rightHandWeapon;

			for (int i = 0; i < w.actions.Count; i++) {
				// a represents actionSlot's slot.
				Action a = GetAction(w.actions[i].input);
				// assign ActionSlot's targetAnim with targetAnim get from InventoryManager.cs
				a.targetAnim = w.actions[i].targetAnim;
			}*/
        }

        /*public void UpdateActionsWithLeftHand(){
			/*Weapon r_w = states.inventoryManager.rightHandWeapon;
			Weapon l_w = states.inventoryManager.leftHandWeapon;

			Action rb = GetAction (ActionInput.rb);
			Action rt = GetAction (ActionInput.rt);

			Action w_rb = r_w.GetAction (r_w.actions, ActionInput.rb);
			rb.targetAnim = w_rb.targetAnim;
			rb.type = w_rb.type;
			rb.canBeParried = w_rb.canBeParried;
			rb.changeSpeed = w_rb.changeSpeed;
			rb.animSpeed = w_rb.animSpeed;

			Action w_rt = r_w.GetAction (r_w.actions, ActionInput.rt);
			rt.targetAnim = w_rt.targetAnim;
			rt.type = w_rt.type;
			rt.canBeParried = w_rt.canBeParried;
			rt.changeSpeed = w_rt.changeSpeed;
			rt.animSpeed = w_rt.animSpeed;


			Action lb = GetAction (ActionInput.lb);
			Action lt = GetAction (ActionInput.lt);

			Action w_lb = l_w.GetAction (l_w.actions, ActionInput.rb);
			lb.targetAnim = w_lb.targetAnim;
			lb.type = w_lb.type;
			lb.canBeParried = w_lb.canBeParried;
			lb.changeSpeed = w_lb.changeSpeed;
			lb.animSpeed = w_lb.animSpeed;

			Action w_lt = l_w.GetAction (l_w.actions, ActionInput.rt);
			lt.targetAnim = w_lt.targetAnim;
			lt.type = w_lt.type;
			lt.canBeParried = w_lt.canBeParried;
			lt.changeSpeed = w_lt.changeSpeed;
			lt.animSpeed = w_lt.animSpeed;

			if (l_w.LeftHandMirror) {
				lb.mirror = true;
				lt.mirror = true;
			}
		}*/

        // public targetAnim setter.
        public void UpdateActionsTwoHanded() {
            EmptyAllSlots();
            Weapon w = states.inventoryManager.rightHandWeapon.instance;

            for (int i = 0; i < w.two_handedActions.Count; i++) {
                Action a = StaticFunctions.GetAction(w.two_handedActions[i].input, actionSlots);
                // assign ActionSlot's targetAnim with targetAnim get from InventoryManager.cs
                a.steps = w.two_handedActions[i].steps;
                a.type = w.two_handedActions[i].type;
            }
        }

       

        // public input setter
        public ActionInput GetActionInput(StateManager st) {

            if (st.rb)
                return ActionInput.rb;
            if (st.lb)
                return ActionInput.lb;
            if (st.rt)
                return ActionInput.rt;
            if (st.lt)
                return ActionInput.lt;

            return ActionInput.rb;

        }

		// public Getter
		public Action GetActionSlot(StateManager st){
			ActionInput a_input = GetActionInput (st);
			// return ActionSlots corresponding to the input get from states
			return StaticFunctions.GetAction (a_input, actionSlots);
		}

        public Action GetActionFromInput(ActionInput a_input) {
            return StaticFunctions.GetAction(a_input, actionSlots);
        }
	}

    public enum ActionInput
    {
        rb, lb, rt, lt
    }

    public enum ActionType
    {
        attack, block, spells, parry
    }

    public enum SpellClass
    {
        pyromancy, miracles, sorcery
    }

    public enum SpellType
    {
        projectile, buff, looping
    }

    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public ActionType type;
        public SpellClass spellClass;
        public string targetAnim;
        public string audio_ids;
        public List<ActionSteps> steps;
        public bool mirror = false;
        public bool canBeParried = true;
        public bool changeSpeed = false;
        public float animSpeed = 1;
        public bool canParry = false;
        public bool canBackStab = false;
        public float staminaCost;
        public float fpCost;

        public ActionSteps GetActionStep(ref int indx)
        {
            if (steps.Count > 0) {
                if (indx > steps.Count - 1)
                    indx = 0;
                ActionSteps retVal = steps[indx];

                if (indx > steps.Count - 1)
                    indx = 0;
                else
                    indx++;

                return retVal;
            }
            return null;
            
        }

        [HideInInspector]
        public float parryMultiplier;
        [HideInInspector]
        public float backstabMultiplier;

        public bool overrideDamageAnim;
        public string damageAnim;
    }

    [System.Serializable]
    public class ActionSteps
    {
        public List<ActionAnim> branches = new List<ActionAnim>();

        public ActionAnim GetBranch(ActionInput inp)
        {
            for (int i = 0; i < branches.Count; i++)
            {
                if (branches[i].input == inp)
                    return branches[i];
            }

            return branches[0];
        }
    }

    [System.Serializable]
    public class ActionAnim
    {
        public ActionInput input;
        public string targetAnim;
        public string audio_ids;
    }

    [System.Serializable]
    public class SpellAction
    {
        public ActionInput input;
        public string targetAnim;
        public string throwAnim;
        public float castTime;
        public float focusCost;
        public float staminaCost;
    }
}

/*NOTE TO SELF:
 	The process of passing animations to actionSlots[].targetAnim:
 		- Update "Input": In StateManager: match the "state.actions.input" with "action.actions.input" (targetAnim == null)
 		- Populate "TargetAnim": In InventoryManger: populate targetAnim by hand (hard-coding) in the Inspector window.
 		- Update "TargetAnim": In ActionManager: UpdateActionsOneHanded + UpdateActionsTwoHanded:
 			=> states.actions[i].targetAnim = inventory.actions[i].targetAnim = action.actions[i].targetAnim;
 		

 */

