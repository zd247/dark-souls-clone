using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {
    public class EnemyManager : MonoBehaviour
    {
        public List<EnemyTarget> enemyTargets = new List<EnemyTarget>();

        public EnemyTarget GetEnemy(Vector3 from) {
            EnemyTarget r = null;
            float minDist = float.MaxValue;
            for (int i = 0; i < enemyTargets.Count; i++)
            {
                float tDist = Vector3.Distance(from, enemyTargets[i].GetTarget().position);
                if (tDist < minDist) {
                    minDist = tDist;
                    r = enemyTargets[i];
                }
            }

            return r;
        }

        public static EnemyManager singleton;
        public void Awake()
        {
            singleton = this;
        }

    }
}

