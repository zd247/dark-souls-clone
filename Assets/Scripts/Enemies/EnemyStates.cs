using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace SA{
    public class EnemyStates : MonoBehaviour {
        [Header("Stats")]
        public int health;
        public int maxHealth;
        public CharacterStats characterStats;

        [Header("Value")]
        public float delta;
        public float vertical;
        public float horizontal;
        public float poiseDegrade = 2f;

        [Header("States")]
        public bool canBeParried = true;
        public bool parryIsOn = true;
        public bool isInvincible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        public bool hasDestination;
        public Vector3 targetDestionation;
        public Vector3 dirToTarget;
        public bool rotateToTarget;
        public bool isGreatSword;
        public bool damaged = false;

        //References
        public Animator anim;
        public Rigidbody rigid;
        EnemyTarget enTarget;
        AnimatorHook a_hook;
        public StateManager parriedBy;
        public LayerMask ignoreLayers;
        public NavMeshAgent agent;
        public StateManager player;
        public GameObject lockOnGameObject;
        public Canvas enemyCanvas;
        public GameObject dropGameObject;
        public AudioSource audioSource;

        //Attacks
        AIAttacks curAttack;
        public void SetCurrentAttack(AIAttacks a) {
            curAttack = a;
        }
        public AIAttacks GetCurrentAttack() {
            return curAttack;
        }

        public GameObject[] defaultDamageCollider;
       

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        float timer;
        Image healthBar;

        public delegate void SpellEffect_Loop();
        public SpellEffect_Loop spellEffect_loop;

        

        public void Init() {
            anim = GetComponentInChildren<Animator>();
            audioSource = this.gameObject.AddComponent<AudioSource>();
            audioSource.maxDistance = 3.5f;
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);

            rigid = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            rigid.isKinematic = true;

            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                // add AnimatorHook component to the active model.
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();

            enemyCanvas = GetComponentInChildren<Canvas>();

            a_hook.Init(null, this);
            InitRagDoll();
            parryIsOn = false;

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            lockOnGameObject.SetActive(false);
            healthBar = enemyCanvas.transform.Find("HealthBG").Find("Health").GetComponent<Image>();
            enemyCanvas.gameObject.SetActive(false);
            health = maxHealth;

        }

        void InitRagDoll() {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++) {
                if (rigs[i] == rigid)
                    continue;

                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll() {
            for (int i = 0; i < ragdollRigids.Count; i++) {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
                ragdollRigids[i].detectCollisions = false;
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;



            StartCoroutine("CloseAnimator");
        }

        IEnumerator CloseAnimator() {
            yield return new WaitForEndOfFrame();
            anim.enabled = false;
            this.enabled = false;
        }

        public void Tick(float d) {
            if (isGreatSword)
                anim.Play("gs_oh_idle_r");
            delta = d;
            delta = Time.deltaTime;
            canMove = anim.GetBool("OnEmpty");

            if (enTarget != null)
            {
                if (player.lockOnTarget == null)
                    enTarget.isLockOn = false;

                if (enTarget.isLockOn)
                {
                    lockOnGameObject.SetActive(true);
                }
                else
                {
                    lockOnGameObject.SetActive(false);
                }
            }

            if (damaged)
                enemyCanvas.gameObject.SetActive(true);

            UpdateEnemyHealthUI(health, maxHealth);

            if (spellEffect_loop != null)
                spellEffect_loop();

            if (dontDoAnything) {
                dontDoAnything = !canMove;
                return;
            }

            if (rotateToTarget)
            {
                LookTowardsTarget();
            }

            if (health <= 0) {
                if (!isDead) {
                    isDead = true;
                    enemyCanvas.gameObject.SetActive(false);
                    audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("die").audio_clip);
                    EnableRagdoll();
                    StartCoroutine(StartSinking());
                }
            }

            if (isInvincible) {
                isInvincible = !canMove;
            }

            if (parriedBy != null && parryIsOn == false) {
                parriedBy = null;
            }

            if (canMove)
            {
                parryIsOn = false;
                anim.applyRootMotion = false;

                MovementAnimation();
            }
            else {
                if (anim.applyRootMotion == false)
                    anim.applyRootMotion = true;
            }

            characterStats.poise -= delta * poiseDegrade;
            if (characterStats.poise < 0)
                characterStats.poise = 0;
            damaged = false;
        }

        public void MovementAnimation() {
            float square = agent.desiredVelocity.sqrMagnitude;
            float v = Mathf.Clamp(square, 0, 0.5f);
            anim.SetFloat("vertical", v, 0.2f, delta);
        }

        void LookTowardsTarget()
        {
            Vector3 dir = dirToTarget;
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, delta * 5);
        }

        public void SetDestination(Vector3 d) {
            if (agent)
            {
                if (!hasDestination)
                {
                    hasDestination = true;
                    agent.isStopped = false;
                    agent.SetDestination(d);
                    targetDestionation = d;
                }
            }
        }

        public void DoDamage() {
            if (isInvincible)
                return;
            damaged = true;
            rotateToTarget = true;
            //int damage = StatsCalculations.CalculateBaseDamage(curWeapon.weaponStats, characterStats);
            int damage = 20;
            health -= damage;
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("slash_impact").audio_clip);
            if (canMove )
            {
                int ran = Random.Range(0, 100);
                string tA = (ran > 50) ? "damage_1" : "damage_2";
                anim.Play(tA);
            }
            isInvincible = true;
            anim.applyRootMotion = true;
            anim.SetBool("canMove", false);

        }

        public void DoDamageSpell() {
            if (isInvincible)
                return;
            health -= 50;
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("damage_3").audio_clip);
            damaged = true;
            rotateToTarget = true;
            anim.Play("damage_3");

        }

        public void HandleBlocked() {
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("shield_impact").audio_clip);
            anim.Play("attack_interrupt");
            anim.SetFloat("interruptSpeed", 1.2f);
            player.characterStats._stamina -= 40;
            Vector3 targetDir = transform.position - player.transform.position;
            player.SnapToRotation(targetDir);
            CloseDamageCollider();

        }

        public void CheckForParry(Transform target, StateManager states) {
            if (canBeParried == false || parryIsOn == false || isInvincible)
                return;

            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            // means that the enemy and the character are not facing towards each other.
            if (dot < 0)
                return;


            isInvincible = true;
            anim.Play("attack_interrupt");
            anim.SetFloat("interruptSpeed", 0.5f);
            anim.applyRootMotion = true;
            anim.SetBool("canMove", false);
            //			states.parryTarget = this;
            parriedBy = states;
            return;
        }

        public void IsGettingParried(Action a, Weapon curWeapon) {
            //float damage = StatsCalculations.CalculateBaseDamage(curWeapon.weaponStats, characterStats, a.parryMultiplier);
            float damage = 80;

            health -= Mathf.RoundToInt(damage);
            dontDoAnything = true;
            anim.SetBool("canMove", false);
            anim.Play("parry_received");
        }

        public void IsGettingBackStabbed(Action a, Weapon curWeapon)
        {
            dontDoAnything = true; 
            anim.SetBool("canMove", false);
            anim.Play("backstab_received");
            StartCoroutine(PlaySlashImpact());
            StartCoroutine(SetHealth());
        }

        IEnumerator SetHealth() {
            yield return new WaitForSeconds(2.07f);
            health = 0;
        }

        IEnumerator PlaySlashImpact() {
            yield return new WaitForSeconds(0.5f);
            audioSource.PlayOneShot(ResourceManager.singleton.GetAudio("slash_impact").audio_clip);
        }

        public ParticleSystem fireParticle;
        float _t;

        public void OnFire() {
            if (fireParticle == null)
                return;

            if (_t < 3)
            {
                _t += Time.deltaTime;
                fireParticle.Emit(1);
            }
            else {
                _t = 0;
                spellEffect_loop = null;
            }
        }

        public void OpenDamageCollier()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, true);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, true);
            }
        }

        public void CloseDamageCollider()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, false);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, false);
            }
        }

        void ObjectListStatus(GameObject[] l, bool status)
        {
            for (int i = 0; i < l.Length; i++)
            {
                l[i].SetActive(status);
            }
        }

        IEnumerator StartSinking() {
            this.GetComponent<CapsuleCollider>().enabled = false;
            player.lockOnTarget = null;
            EnemyManager.singleton.enemyTargets.Remove(enTarget);
            yield return new WaitForSeconds(0.8f);
            HandleDropItem();
            Destroy(this.gameObject);

        }

        void HandleDropItem() {
            GameObject go = new GameObject();
            go = Instantiate(dropGameObject) as GameObject;
            go.transform.position = this.transform.position;
            player.pickManager.pick_items.Add(go.GetComponent<PickableItem>());
            
        }

        void UpdateEnemyHealthUI(int curHealth, int maxHealth) {
            healthBar.fillAmount = (float)curHealth / (float) maxHealth;
        }
    }
}

