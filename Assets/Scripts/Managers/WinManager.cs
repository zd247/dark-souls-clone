using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA {
    public class WinManager : MonoBehaviour
    {
        public EnemyManager enManager;
        public GameObject winMenu;

        void Init() {
            enManager = GetComponent<EnemyManager>();
        }

        void OnTriggerEnter(Collider other)
        {
            StateManager states = other.GetComponent<StateManager>();
            if (states != null)
            {
                if (enManager.enemyTargets.Count == 0)
                {
                    winMenu.GetComponentInChildren<Text>().text = "YOU HAVE BEATEN ALL ENEMIES. WELCOME HOME CHOSEN ONE !";
                    StartCoroutine(handleWinMenu());
                    this.gameObject.SetActive(false);
                }
                else
                {
                    winMenu.GetComponentInChildren<Text>().text = "TURN BACK, YOU HAVE NOT FINISHED YOUR JOB, YOU STILL HAVE " + enManager.enemyTargets.Count + " ENEMIES LEFT TO SLAY !";
                    StartCoroutine(handleWinMenu());
                }
            }
            else {
                return;
            }
            
        }

        IEnumerator handleWinMenu() {
            winMenu.SetActive(true);
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(3);
            Time.timeScale = 1f;
            winMenu.SetActive(false);
        }
    }
}

