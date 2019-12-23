using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class UIManager : MonoBehaviour
    {
        public float lerpSpeed;
        public Slider health;
        public Slider h_vis;
        public Slider focus;
        public Slider f_vis;
        public Slider stamina;
        public Slider s_vis;

        public Text souls;
        public Text itemCount;
        public float sizeMultiplier;
        int curSouls;
        int curItemCount;

        public GameObject interactCard;
        public Text ac_action_type;

        int ac_index;
        public List<AnnounceCard> announceCard;

        //-----------------------------------------------------------------

        void Start()
        {
            CloseInteractCard();
            CloseAnnounceCard();
        }

        public void InitSouls(int v) {
            curSouls = v;
        }

        public void InitSlider(StatSlider t, int value) {
            

            Slider s = null;
            Slider v = null;

            switch (t)  
            {
                case StatSlider.health: 
                    s = health;
                    v = h_vis;
                    break;
                case StatSlider.focus:
                    s = focus;
                    v = f_vis;
                    break;
                case StatSlider.stamina:
                    s = stamina;
                    v = s_vis;
                    break;
                default:
                    break;
            }

            s.maxValue = value;
            v.maxValue = value;
            RectTransform r = s.GetComponent<RectTransform>();
            RectTransform r_v = v.GetComponent<RectTransform>();

            //Modify the width
            float value_actual = value * sizeMultiplier;
            value_actual = Mathf.Clamp(value_actual, 0, 1000);

            //Set modifed width for the Slider
            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
            r_v.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value_actual);
        }

        public void Tick(CharacterStats stats, float delta, StateManager states) {
            health.value = Mathf.Lerp(health.value, stats._health, delta * lerpSpeed * 2);
            focus.value = Mathf.Lerp(focus.value, stats._focus, delta * lerpSpeed * 2);
            stamina.value = stats._stamina;

            curSouls = Mathf.RoundToInt(Mathf.Lerp(curSouls, stats._souls, delta * lerpSpeed * 10)); // curSouls = 0 at Init but changes overtime.
            souls.text = curSouls.ToString();
            itemCount.text = states.inventoryManager.curConsumable.itemCount.ToString();

            h_vis.value = Mathf.Lerp(h_vis.value, stats._health, delta * lerpSpeed);
            f_vis.value = Mathf.Lerp(f_vis.value, stats._focus, delta * lerpSpeed);
            s_vis.value = Mathf.Lerp(s_vis.value, stats._stamina, delta * lerpSpeed);
        }

        public void AffectAll(int h, int f, int s) {
            InitSlider(StatSlider.health, h);
            InitSlider(StatSlider.focus, f);
            InitSlider(StatSlider.stamina, s);

        }

        public void OpenInteractCard(UIActionType type) {
            switch (type)
            {
                case UIActionType.pickup:
                    ac_action_type.text = "Pick Up: Space";
                    break;
                case UIActionType.interact:
                    ac_action_type.text = "Interact: Space";
                    break;
                case UIActionType.open:
                    ac_action_type.text = "Open: Space";
                    break;
                case UIActionType.talk:
                    ac_action_type.text = "Talk: Space";
                    break;
                default:
                    break;
            }

            interactCard.SetActive(true);
        }

        public void CloseInteractCard()
        {
            interactCard.SetActive(false);
        }

        public void AddAnnounceCard(Item i) {
            announceCard[ac_index].itemName.text = i.itemName;
            announceCard[ac_index].icon.sprite = i.icon;
            announceCard[ac_index].gameObject.SetActive(true);
            ac_index++;
            if (ac_index > 4)
            {
                ac_index = 0;
            }
        }

        public void CloseAnnounceCard() {
            for (int i = 0; i < announceCard.Count; i++)
            {
                announceCard[i].gameObject.SetActive(false);
            }
        }

        public static UIManager singleton;
        void Awake()
        {
            singleton = this;
        }        
    }

    public enum StatSlider
    {
        health, focus, stamina
    }

    public enum UIActionType
    {
        pickup, interact, open, talk
    }
}

