using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {
    public class AIHandler : MonoBehaviour
    {
        public AIAttacks[] ai_attacks;

        public EnemyStates states;

        public StateManager en_states;
        public Transform target;

        public float sight;
        public float fov_angle;

        public int closeCount = 10;
        int _close;

        public int frameCount = 30;
        int _frame;

        public int attackCount = 30;
        int _attack;

        float dis;
        float angle;
        float delta;
        Vector3 dirToTarget;

        void Start() {
            if (states == null)
                states = GetComponent<EnemyStates>();

            states.Init();
            InitDamageColliders();
        }

        void InitDamageColliders() {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                for (int j = 0; j < ai_attacks[i].damageCollider.Length; j++)
                {
                    DamageCollider d = ai_attacks[i].damageCollider[j].GetComponent<DamageCollider>();
                    d.InitEnemy(states);
                }
            }

            for (int i = 0; i < states.defaultDamageCollider.Length; i++)
            {
                DamageCollider d = states.defaultDamageCollider[i].GetComponent<DamageCollider>();
                d.InitEnemy(states);
            }
        }

        public enum AIState {
            far, close, inSight, attacking
        }
        public AIState aiState;

        void Update() {
            delta = Time.deltaTime;
            dis = DistanceFromTarget();
            angle = AngleToTarget();
            if (target)
                dirToTarget = target.position - transform.position;
            states.dirToTarget = dirToTarget;

            switch (aiState)
            {
                case AIState.far:
                    HandleFarSight();
                    break;
                case AIState.close:
                    HandleCloseSight();
                    break;
                case AIState.inSight:
                    Insight();
                    break;
                case AIState.attacking:
                    if (states.canMove)
                    {
                        states.rotateToTarget = true;
                        aiState = AIState.inSight;
                    }
                    break;
                default:
                    break;
            }

            states.Tick(delta);
        }

        void GoToTarget() {
            states.hasDestination = false;
            states.SetDestination(target.position);
        }

        void Insight() {
            #region delay handler

            HandleCooldowns();

            float d2 = Vector3.Distance(states.targetDestionation, target.position);
            if (d2 > 2 || dis > sight * 5)
                GoToTarget();
            if (dis < 2)
                states.agent.isStopped = true;
                

            if (_attack > 0)
            {
                _attack--;
                return;
            }
            _attack = attackCount;

            #endregion

            #region perform attack

            AIAttacks a = WillAttack();
            states.SetCurrentAttack(a);

            if (a != null && en_states.isDead == false)
            {
                aiState = AIState.attacking;
                states.anim.Play(a.targetAnim);
                states.anim.SetBool("OnEmpty", false);
                states.canMove = false;
                a._cool = a.cooldown;
                states.agent.isStopped = true;
                states.rotateToTarget = false;
                return;
            }
            #endregion

            return;
        }

        void HandleCooldowns()
        {
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cool > 0)
                {
                    a._cool -= delta;
                    if (a._cool < 0)
                    {
                        a._cool = 0;
                    }
                    continue;
                }
            }
        }

        public AIAttacks WillAttack() {
            int w = 0;
            List<AIAttacks> l = new List<AIAttacks>();
            for (int i = 0; i < ai_attacks.Length; i++)
            {
                AIAttacks a = ai_attacks[i];
                if (a._cool > 0)
                    continue;
                if (dis > a.minDistance)
                    continue;
                if (angle < a.minAngle)
                    continue;
                if (angle > a.maxAngle)
                    continue;
                if (a.weight == 0)
                    continue;

                w += a.weight;
                l.Add(a);
            }

            if (l.Count == 0)
                return null;

            int ran = Random.Range(0, w + 1);
            int c_w = 0;
            for (int i = 0; i < l.Count; i++)
            {
                c_w += l[i].weight;
                if (c_w > ran)
                {
                    return l[i];
                }
            }

            return null;
        }

        void HandleFarSight()
        {
            if (target == null)
                return;

            _frame++;
            if (_frame > frameCount)
            {
                _frame = 0;

                if (dis < sight)
                {
                    if (angle < fov_angle)
                    {
                        aiState = AIState.close;
                    }
                }
            }
        }

        void HandleCloseSight()
        {
            _close++;
            if (_close > closeCount)
            {
                _close = 0;

                if (dis > sight || angle > fov_angle)
                {
                    aiState = AIState.far;
                    return;
                }
            }
            RaycastToTarget();
        }

        void RaycastToTarget() {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += 0.5f;
            Vector3 dir = dirToTarget;
            dir.y += 0.5f;

            if (Physics.Raycast(origin, dir, out hit, sight, states.ignoreLayers)) {
                StateManager st = hit.transform.GetComponentInParent<StateManager>();
                if (st != null) {
                    states.rotateToTarget = true; //happen only at the initial attack, updated in Update().
                    aiState = AIState.inSight;
                }
            }

        }

        float DistanceFromTarget()
        {
            if (target == null)
                return 100;

            return Vector3.Distance(target.position, transform.position);
        }

        float AngleToTarget()
        {
            float a = 180;
            if (target)
            {
                Vector3 d = dirToTarget;
                a = Vector3.Angle(d, transform.forward);
            }
            return a;
        }

       

        
    }

    [System.Serializable]
    public class AIAttacks {
        public int weight;
        public float minDistance;
        public float minAngle;
        public float maxAngle;

        public float cooldown = 2;
        public float _cool;

        public string targetAnim;

        public bool isDefaultDamageCollider;
        public GameObject[] damageCollider;
    }
}

