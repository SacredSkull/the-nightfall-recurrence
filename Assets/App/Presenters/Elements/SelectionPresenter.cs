using System;
using System.Collections;
using Action.Attack;
using Controllers;
using Gamelogic.Extensions;
using Karma.Metadata;
using Level;
using Level.Entity;
using Models;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Zenject;
using Logger = UnityUtilities.Logger;

namespace Presenters {
    [Element(PrefabPath)]
    public class SelectionPresenter : MVCPresenter2D {
        public const string PrefabPath = "SelectionPanel";

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
        private MapItem currentMI = null;
        
        private void Start() {
            HideInfo();
        }
        
        private void HideInfo() {
            try {
                Icon.gameObject.SetActive(false);
                Name.gameObject?.SetActive(false);
                Description.gameObject?.SetActive(false);
                AbilitiesContainer.SetActive(false);
                if (AbilityDocumentation == null) 
                    return;
                AbilityDocumentation.text = "";
                AbilityDocumentation.gameObject.SetActive(false);
            }
            catch (UnassignedReferenceException){ }
        }

        public void handleSelection(MapItem mi, string layerName) {
            // TODO: This function "works", but not very well. Rather than firing the click event on everything below the cursor, an event layer should pass the event on to the relevant tile.
            //if(Time.time < timer && Math.Abs(Cooldown) > 0.001f) {
            //    if(DebugSelection)
            //        Logger.Log($"Selection for {mi.name} is on cooldown (-{timer - Time.time}s)");
            //    return;
            //};

            timer = Time.time + Cooldown;

            if(DebugSelection)
                Logger.UnityLog($"Selected {mi}");
            if(AbilityDocumentation != null) {
                AbilityDocumentation.GetComponent<Text>().text = "";
            }
            
            

//            if (Time.time < timer) {
//                if (mi is SoftwareTool)
//                    currentMI = mi;
//                else if (tile.MapItem.Value is TrailTile)
//                    currentMI = ((TrailTile) mi).Trail.Head;
//                
//            }

            SoftwareTool tool = mi as SoftwareTool;

            TrailTile item = mi as TrailTile;
            if(item != null) {
                TrailTile trail = item;
                tool = trail.Trail.Head;
            } else if(mi is SoftwareTool) {
                tool = (SoftwareTool) mi;
            }
            bool sentry = (tool is Sentry);

            if(tool == null) {
                HideInfo();
                return;
            }

            Icon.sprite = mi.sprite;
            Name.text = mi.name;
            Description.text = mi.description;

            foreach(Transform child in AbilitiesContainer.transform) {
                Destroy(child.gameObject);
            }

            foreach(var ability in tool.Attacks) {
                GameObject button = Instantiate(AbilityPrefab, AbilitiesContainer.transform.position,
                    Quaternion.identity, AbilitiesContainer.transform);
                Text buttonText = button.transform.Find("Name").GetComponent<Text>();

                if(buttonText == null) {
                    Logger.UnityLog("Could not create a button for an ability/attack!", LogLevels.ERROR);
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
