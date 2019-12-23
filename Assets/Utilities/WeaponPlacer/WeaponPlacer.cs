using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string itemName;

        public GameObject targetModel;

        public bool leftHand;
        public bool saveItem;

        public SaveType saveType;

        public enum SaveType {
            weapon, item
        }

        void Update()
        {
            if (!saveItem)
                return;
            saveItem = false;

            switch (saveType) {
                case SaveType.weapon:
                    SaveWeapon();
                    break;
                case SaveType.item:
                    SaveConsumable();
                    break;

            }
        }

        void SaveWeapon() {
            if (targetModel == null)
                return;

            if (string.IsNullOrEmpty(itemName))
                return;

            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
                return;

            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if (obj.weapons_all[i].itemName == itemName)
                {
                    Weapon w = obj.weapons_all[i];
                    if (leftHand)
                    {
                        w.l_model_eulers = targetModel.transform.localEulerAngles;
                        w.l_model_pos = targetModel.transform.localPosition;
                    }
                    else
                    {
                        w.r_model_eulers = targetModel.transform.localEulerAngles;
                        w.r_model_pos = targetModel.transform.localPosition;
                    }
                    w.model_scale = targetModel.transform.localScale;

                    return;
                }
            }

            Debug.Log(itemName + " wasn't found in inventory!");
        }

        void SaveConsumable()
        {
            if (targetModel == null)
                return;

            if (string.IsNullOrEmpty(itemName))
                return;

            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;

            if (obj == null)
                return;

            for (int i = 0; i < obj.consumables.Count; i++)
            {
                if (obj.consumables[i].itemName == itemName)
                {
                    Consumable w = obj.consumables[i];
                    w.r_model_eulers = targetModel.transform.localEulerAngles;
                    w.r_model_pos = targetModel.transform.localPosition;
                    w.model_scale = targetModel.transform.localScale;

                    return;
                }
            }

            Debug.Log(itemName + " wasn't found in inventory!");
        }

    }
}

