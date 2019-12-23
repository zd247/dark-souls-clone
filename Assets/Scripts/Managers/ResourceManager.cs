using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {
    public class ResourceManager : MonoBehaviour
    {
        //The dictionary only contains weapons' name.
        Dictionary<string, int> weapon_ids = new Dictionary<string, int>();
        Dictionary<string, int> spell_ids = new Dictionary<string, int>();
        Dictionary<string, int> consum_ids = new Dictionary<string, int>();
        Dictionary<string, int> item_ids = new Dictionary<string, int>();
        Dictionary<string, int> interaction_ids = new Dictionary<string, int>();
        Dictionary<string, int> npc_ids = new Dictionary<string, int>();
        Dictionary<string, int> audio_ids = new Dictionary<string, int>();

        //singleton
        public static ResourceManager singleton;
        void Awake() {
            singleton = this;

            LoadWeaponIds();
            LoadSpellIds();
            LoadConsumables();
            LoadItems();
            LoadInteractions();
            LoadNpcs();
            LoadAudios();
        }

        //--------------------------Weapons-----------------------------

        //Check if whether the Dictionary has the weapon in the system or not, if not Add the weapon's name to the dictionary (Weapon is derived from the ScriptableWeaponObject)
        // Populates the Dictionary<string> with item_name in the Resource.
        void LoadWeaponIds() {
            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.WeaponScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.weapons_all.Count; i++) {
                if (weapon_ids.ContainsKey(obj.weapons_all[i].itemName)){
                    Debug.Log("Weapon item is a duplicate");
                }
                else {
                    weapon_ids.Add(obj.weapons_all[i].itemName, i);
                }
            }
        }

        
        int GetWeaponIdfromString(string name) {// Get Item's position in the List<Weapon> in the dictionary.
            int index = -1;


            if (weapon_ids.TryGetValue(name, out index)) {
                return index;
            }

            return -1;
        }

        
        public Weapon GetWeapon(string name) {// Return a Weapon from the Resource with the similar name (id).
            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.WeaponScriptableObject could not be loaded!");
                return null;
            }

            int index = GetWeaponIdfromString(name);

            if (index == -1)
                return null;

            return obj.weapons_all[index];

            
            /*int index = -1;
            if (weapon_dict.TryGetValue(id, out index))
            {

                return weaponList[index];
            }

            return null;*/
            
        }

        // --------------------------Spells-----------------------------
        void LoadSpellIds()
        {
            SpellItemScriptableObject obj = Resources.Load("SA.SpellItemScriptableObject") as SpellItemScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.SpellItemScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.spell_items.Count; i++)
            {
                if (spell_ids.ContainsKey(obj.spell_items[i].itemName))
                {
                    Debug.Log("Spell item is a duplicate");
                }
                else
                {
                    spell_ids.Add(obj.spell_items[i].itemName, i);
                }
            }
        }

        int GetSpellIdFromString(string name) {
            int index = -1;

            if (spell_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;
            
        }

        public Spell GetSpell(string name) {
            SpellItemScriptableObject obj = Resources.Load("SA.SpellItemScriptableObject") as SpellItemScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.SpellItemScriptableObject could not be loaded!");
                return null;
            }

            int index = GetSpellIdFromString(name);
            if (index == -1)
                return null;

            return obj.spell_items[index];
        }

        // -------------------------Consumables--------------------------

        void LoadConsumables() {
            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.ConsumablesScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.consumables.Count; i++)
            {
                if (consum_ids.ContainsKey(obj.consumables[i].itemName))
                {
                    Debug.Log("Consumable item is a duplicate");
                }
                else
                {
                    consum_ids.Add(obj.consumables[i].itemName, i);
                }
            }
        }

        int GetConsumableIdFromString(string name)
        {
            int index = -1;

            if (consum_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;

        }

        public Consumable GetConsumable(string name)
        {
            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.ConsumablesScriptableObject could not be loaded!");
                return null;
            }

            int index = GetConsumableIdFromString(name);
            if (index == -1)
                return null;

            return obj.consumables[index];
        }

        //--------------------------Items---------------------------------

        void LoadItems()
        {
            ItemScriptableObject obj = Resources.Load("SA.ItemScriptableObject") as ItemScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.ItemScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.items.Count; i++)
            {
                if (item_ids.ContainsKey(obj.items[i].itemName))
                {
                    Debug.Log("Item is a duplicate");
                }
                else
                {
                    item_ids.Add(obj.items[i].itemName, i);
                }
            }
        }

        int GetItemIdFromString(string name)
        {
            int index = -1;

            if (item_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;

        }

        public Item GetItem(string name)
        {
            ItemScriptableObject obj = Resources.Load("SA.ItemScriptableObject") as ItemScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.ItemScriptableObject could not be loaded!");
                return null;
            }

            int index = GetItemIdFromString(name);
            if (index == -1)
                return null;

            return obj.items[index];
        }

        //------------------------Interactions-----------------------------

        void LoadInteractions()
        {
            InteractionScriptableObject obj = Resources.Load("SA.InteractionScriptableObject") as InteractionScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.InteractionScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.interactions.Count; i++)
            {
                if (interaction_ids.ContainsKey(obj.interactions[i].interactionId))
                {
                    Debug.Log("Interaction is a duplicate");
                }
                else
                {
                    interaction_ids.Add(obj.interactions[i].interactionId, i);
                }
            }
        }

        int GetInteractionIdFromString(string name)
        {
            int index = -1;

            if (interaction_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;

        }

        public Interactions GetInteraction(string name)
        {
            InteractionScriptableObject obj = Resources.Load("SA.InteractionScriptableObject") as InteractionScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.InteractionScriptableObject could not be loaded!");
                return null;
            }

            int index = GetInteractionIdFromString(name);
            if (index == -1)
                return null;

            return obj.interactions[index];
        }

        //------------------------NPCs-----------------------------

        void LoadNpcs()
        {
            NPCScriptableObject obj = Resources.Load("SA.NPCScriptableObject") as NPCScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.NPCScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.npcs.Length; i++)
            {
                if (npc_ids.ContainsKey(obj.npcs[i].npc_id))
                {
                    Debug.Log("NPC is a duplicate");
                }
                else
                {
                    npc_ids.Add(obj.npcs[i].npc_id, i);
                }
            }
        }

        int GetNPCfromString(string name)
        {
            int index = -1;

            if (npc_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;

        }

        public NPCDialogue GetNPCDialogue(string name)
        {
            NPCScriptableObject obj = Resources.Load("SA.NPCScriptableObject") as NPCScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.NPCScriptableObject could not be loaded!");
                return null;
            }

            int index = GetNPCfromString(name);
            if (index == -1)
                return null;

            return obj.npcs[index];
        }

        //------------------------Audios-----------------------------

        void LoadAudios()
        {
            AudioScriptableObject obj = Resources.Load("SA.AudioScriptableObject") as AudioScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.AudioScriptableObject could not be loaded!");
                return;
            }

            for (int i = 0; i < obj.audio_list.Count; i++)
            {
                if (audio_ids.ContainsKey(obj.audio_list[i].id))
                {
                    Debug.Log("Audio is a duplicate");
                }
                else
                {
                    audio_ids.Add(obj.audio_list[i].id, i);
                }
            }
        }

        int GetAudioFromString(string name)
        {
            int index = -1;

            if (audio_ids.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;

        }

        public Audio GetAudio(string name)
        {
            AudioScriptableObject obj = Resources.Load("SA.AudioScriptableObject") as AudioScriptableObject;

            if (obj == null)
            {
                Debug.Log("SA.AudioScriptableObject could not be loaded!");
                return null;
            }

            int index = GetAudioFromString(name);
            if (index == -1)
                return null;

            return obj.audio_list[index];
        }
    }
}

