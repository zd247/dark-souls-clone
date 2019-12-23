using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using SA;

namespace SA.Utilities {
    [ExecuteInEditMode]
    public class XMLtoResources : MonoBehaviour
    {
        public bool load;

        public ResourceManager resourceManager;
        public string weaponFileName = "items_database.xml";
        public ActionManager actionManager;
        
        void Update()
        {
            if (!load)
                return;

            load = false;

            LoadWeaponData();
        }

        public void LoadWeaponData() {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode w in doc.DocumentElement.SelectNodes("//weapon")) {
                Weapon _w = new Weapon();

                _w.actions = new List<Action>();
                _w.two_handedActions = new List<Action>();

                //XmlNode weaponID = w.SelectSingleNode("weaponID");
                //_w.weaponID = weaponID.InnerText;

                XmlNode oh_idle = w.SelectSingleNode("oh_idle");
                _w.oh_idle = oh_idle.InnerText;
                XmlNode th_idle = w.SelectSingleNode("th_idle");
                _w.th_idle = th_idle.InnerText;

                XmlToActions(doc, "actions", ref _w);
                XmlToActions(doc, "two_handed", ref _w);

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);

                XmlNode LeftHandMirror = w.SelectSingleNode("LeftHandMirror");
                _w.LeftHandMirror = (LeftHandMirror.InnerText == "True");

                _w.weaponStats = new WeaponStats();

                XmlNode physical = w.SelectSingleNode("physical");
                int.TryParse(physical.InnerText, out _w.weaponStats.physical);
                XmlNode strike = w.SelectSingleNode("strike");
                int.TryParse(strike.InnerText, out _w.weaponStats.strike);
                XmlNode slash = w.SelectSingleNode("slash");
                int.TryParse(slash.InnerText, out _w.weaponStats.slash);
                XmlNode thrust = w.SelectSingleNode("thrust");
                int.TryParse(thrust.InnerText, out _w.weaponStats.thrust);
                XmlNode magic = w.SelectSingleNode("magic");
                int.TryParse(magic.InnerText, out _w.weaponStats.magic);
                XmlNode fire = w.SelectSingleNode("fire");
                int.TryParse(fire.InnerText, out _w.weaponStats.fire);
                XmlNode lightning = w.SelectSingleNode("lightning");
                int.TryParse(lightning.InnerText, out _w.weaponStats.lightning);
                XmlNode dark = w.SelectSingleNode("dark");
                int.TryParse(dark.InnerText, out _w.weaponStats.dark);



                /* _w.model_pos = XmlToVector(w, "mp");
                 _w.model_eulers = XmlToVector(w, "me");
                 _w.model_scale = XmlToVector(w, "ms"); */


                //  resourceManager.weaponList.Add(_w);
            }  
        }

        Vector3 XmlToVector(XmlNode w, string prefix) {
            XmlNode _x = w.SelectSingleNode(prefix + "_x");
            float x = 0;
            float.TryParse(_x.InnerText, out x);

            XmlNode _y = w.SelectSingleNode(prefix + "_y");
            float y = 0;
            float.TryParse(_y.InnerText, out y);

            XmlNode _z = w.SelectSingleNode(prefix + "_z");
            float z = 0;
            float.TryParse(_z.InnerText, out z);

            return new Vector3(x, y, z);

        }

        void XmlToActions(XmlDocument doc, string nodeName, ref Weapon _w)
        {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//" + nodeName))
            {
                Action _a = new Action();

                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _a.input = (ActionInput) Enum.Parse(typeof(ActionInput), actionInput.InnerText);
                XmlNode actionType = a.SelectSingleNode("ActionType");
                _a.type = (ActionType)Enum.Parse(typeof(ActionType), actionType.InnerText);

                XmlNode targetAnim = a.SelectSingleNode("targetAnim");
                _a.GetActionStep(ref actionManager.actionIndex).GetBranch(_a.input).targetAnim = targetAnim.InnerText;

                XmlNode mirror = a.SelectSingleNode("mirror");
                _a.mirror = (mirror.InnerText == "True");
                XmlNode canBeParried = a.SelectSingleNode("canBeParried");
                _a.canBeParried = (canBeParried.InnerText == "True");

                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                _a.changeSpeed = (changeSpeed.InnerText == "True");
                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _a.animSpeed);


                XmlNode canParry = a.SelectSingleNode("canParry");
                _a.canParry = (canParry.InnerText == "True");
                XmlNode canBackStab = a.SelectSingleNode("canBackStab");
                _a.canBackStab = (canBackStab.InnerText == "True");

                XmlNode overrideDamageAnim = a.SelectSingleNode("overrideDamageAnim");
                _a.overrideDamageAnim = (overrideDamageAnim.InnerText == "True");
                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _a.damageAnim = damageAnim.InnerText;

                


                if (nodeName == "actions") {
                    _w.actions.Add(_a);
                } else {
                    _w.two_handedActions.Add(_a);
                }

            }
        }
    }
}

