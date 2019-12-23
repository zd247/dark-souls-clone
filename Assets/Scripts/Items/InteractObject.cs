using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA{
    public class InteractObject : WorldInteraction
    {
        public GameObject obj;
        public override void InteractActual() {
            obj.SetActive(true);
            base.InteractActual();
        }
    }
}

