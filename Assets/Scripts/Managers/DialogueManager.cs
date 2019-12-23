using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA {
    public class DialogueManager : MonoBehaviour
    {
        public Text dialogueText;
        public GameObject textObj;
        Transform origin;
        NPCDialogue npc_dialogue;
        public bool dialogueActive;
        bool updateDialog;
        int textIndex;
        public Transform playerObject;

        public void Init(Transform po) {
            playerObject = po;
        }

        //npc states
        public NPCStates[] npcStates;
        Dictionary<string, int> npc_ids = new Dictionary<string, int>();
        NPCStates npc_state;

        public void InitDialogue(Transform o, string id) {
            origin = o;
            npc_dialogue = ResourceManager.singleton.GetNPCDialogue(id);
            npc_state = GetNPCStates(id);
            dialogueActive = true;
            textObj.SetActive(true);
            updateDialog = false;
            textIndex = 0;
        }


        public void Tick(bool a_input) {
            if (!dialogueActive)
                return;
            if (origin == null)
                return;

            float distance = Vector3.Distance(playerObject.transform.position, origin.transform.position);
            if (distance > 3.5) {
                CloseDialogue();
            }

            if (!updateDialog) {
                updateDialog = true;
                
                dialogueText.text = npc_dialogue.dialogue[npc_state.dialogueIndex].dialogueText[textIndex];
            }

            if (a_input)
            {
                updateDialog = false;
                textIndex++;
                
                if (textIndex > npc_dialogue.dialogue[npc_state.dialogueIndex].dialogueText.Length - 1)
                {
                    if (npc_dialogue.dialogue[npc_state.dialogueIndex].increaseIndex) {
                        npc_state.dialogueIndex++;

                        if (npc_state.dialogueIndex > npc_dialogue.dialogue.Length - 1) {
                            npc_state.dialogueIndex = npc_dialogue.dialogue.Length - 1;
                        }
                    }
                    CloseDialogue();
                }
            }

        }

        void CloseDialogue() {
            dialogueActive = false;
            textObj.SetActive(false);
        }


        public static DialogueManager singleton;
        private void Awake()
        {
            singleton = this;
            textObj.SetActive(false);
            for (int i = 0; i < npcStates.Length; i++)
            {
                npc_ids.Add(npcStates[i].npc_id, i);
            }
        }

        public NPCStates GetNPCStates(string id)
        {
            int index = -1;
            npc_ids.TryGetValue(id, out index);

            if (index == -1)
                return null;

            return npcStates[index];
        }
    }

    [System.Serializable]
    public class NPCStates
    {
        public string npc_id;
        public int dialogueIndex;
        
    }
}

