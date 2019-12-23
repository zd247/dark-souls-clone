using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class SceneManager : MonoBehaviour
    {
        public GameObject map1;
        public GameObject map2;
        public StateManager states;
        public GameObject gameOverUI;

        void Start() {
            map2.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            StateManager states = other.GetComponent<StateManager>();
            if (states != null && this.gameObject.tag == "LevelChanger")
            {
                map2.SetActive(true);
                states.gameObject.transform.position = new Vector3(-442.9f, -14.214f, -219.52f);
                map1.SetActive(false);
            }
        }

        public IEnumerator HandleGameOver() {
            states.audio_clip = ResourceManager.singleton.GetAudio("character_die").audio_clip;
            states.audio_source.PlayOneShot(states.audio_clip);
            states.canAttack = false;
            states.canMove = false;
            states.isInvincible = true;
            states.anim.Play("dead");
            
            
            yield return new WaitForSeconds(3.5f);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameOverUI.SetActive(true);
        }
    }
}

