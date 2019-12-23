using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class AudioScriptableObject : ScriptableObject
    {
        public List<Audio> audio_list = new List<Audio>();
    }

    [System.Serializable]
    public class Audio {
        public string id;
        public AudioClip audio_clip;
    }
}

