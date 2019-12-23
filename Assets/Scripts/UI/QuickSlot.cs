using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SA.UI
{
    //----------------------classes-------------------------------
    [System.Serializable]
    public class QSlots
    {
        public Image icon;
        public QSlotType type;
    }

    public enum QSlotType
    {
        rh, lh, item, spell
    }

    //--------------------main------------------------------------
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlots> slots = new List<QSlots>();


        public void Init()
        {
            ClearIcons();
        }

        public void ClearIcons()
        {
            for (int i = 0; i < slots.Count; i++)   
            {
                slots[i].icon.gameObject.SetActive(false);
            }
        }


        public void UpdateSlot(QSlotType sType, Sprite spr)
        {
            QSlots q = GetSlot(sType);
            q.icon.sprite = spr;
            q.icon.gameObject.SetActive(true);
        }


        public QSlots GetSlot(QSlotType sType)
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].type == sType)
                    return slots[i];
            }
            return null;
        }

        public static QuickSlot singleton;

        void Awake()
        {
            singleton = this;
        }
    }
}

