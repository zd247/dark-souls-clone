using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
    public class InventoryManager : MonoBehaviour {
        public List<string> rh_weapons = new List<string>();
        public List<string> lh_weapons = new List<string>();
        public List<string> spell_items = new List<string>();
        public List<string> consumable_items = new List<string>();

        public int r_index;
        public int l_index;
        public int s_index;
        public int c_index;

        // Get from Resources
        public List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
        public List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
        public List<RuntimeSpellItems> r_spells = new List<RuntimeSpellItems>();
        public List<RuntimeConsumable> r_consum = new List<RuntimeConsumable>();

        // Realtime usage
        public RuntimeConsumable curConsumable;
        public RuntimeSpellItems currentSpell;
        public RuntimeWeapon rightHandWeapon; // new gameObject that going to be created based on the list of string that contains weapons' ids
        public RuntimeWeapon leftHandWeapon;
        public bool hasLeftHandWeapon = true;


        public GameObject parryCollider;
        public GameObject breathCollider;
        public GameObject blockCollider;

        StateManager states;


        public void Init(StateManager st) {

            states = st;
            UI.QuickSlot.singleton.Init();

            LoadInventory();

            // Parry Collider Init
            ParryCollider pr = parryCollider.GetComponent<ParryCollider>();
            pr.InitPlayer(states);
            CloseParryCollider();
            CloseBreathCollider();
            CloseBlockCollider();

        }




        //*********************** HEY HEY HEY (I)
        //When you first started the game, r_index & l_index will be 0, so EquipWeapon will detect the first name element in the List<string> rh_weapons or List<string> lh_weapons
        //*********************** HEY HEY HEY
        public void LoadInventory()
        {
            //____________Weapons_Init______________
            for (int i = 0; i < rh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(rh_weapons[i]));
            }
            for (int i = 0; i < lh_weapons.Count; i++)
            {
                WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(lh_weapons[i]), true);
            }

            if (r_r_weapons.Count > 0)
            {
                if (r_index > r_r_weapons.Count - 1)
                    r_index = 0;

                // Passing values from List<RuntimeWeapon> to RuntimeWeapon reference variable bases on r_index (1)
                rightHandWeapon = r_r_weapons[r_index];
            }

            if (r_l_weapons.Count > 0)
            {
                if (l_index > r_l_weapons.Count - 1)
                    l_index = 0;

                // Passing values from List<RuntimeWeapon> to RuntimeWeapon reference variable bases on l_index (1)
                leftHandWeapon = r_l_weapons[l_index];
            }

            // Initial Equip
            if (rightHandWeapon != null)
                EquipWeapon(rightHandWeapon, false);
            if (leftHandWeapon != null)
                EquipWeapon(leftHandWeapon, true);

            hasLeftHandWeapon = (leftHandWeapon != null);


            //____________Spells_Init___________________
            for (int i = 0; i < spell_items.Count; i++)
            {
                SpellToRuntimeSpell(ResourceManager.singleton.GetSpell(spell_items[i]));
            }

            if (r_spells.Count > 0)
            {
                if (s_index > r_spells.Count - 1)
                    s_index = 0;

                EquipSpells(r_spells[s_index]);
            }

            //____________Consumables_Init______________
            for (int i = 0; i < consumable_items.Count; i++)
            {
                ConsumableToRuntimeConsumable(ResourceManager.singleton.GetConsumable(consumable_items[i]));
            }

            if (r_consum.Count > 0)
            {
                if (c_index > r_consum.Count - 1)
                    c_index = 0;

                EquipConsumable(r_consum[c_index]);
            }

            //______________Colliders_Init_______________
            InitAllDamageColliders(states);
            CloseAllDamageColliders();
        }

        //--------------------------Weapons--------------------------------------

        public Weapon GetCurrentWeapon(bool isLeft)
        {
            if (isLeft)
                return leftHandWeapon.instance;
            else
                return rightHandWeapon.instance;
        }

        public void ChangeToNextWeapon(bool isLeft)
        {
            //***************** HEY HEY HEY (II)
            // At this point, l_index & r_index is still = 0, but after this function is called, their values are incremented. Hence the next weapon_name in the List will be detected.
            //***************** HEY HEY HEY

            if (isLeft)
            {
                if (l_index < r_l_weapons.Count - 1)
                    l_index++;
                else
                    l_index = 0;
                //(3)
                EquipWeapon(r_l_weapons[l_index], true);


            }
            else
            {
                if (r_index < r_r_weapons.Count - 1)
                    r_index++;
                else
                    r_index = 0;
                //(3)
                EquipWeapon(r_r_weapons[r_index]);
            }
            states.actionManager.UpdateActionsOneHanded();
        }

        public void EquipWeapon(RuntimeWeapon w, bool isLeft = false) {
            if (isLeft) {
                if (leftHandWeapon != null) {
                    leftHandWeapon.weaponModel.SetActive(false);

                }
                //(2)
                leftHandWeapon = w;
            }
            else
            {
                if (rightHandWeapon != null) {
                    rightHandWeapon.weaponModel.SetActive(false);
                }
                //(2)
                rightHandWeapon = w;
            }

            string targetIdle = w.instance.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";

            if (isLeft == true) {
                for (int i = 0; i < leftHandWeapon.instance.actions.Count; i++)
                {
                    leftHandWeapon.instance.actions[i].mirror = true;
                }
            }

            states.anim.SetBool("mirror", isLeft);
            states.anim.Play("changeWeapon");
            states.anim.Play(targetIdle);

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);

            w.weaponModel.SetActive(true);
        }

        public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false) {
            GameObject gO = new GameObject();
            gO.AddComponent<RuntimeWeapon>();
            RuntimeWeapon inst = gO.GetComponent<RuntimeWeapon>();

            inst.instance = new Weapon();
            StaticFunctions.DeepCopyWeapon(w, inst.instance);
            gO.name = w.itemName;

            inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.weaponModel.transform.parent = p;


            inst.weaponModel.transform.localPosition =
                (isLeft) ? inst.instance.l_model_pos : inst.instance.r_model_pos;
            inst.weaponModel.transform.localEulerAngles =
                (isLeft) ? inst.instance.l_model_eulers : inst.instance.r_model_eulers;
            inst.weaponModel.transform.localScale = inst.instance.model_scale;

            inst.w_hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
            inst.w_hook.InitDamageColliders(states);

            if (isLeft)
                r_l_weapons.Add(inst);
            else
                r_r_weapons.Add(inst);

            // After populate the RuntimeWeapon, turn off the Instantiated weaponModel and let EquipWeapon handling turning on/off the weaponModel.
            inst.weaponModel.SetActive(false);
            return inst;
        }

        //--------------------------Spells--------------------------------------
        public void ChangeToNextSpell() {
            if (s_index < r_spells.Count - 1)
                s_index++;
            else
                s_index = 0;
            EquipSpells(r_spells[s_index]);
        }

        public void EquipSpells(RuntimeSpellItems spell)
        {
            currentSpell = spell;

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(UI.QSlotType.spell, spell.instance.icon);
        }

        public RuntimeSpellItems SpellToRuntimeSpell(Spell s, bool isLeft = false)
        {
            GameObject g0 = new GameObject();
            RuntimeSpellItems inst = g0.AddComponent<RuntimeSpellItems>();

            inst.instance = new Spell();
            StaticFunctions.DeepCopySpell(s, inst.instance);
            g0.name = s.itemName;

            

            r_spells.Add(inst);

            return inst;
        }

        public void CreateSpellParticle(RuntimeSpellItems inst, bool isLeft, bool parentUnderRoot = false) {
            if (inst.currentParticle == null)
            {
                inst.currentParticle = Instantiate(inst.instance.particle_prefab) as GameObject;
                inst.p_hook = inst.currentParticle.GetComponentInChildren<ParticleHook>();
                inst.p_hook.Init();
            }
                

            if (!parentUnderRoot)
            {
                Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
                inst.currentParticle.transform.parent = p;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
                inst.currentParticle.transform.localPosition = Vector3.zero;
                
            }
            else {
                inst.currentParticle.transform.parent = transform;
                inst.currentParticle.transform.localRotation = Quaternion.identity;
                inst.currentParticle.transform.localPosition = new Vector3(0, 1.5f, 0.65f);
            }

            //inst.currentParticle.SetActive(false);
        }

        //------------------------Consumables-----------------------------------

        public RuntimeConsumable ConsumableToRuntimeConsumable(Consumable c) {
            GameObject g0 = new GameObject();
            RuntimeConsumable inst = g0.AddComponent<RuntimeConsumable>();
            g0.name = c.itemName;

            inst.instance = new Consumable();
            StaticFunctions.DeepCopyConsumable(inst.instance, c);

            if (inst.instance.itemPrefab != null) {
                GameObject model = Instantiate(inst.instance.itemPrefab) as GameObject;
                Transform p = states.anim.GetBoneTransform(HumanBodyBones.RightHand);
                model.transform.parent = p;
                model.transform.localPosition = inst.instance.r_model_pos;
                model.transform.localEulerAngles = inst.instance.r_model_eulers;

                Vector3 targetScale = inst.instance.model_scale;
                if (targetScale == Vector3.zero)
                    targetScale = Vector3.one;
                model.transform.localScale = targetScale;

                inst.itemModel = model;
                inst.itemModel.SetActive(false);

            }
            r_consum.Add(inst);
            return inst;
        }

        public void EquipConsumable(RuntimeConsumable consum)
        {
            curConsumable = consum;

            UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
            uiSlot.UpdateSlot(UI.QSlotType.item, consum.instance.icon);
        }

        public void ChangeToNextConsumable()
        {
            if (c_index < r_consum.Count - 1)
                c_index++;
            else
                c_index = 0;

            EquipConsumable(r_consum[c_index]);
        }


        // ----------------------Colliders---------------------------------------

        //__________Weapons_____________
        public void OpenAllDamageColliders()
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.OpenDamageColliders();
            if (hasLeftHandWeapon)
            {
                if (leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.OpenDamageColliders();
            }

        }

        public void CloseAllDamageColliders()
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.CloseDamageColliders();

            if (hasLeftHandWeapon)
            {
                if (leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.CloseDamageColliders();
            }

        }

        public void InitAllDamageColliders(StateManager states)
        {
            if (rightHandWeapon.w_hook != null)
                rightHandWeapon.w_hook.InitDamageColliders(states);

            if (hasLeftHandWeapon)
            {
                if (leftHandWeapon.w_hook != null)
                    leftHandWeapon.w_hook.InitDamageColliders(states);
            }

        }

        //___________Parry_________________

        public void OpenParryCollider()
        {
            parryCollider.SetActive(true);
        }

        public void CloseParryCollider()
        {
            parryCollider.SetActive(false);
        }

        //_________Spells__________________
        #region Delegate Calls
        public void OpenBreathCollider()
        {
            breathCollider.SetActive(true);
        }

        public void CloseBreathCollider()
        {
            breathCollider.SetActive(false);
        }

        public void OpenBlockCollider()
        {
            blockCollider.SetActive(true);
        }

        public void CloseBlockCollider()
        {
            blockCollider.SetActive(false);
        }

        public void EmitSpellParticle() {
            currentSpell.p_hook.Emit(1);
        }
        #endregion
    }

    //------------------------------------Classes------------------------------------------------

    [System.Serializable]
    public class Item
    {
        public string itemName;
        public string itemDescription;
        public Sprite icon;
    }

    [System.Serializable]
    public class Weapon : Item
    {

        public string oh_idle;
        public string th_idle;
        public ItemType itemType;

        public List<Action> actions;
        public List<Action> two_handedActions;

        public float parryMultiplier;
        public float backstabMultiplier;
        public bool LeftHandMirror;

        public GameObject modelPrefab;

        public WeaponStats weaponStats;

        public Action GetAction(List<Action> l, ActionInput inp)
        {
            if (l == null)
                return null;

            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }
            return null;
        }

        public Vector3 r_model_pos;
        public Vector3 l_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 l_model_eulers;
        public Vector3 model_scale;
    }

    [System.Serializable]
    public class Spell : Item
    {
        public SpellType spellType;
        public SpellClass spellClass;
        public ItemType itemType;

        public List<SpellAction> actions = new List<SpellAction>();
        public GameObject projectile;
        public GameObject particle_prefab;
        public string spell_effect;

        public SpellAction GetAction(List<SpellAction> l, ActionInput inp)
        {
            if (l == null)
                return null;

            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].input == inp)
                {
                    return l[i];
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class Consumable : Item {
        public string consumableEffect;
        public string audio_id;
        public string targetAnim;
        public ItemType itemType;

        public GameObject itemPrefab;
        public Vector3 r_model_pos;
        public Vector3 r_model_eulers;
        public Vector3 model_scale;
    }

    public enum ItemType {
        weapon, item, spell
    }

    

}
