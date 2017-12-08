﻿using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using Action.Ability;
using Action.AI;
using UnityEngine;
using Zenject;

namespace Level.Entity {
    //TODO: Add the sound clip for movement in this class. It should be activated inside the move() function, which will also need a reference to the AudioQueue (via the ServiceLocator), which hasn't been made yet!
    //renderedTile.GetComponent<AudioSource>().clip = MovementClip;
    public class SoftwareTool : MapItem {
        [XmlIgnore]
        public int CurrentHealth = 1;
        [XmlAttribute("maxsize")]
        public int MaxHealth;
        [XmlAttribute("cost")]
        public short Cost;
        [XmlAttribute("level")]
        public short Level;
        [XmlAttribute("movement")]
        public int Movement { get; set; }
        [XmlArray("attacks")]
        [XmlArrayItem(ElementName = "attackbasic", Type = typeof(AttackBasic)),
         XmlArrayItem(ElementName = "attributemodifier", Type = typeof(AttributeModifier)),
         XmlArrayItem(ElementName = "mapmodifier", Type = typeof(MapModifier))]
        public List<Attack> Attacks { get; set; }

        [XmlAttribute("governor")]
        [DefaultValue("Standard")]
        public string GovernorName = "Standard";
        
        [XmlIgnore]
        public Governor Governor {
            get {
                return _gov;
            }
            set { _gov = value; }
        }
        [XmlIgnore]
        private Governor _gov;

        public delegate void AttackEventHandler(SoftwareTool victim, SoftwareTool perp, Attack candlestick);
        public static event AttackEventHandler DeathEvent;
        public static event AttackEventHandler AttackEvent;

        [XmlIgnore]
        protected Trail _Trail;

        [XmlIgnore]
        public Trail Tail {
            get {
                return _Trail ?? (_Trail = new Trail(this));
            }
        }

        public bool AtMaxSize => CurrentHealth == MaxHealth;

        [XmlIgnore]
        public Sprite TailSprite = null;

        public SoftwareTool() {
            gridPosition = new Vector2();
            _Trail = new Trail(this);
        }

        [Inject]
        public SoftwareTool(SoftwareTool blueprint, GovernorFactory governorFactory) : base(blueprint) {
            MaxHealth = blueprint.MaxHealth;
            Cost = blueprint.Cost;
            Level = blueprint.Level;
            CurrentHealth = blueprint.CurrentHealth;
            Movement = blueprint.Movement;
            Attacks = blueprint.Attacks;
            TailSprite = blueprint.TailSprite;
            _Trail = new Trail(this);
            _gov = governorFactory.Create(this, GovernorNames.Find(GovernorName));
        }

        public bool Attack(Attack attack, SoftwareTool target) {
            return Attacks.Contains(attack) && attack.Execute(target, this);
        }

        public bool IsEntirelyRanged {
            get {
                bool ranged = false;
                foreach(var attack in Attacks) {
                    if(attack.Range > 1)
                        ranged = true;
                }

                return ranged;
            }
        }

        public IEnumerable<Attack> PotentialAttacks(int distance) {
            return Attacks.Where(x => x.Range >= distance);
        }

        public Attack LongestRangeAttack => Attacks.OrderByDescending(x => x.Range).FirstOrDefault();

        public HashSet<TemporalStatusEffect> StatusEffects = new HashSet<TemporalStatusEffect>();

        public virtual void Move(Vector2 destination) {
            if(destination == gridPosition) return;

            SetPosition(destination);
            Tail.Move();
        }

        public virtual void Damage(int hp) {
            CurrentHealth -= hp;
            // This is very ugly!
            for(int i = 0; i < hp; i++)
                Tail.Shorten();
        }

        public virtual bool ReceiveAttack(Attack attack, SoftwareTool source) {
            if (CurrentHealth <= 0) {
                DeathEvent?.Invoke(this, source, attack);
                Delete();
            } else {
                AttackEvent?.Invoke(this, source, attack);
            }
            return true;
        }
    }
}