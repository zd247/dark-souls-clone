using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SA;


namespace SA.Utilities {

    [ExecuteInEditMode]
    public class itemToXML : MonoBehaviour
    {
        public bool make;
        public List<RuntimeWeapon> canidates = new List<RuntimeWeapon>();
        public string xml_version;
        public string targetName;
        public ActionManager actionManager;

        void Update()
        {
            if (!make)
                return;
            make = false;

            string xml = xml_version; //<?xml version = "1.0" encoding = "UTF-8" ?>
            xml += "\n";
            xml += "<root>" + "\n";
            

            foreach (RuntimeWeapon i in canidates) {
                Weapon w = i.instance;

                xml += "<weapon>" + "\n";

                    //xml += "<weaponID>" + w.weaponID + "</weaponID>" + "\n";
                    xml += "<oh_idle>" + w.oh_idle + "</oh_idle>" + "\n";
                    xml += "<th_idle>" + w.th_idle + "</th_idle>" + "\n";

                    xml += ActionListToString(w.actions, "actions");
                    xml += ActionListToString(w.two_handedActions, "two_handed");

                    xml += "<parryMultiplier>" + w.parryMultiplier.ToString() + "</parryMultiplier>" + "\n";
                    xml += "<backstabMultiplier>" + w.backstabMultiplier.ToString() + "</backstabMultiplier>" + "\n";
                    xml += "<LeftHandMirror>" + w.LeftHandMirror + "</LeftHandMirror>" + "\n";

                    WeaponStats s = w.weaponStats;

                    xml += "<physical>" + s.physical + "</physical>" + "\n";
                    xml += "<strike>" + s.strike + "</strike>" + "\n";
                    xml += "<slash>" + s.slash + "</slash>" + "\n";
                    xml += "<thrust>" + s.thrust + "</thrust>" + "\n";
                    xml += "<magic>" + s.magic + "</magic>" + "\n";
                    xml += "<fire>" + s.fire + "</fire>" + "\n";
                    xml += "<lightning>" + s.lightning + "</lightning>" + "\n";
                    xml += "<dark>" + s.dark + "</dark>" + "\n";

                xml += "</weapon>" + "\n";
            }

            xml += "</root>";


            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName)) {
                targetName = "items_database.xml";
            }

            path += targetName;

            File.WriteAllText(path, xml);
        }

        string ActionListToString(List<Action> l, string nodeName) {
            string xml = null;

            foreach (Action a in l)
            {
                xml += "<" + nodeName + ">" + "\n";


                xml += "<ActionInput>" + a.input.ToString() + "</ActionInput>" + "\n";
                xml += "<ActionType>" + a.type.ToString() + "</ActionType>" + "\n";
                xml += "<targetAnim>" + a.GetActionStep(ref actionManager.actionIndex).GetBranch(a.input).targetAnim + "</targetAnim>" + "\n";
                xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed.ToString() + "</animSpeed>" + "\n";
                xml += "<canParry>" + a.canParry + "</canParry>" + "\n";
                xml += "<canBackStab>" + a.canBackStab + "</canBackStab>" + "\n";
                xml += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>" + "\n";
                xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                



                xml += "</" + nodeName + ">" + "\n";
            }

            return xml;
        }

    }
}

