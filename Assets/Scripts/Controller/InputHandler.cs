using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA{
	public class InputHandler : MonoBehaviour {
		float vertical;
		float horizontal;
		bool b_input;
		bool a_input;
		bool x_input;
		bool y_input;

		bool rb_input;
		bool lb_input;

		float rt_axis;
		bool rt_input;

		float lt_axis;
		bool lt_input;

        float d_y;
        float d_x;

        bool d_up;
        bool d_down;
        bool d_right;
        bool d_left;

        bool p_d_up;
        bool p_d_down;
        bool p_d_left;
        bool p_d_right;

		bool leftAxis_down;
		bool rightAxis_down;

		float b_timer;
		float rt_timer;
		float lt_timer;
        float close_timer = 0;
        float a_input_count = 1.5f;

		StateManager states;
		CameraManager camManager;
        UIManager uiManager;
        DialogueManager dialogueManager;

		float delta;


		//-----------------------------------------------------------------------

		void Start () {
			states = GetComponent<StateManager> ();
			states.Init ();

			camManager = CameraManager.singleton;
			camManager.Init (states);

            uiManager = UIManager.singleton;

            dialogueManager = DialogueManager.singleton;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;


        }

        // intitialize the movement and camera functionalities
        void FixedUpdate () {
			delta = Time.fixedDeltaTime;
			GetInput ();
			UpdateStates ();
			states.FixedTick (delta);
			camManager.Tick (delta);
        }

        bool preferItem;

		void Update(){
			delta = Time.deltaTime;
            if (a_input)
                a_input_count ++;
            Debug.Log(delta);
            states.Tick (delta);
            if (!dialogueManager.dialogueActive)
            {
                if (states.pickManager.itemCandidate != null || states.pickManager.interactionCandidate != null)
                {
                    if (states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                    {
                        if (preferItem)
                        {
                            PickupItem();
                        }
                        else
                            Interact();
                    }

                    if (states.pickManager.itemCandidate && !states.pickManager.interactionCandidate)
                    {
                        
                        PickupItem();
                    }

                    if (!states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                    {
                        Interact();
                    }
                }
                else
                {
                    uiManager.CloseInteractCard();
                    if (uiManager.announceCard[0].gameObject.activeSelf == true 
                        || uiManager.announceCard[1].gameObject.activeSelf == true 
                        || uiManager.announceCard[2].gameObject.activeSelf == true 
                        || uiManager.announceCard[3].gameObject.activeSelf == true
                        || uiManager.announceCard[4].gameObject.activeSelf == true)
                        close_timer += 1;
                    if (close_timer > 190)
                    {
                        close_timer = 0;
                        uiManager.CloseAnnounceCard();
                        a_input = false;
                    }
                   
                }
            }
            else {
                uiManager.CloseInteractCard();
               
            }

            if (a_input_count > 1f)
            {
                a_input = false;
                a_input_count = 0;
            }
           
            
            dialogueManager.Tick(a_input);
            states.MonitorStats();
            ResetInputNState();
            uiManager.Tick(states.characterStats, delta, states);
            


        }

        void PickupItem() {
            uiManager.OpenInteractCard(UIActionType.pickup);
            if (a_input)
            {
                Vector3 targetDir = states.pickManager.itemCandidate.transform.position - transform.position;
                states.SnapToRotation(targetDir);
                states.pickManager.PickCandidate(states);
                states.PlayAnimation("pick_up");
                a_input = false;
            }
        }

        void Interact() {
            uiManager.OpenInteractCard(states.pickManager.interactionCandidate.actionType);
            if (a_input)
            {
                states.audio_source.PlayOneShot(ResourceManager.singleton.GetAudio("interact").audio_clip);
                states.InteractLogic();
                a_input = false;
            }
        }


        void GetInput(){
			vertical = Input.GetAxis (StaticStrings.Vertical);
			horizontal = Input.GetAxis (StaticStrings.Horizontal);

			b_input = Input.GetButton (StaticStrings.B);
			a_input = Input.GetButton ("A");
			x_input = Input.GetButton (StaticStrings.X);
			y_input = Input.GetButtonUp (StaticStrings.Y);

			rb_input = Input.GetButton(StaticStrings.RB);
			lb_input = Input.GetButton (StaticStrings.LB);

			rt_input = Input.GetButton (StaticStrings.RT);
			rt_axis = Input.GetAxis (StaticStrings.RT);

            if (rt_axis != 0)
				rt_input = true;

			lt_input = Input.GetButton (StaticStrings.LT);
			lt_axis = Input.GetAxis (StaticStrings.LT);
			if (lt_axis != 0)
				lt_input = true;

			rightAxis_down = Input.GetButtonUp (StaticStrings.L);

			if (b_input)
				b_timer += delta;

            d_x = Input.GetAxis("Pad X");
            d_y = Input.GetAxis("Pad Y");

            d_up = Input.GetKeyUp(KeyCode.Alpha1) || d_y > 0;
            d_down = Input.GetKeyUp(KeyCode.Alpha2) || d_y < 0;
            d_left = Input.GetKeyUp(KeyCode.Alpha3) || d_x < 0;
            d_right = Input.GetKeyUp(KeyCode.Alpha4) || d_x > 0;

            if (d_y > 0) {

            }

        }

		// passing values to StateManager variables and functions.
		void UpdateStates(){
			states.vertical = vertical;
			states.horizontal = horizontal;

			states.itemInput = x_input;
			states.rt = rt_input;
			states.lt = lt_input;
			states.rb = rb_input;
			states.lb = lb_input;


			// moveDir
			Vector3 v = states.vertical * camManager.transform.forward;
			Vector3 h = states.horizontal * camManager.transform.right;
			states.moveDir = (v + h).normalized;

			// moveAmount
			float m = Mathf.Abs (states.horizontal) + Mathf.Abs (states.vertical);
			states.moveAmount = Mathf.Clamp01 (m);

			// B_input: 
			if (b_input && b_timer > 0.5f) {// run when holding down.
				states.run = (states.moveAmount > 0) && states.characterStats._stamina > 0;
			}
			/* roll when tap the button b_input, thus b_input must equal false at the following FixedFrame because b_input == false when not the button for it is released.
			 and the timer at that following FixedFrame still equals to the last calculated value before being set back to 0 in the next FixedFrame after this one. */
			if (b_input == false && b_timer > 0 && b_timer < 0.5f)
				states.rollInput = true;

			if (y_input) {
                if (states.pickManager.itemCandidate && states.pickManager.interactionCandidate)
                {
                    preferItem = !preferItem;
                }
                else {
                    states.isTwoHanded = !states.isTwoHanded;
                    states.HandleTwoHanded();
                }
			}

            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockOn = false;
                    camManager.lockOnTarget = null;
                }
            }
            else {
                states.lockOn = false;
                states.lockOnTarget = null;
                states.lockOnTransform = null;
                camManager.lockOn = false;
                camManager.lockOnTarget = null;
            }


			if (rightAxis_down) {
				states.lockOn = !states.lockOn;
                states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);
                if (states.lockOnTarget == null)
				    states.lockOn = false;

				camManager.lockOnTarget = states.lockOnTarget;
                // when there's no multiple targets, lockOnTransform will be the target's transform position. (6)
                states.lockOnTransform = states.lockOnTarget.GetTarget();
				camManager.lockOnTransform = states.lockOnTransform ;
				// same state in two different managers should be the same
				camManager.lockOn = states.lockOn;
			}


			if (x_input)
				b_input = false;

            HandleQuickSlotChanges();

        }

        void HandleQuickSlotChanges() {
            if (states.isSpellCasting || states.usingItem)
                return;

            if (d_up)
            {
                if (!p_d_up)
                {
                    p_d_up = true;
                    states.inventoryManager.ChangeToNextSpell();
                }
            }

            if (!d_up)
                p_d_up = false;

            if (d_down)
            {
                if (!p_d_down)
                {
                    p_d_down = true;
                    states.inventoryManager.ChangeToNextConsumable();
                }
            }

            if (!d_up)
                p_d_down = false;

            if (states.onEmpty == false)
                return;

            if (states.isTwoHanded)
                return;

            if (d_left)
            {
                if (!p_d_left)
                {
                    states.inventoryManager.ChangeToNextWeapon(true);
                    p_d_left = true;
                }
            }
            if (d_right)
            {
                if (!p_d_right)
                {
                    states.inventoryManager.ChangeToNextWeapon(false);
                    p_d_right = true;
                }
            }

            
            if (!d_down)
                p_d_down = false;
            if (!d_left)
                p_d_left = false;
            if (!d_right)
                p_d_right = false;
        }

		void ResetInputNState(){
			// reset b_timer when b_input is released from keyboard
			if (b_input == false)
				b_timer = 0;
			// turn off rollInput and run state after being pressed.
			if (states.rollInput)
				states.rollInput = false;
			if (states.run)
				states.run = false;
		}
			  

	}
}

// NOTE TO SELF:
 /*
  * 3rd person movement formula:
    v.moveDirection = v.Vertical input) * v.Camera Direction
 	h.moveDirection = h.Vertical Input * h.Camera Direction.
 	Vector3 moveDirection = (v+h).normalize;
 */
