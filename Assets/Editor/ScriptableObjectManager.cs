using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace SA {
    public class ScriptableObjectManager : MonoBehaviour
    {

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if (Resources.Load(typeof(T).ToString()) == null)
            {
                string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString() + ".asset");

                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;

            }
            else {
                Debug.Log(typeof(T).ToString() + " already created!");
            }
        }

        [MenuItem("Assets/Inventory/Create Consumables List")]
        public static void CreateConsumables() {
            ScriptableObjectManager.CreateAsset<ConsumablesScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateWeaponList")]
        public static void CreateWeaponList() {
            ScriptableObjectManager.CreateAsset<WeaponScriptableObject>();
        }

         [MenuItem("Assets/Inventory/CreateSpellItemList")]
        public static void CreateSpellItemList() {
            ScriptableObjectManager.CreateAsset<SpellItemScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateItemList")]
        public static void CreateItemList()
        {
            ScriptableObjectManager.CreateAsset<ItemScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateInteractionList")]
        public static void CreateInteractionList()
        {
            ScriptableObjectManager.CreateAsset<InteractionScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateNPCList")]
        public static void CreateNPCList()
        {
            ScriptableObjectManager.CreateAsset<NPCScriptableObject>();
        }

        [MenuItem("Assets/Inventory/CreateAudioList")]
        public static void CreateAudioList()
        {
            ScriptableObjectManager.CreateAsset<AudioScriptableObject>();
        }

    }
}

