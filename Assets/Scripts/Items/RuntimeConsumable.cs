using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class RuntimeConsumable : MonoBehaviour
    {
        public int itemCount = 1;
        public bool unlimitedCount;
        public Consumable instance;
        public GameObject itemModel;
    }
}

