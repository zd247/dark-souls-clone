using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class InteractionScriptableObject : ScriptableObject
    {
        public List<Interactions> interactions = new List<Interactions>();
    }

    [System.Serializable]
    public class Interactions {
        public string interactionId;
        public string anim;
        public bool oneShot;
        public string specialEvent;
    }
}

