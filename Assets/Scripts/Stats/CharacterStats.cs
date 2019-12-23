using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA {
    [System.Serializable]
    public class CharacterStats
    {
        [Header("Current")]
        public float _health;
        public float _focus;
        public float _stamina;
        public int _souls;

        public float _healthRecoverValue = 60;
        public float _focusRecoverValue = 80;
        

        [Header("Base Power")]
        public int hp;
        public int fp;
        public int stamina;
        public float equipLoad;
        public float poise;
        public int itemDiscover;

        [Header("Attack Power")]
        public int R_weapon_1;
        public int R_weapon_2;
        public int R_weapon_3;
        public int L_weapon_1;
        public int L_weapon_2;
        public int L_weapon_3 ;

        [Header("Defence")]
        public int physical;
        public int vs_strike;
        public int vs_slash;
        public int vs_thrust;
        public int magic;
        public int fire;
        public int lightning;
        public int dark;

        [Header("Resistances")]
        public int bleed;
        public int poison;
        public int frost;
        public int curse;

        public int attumentSlots;

        public void InitCurrent()
        {
            _health = hp;
            _focus = fp;
            _stamina = stamina;
        }

        public delegate void StatEffects();
        public StatEffects statEffect;

        public void AddHealth() {
            hp += 5;
        }

        public void RemoveHealth() {
            hp -= 5;
        }
    }

   

    [System.Serializable]
    public class Attributes
    {
        public int level;
        public int souls;
        public int vigor;
        public int attunement;
        public int endurance;
        public int vitality;
        public int strength;
        public int dexterity;
        public int intelligence;
        public int faith;
        public int luck;
    }

    [System.Serializable]
    public class WeaponStats {
        public int physical;
        public int strike;
        public int slash;
        public int thrust;
        public int magic;
        public int fire;
        public int lightning;
        public int dark;
    }


}

