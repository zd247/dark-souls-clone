using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class NPCInteraction : WorldInteraction
    {
        public string npcId;
        
        public override void InteractActual()
        {
            DialogueManager.singleton.InitDialogue(this.transform, npcId);
        }
    }

    
}

