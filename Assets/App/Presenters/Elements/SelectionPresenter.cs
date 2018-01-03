using Action.Ability;
using Controllers;
using Karma.Metadata;
using Level;
using Level.Entity;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityUtilities.Management;
using Zenject;

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
        public Button EndTurnButton;

        private float timer;
        public MapItem currentMI = null;
        private TurnController _turnController;
        
        [Inject]
        private void Inject(TurnController turnController) {
            _turnController = turnController;
        }

        private void Start() {
            HideInfo();
        }
        
        private void HideInfo() {
            try {
                Icon.sprite = null;
                Name.text = "";
                Description.text = "";
                foreach(Transform child in AbilitiesContainer.transform) {
                    Destroy(child.gameObject);
                }
                if (AbilityDocumentation == null) 
                    return;
                AbilityDocumentation.text = "";
            }
            catch (UnassignedReferenceException){ }
        }

        public void HandleSelection(MapItem mi, string layerName) {
            HideInfo();
            
            timer = Time.time + Cooldown;

            if(DebugSelection)
                logger.Log($"Selected {mi}");
            if(AbilityDocumentation != null) {
                AbilityDocumentation.GetComponent<Text>().text = "";
            }
            currentMI = mi;

            SoftwareTool tool = mi as SoftwareTool;

            switch (mi) {
                case TrailTile item:
                    TrailTile trail = item;
                    tool = trail.Trail.Head;
                    break;
                case SoftwareTool _:
                    tool = (SoftwareTool) mi;
                    break;
            }
            
            bool sentry = tool is Sentry;
            
            if(tool is HackTool ht)
                _turnController.PlayerSelectedTool(ht);
            else
                _turnController.PlayerDeselectedTool();

            Icon.sprite = mi.sprite;
            Name.text = mi.name;
            Description.text = mi.description;

            if (tool?.Attacks == null)
                return;
            foreach (var ability in tool.Attacks) {
                GameObject button = Instantiate(AbilityPrefab, AbilitiesContainer.transform.position,
                    Quaternion.identity, AbilitiesContainer.transform);
                Text buttonText = button.transform.Find("Name").GetComponent<Text>();

                if (buttonText == null) {
                    logger.Log("Could not create a button for an ability/attack!", LogLevels.ERROR);
                    continue;
                }

                buttonText.text = ability.Name;
                button.GetComponent<Button>().OnClickAsObservable().Subscribe(x => { abilityButtonHandler(ability, sentry); });
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
