using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA {
    public class Projectile : MonoBehaviour
    {

        Rigidbody rigid;

        public float hSpeed = 5f;
        public float vSpeed = 2f;

        public Transform target;

        public GameObject explosionPrefab;

        public void Init()
        {
            rigid = GetComponent<Rigidbody>();

            //Add force
            Vector3 targetForce = transform.forward * hSpeed;
            targetForce += transform.up * vSpeed;
            rigid.AddForce(targetForce, ForceMode.Impulse);

        }

        void OnTriggerEnter(Collider other)
        {
            EnemyStates eStates = other.GetComponentInParent<EnemyStates>();

            if (eStates != null)
            {
                eStates.DoDamageSpell();
                SpellEffectsManager.singleton.UseSpellEffect("onFire", null, eStates);
            }
                
            GameObject g0 = Instantiate(explosionPrefab, transform.position, transform.rotation) as GameObject;
            Destroy(this.gameObject);
        }
    }

}
