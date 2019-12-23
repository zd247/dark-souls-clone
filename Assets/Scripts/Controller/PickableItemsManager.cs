using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class PickableItemsManager : MonoBehaviour
    {
        public List<WorldInteraction> interactions = new List<WorldInteraction>();
        public List<PickableItem> pick_items = new List<PickableItem>();
        public PickableItem itemCandidate;
        public WorldInteraction interactionCandidate;

        int frameCount;
        public int frameCheck = 15;

        public void Tick() {
            if (frameCount < frameCheck) {
                frameCount++;
                return;
            }
            frameCount = 0;

            for (int i = 0; i < pick_items.Count; i++)
            {
                float distance = Vector3.Distance(pick_items[i].transform.position, transform.position);

                if (distance < 2)
                {
                    itemCandidate = pick_items[i];
                }
                else {
                    if (itemCandidate == pick_items[i])
                        itemCandidate = null;
                }
            }

            for (int i = 0; i < interactions.Count; i++)
            {
                float d = Vector3.Distance(interactions[i].transform.position, transform.position);
                if (d < 2)
                {
                    interactionCandidate = interactions[i];
                }
                else {
                    if (interactionCandidate == interactions[i])
                        interactionCandidate = null;
                }
            }
        }

        public void PickCandidate(StateManager states) {
            if (itemCandidate == null)
                return;
            
            for (int i = 0; i < itemCandidate.items.Length; i++)
            {
                PickItemContainer c = itemCandidate.items[i];
                
                AddItem(c.itemId, c.itemType, states);
            }

            if (pick_items.Contains(itemCandidate))
                pick_items.Remove(itemCandidate);


            Destroy(itemCandidate.gameObject);
            itemCandidate = null;
        }

        void AddItem(string id, ItemType type, StateManager states) {
            InventoryManager inv = states.inventoryManager;
            switch (type)
            {
                case ItemType.weapon:
                    for (int k = 0; k < inv.r_r_weapons.Count; k++)
                    {
                        if (id == inv.r_r_weapons[k].name) {
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(id));
                    inv.WeaponToRuntimeWeapon(ResourceManager.singleton.GetWeapon(id), true);
                    break;
                case ItemType.item:
                    for (int j = 0; j < inv.r_consum.Count; j++) {
                        if (id == inv.r_consum[j].name)
                        {
                            inv.r_consum[j].itemCount++;
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.ConsumableToRuntimeConsumable(ResourceManager.singleton.GetConsumable(id));
                    break;
                case ItemType.spell:
                    for (int k = 0; k < inv.r_spells.Count; k++)
                    {
                        if (id == inv.r_spells[k].name)
                        {
                            Item b = ResourceManager.singleton.GetItem(id);
                            UIManager.singleton.AddAnnounceCard(b);
                            return;
                        }
                    }
                    inv.SpellToRuntimeSpell(ResourceManager.singleton.GetSpell(id));
                    break;
            }

            Item i = ResourceManager.singleton.GetItem(id);
            UIManager.singleton.AddAnnounceCard(i);
            
        }
    }
}

