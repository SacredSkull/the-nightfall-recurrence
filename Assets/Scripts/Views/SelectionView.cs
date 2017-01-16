using System.Collections;
using Action.Attack;
using Level.Entity;
using UnityEngine;
using UnityEngine.UI;
using Logger = Utility.Logger;

namespace Views {
    public class SelectionView : View {
        [Range(0f, 0.5f)]
        public float Cooldown;
        public SpriteRenderer Icon;
        public Text Name;
        public Text Description;
        public GameObject AbilitiesContainer;
        public GameObject AbilityPrefab;
        public Text AbilityDocumentation;
        public bool DebugSelection;

        private float timer;

        private void Awake() {
            TileView.SelectEvent += handleSelection;
        }

        private void Start() {
            Icon?.gameObject.SetActive(false);
            Name?.gameObject.SetActive(false);
            Description?.gameObject.SetActive(false);
            AbilitiesContainer?.SetActive(false);
            if(AbilityDocumentation != null) {
                AbilityDocumentation.text = "";
                AbilityDocumentation.gameObject.SetActive(false);
            }
        }

        private void handleSelection(TileView tile, GameObject o) {
            if(Time.time < timer) {
                if(DebugSelection)
                    Logger.UnityLog($"Selection for {tile.MapItem.name} is on cooldown (-{timer - Time.time}s)");
                return;
            };

            timer = Time.time + Cooldown;

            if(DebugSelection)
                Logger.UnityLog($"Selected {tile?.MapItem.name} - gameobject is {o.name}");
            if(AbilityDocumentation != null) {
                AbilityDocumentation.GetComponent<Text>().text = "";
            }

            SoftwareTool tool;

            TrailTile item = tile.MapItem as TrailTile;
            if(item != null) {
                TrailTile trail = item;
                tool = trail.Trail.Head;
            } else {
                tool = tile.MapItem as SoftwareTool;
            }
            bool sentry = false;

            if(tool == null) {
                Icon?.gameObject.SetActive(false);
                Name?.gameObject.SetActive(false);
                Description?.gameObject.SetActive(false);
                AbilitiesContainer?.SetActive(false);
                AbilityDocumentation?.gameObject.SetActive(false);
                return;
            }

            if(tool is Sentry) {
                sentry = true;
            }

            Icon.sprite = tile.Sprite;
            Name.text = tile.MapItem.name;
            Description.text = tile.MapItem.description;

            foreach(Transform child in AbilitiesContainer.transform) {
                Destroy(child.gameObject);
            }

            foreach(var ability in tool.Attacks) {
                GameObject button = Instantiate(AbilityPrefab, AbilitiesContainer.transform.position,
                    Quaternion.identity, AbilitiesContainer.transform);
                Text buttonText = button.transform.Find("Name").GetComponent<Text>();

                if(buttonText == null) {
                    Logger.UnityLog("Could not create a button for an ability/attack!", Logger.Level.ERROR);
                    continue;
                }

                buttonText.text = ability.Name;
                button.GetComponent<Button>().onClick.AddListener(() => { abilityButtonHandler(ability, sentry); });

                Icon?.gameObject.SetActive(true);
                Description?.gameObject.SetActive(true);
                Name?.gameObject.SetActive(true);
                AbilitiesContainer?.SetActive(true);
                AbilityDocumentation?.gameObject.SetActive(true);
            }
        }

        private void abilityButtonHandler(Attack attack, bool sentry = true) {
            string desc = $"Name: {attack.Name}\nRange: {attack.Range}";
            if(AbilityDocumentation != null) {
                AbilityDocumentation.GetComponent<Text>().text = desc;
            }
        }
    }
}
