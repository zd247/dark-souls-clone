using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace SA {
    public static class StaticStrings
    {

        //Inputs

        public static string Vertical = "Vertical";
        public static string Horizontal = "Horizontal";
        public static string B = "B";
        public static string A = "A";
        public static string X = "X";
        public static string Y = "Y";
        public static string RT = "RT";
        public static string LT = "LT";
        public static string RB = "RB";
        public static string LB = "LB";
        public static string L = "L";
        public static string R = "R";


        // Animator Parameters
        public static string vertical = "vertical";
        public static string horizontal = "horizontal";
        public static string mirror = "mirror";
        public static string parry_attack = "parry_attack";
        public static string animSpeed = "animSpeed";
        public static string onGround = "onGround";
        public static string run = "run";
        public static string two_handed = "two_handed";
        public static string interacting = "interacting";
        public static string blocking = "blocking";
        public static string isLeft = "isLeft";
        public static string canMove = "canMove";
        public static string lockon = "lockon";

        // Animator States
        public static string Rolls = "Rolls";
        public static string attack_interrupt = "attack_interrupt";
        public static string parry_received = "parry_received";
        public static string backstab_received = "backstab_received";

        // Data
        public static string itemFolder = "/Items/";

        public static string SaveLocation()
        {

            string r = Application.streamingAssetsPath;
            if (!Directory.Exists(r))
            {
                Directory.CreateDirectory(r);
            }

            return r;
        }


    }
}

