using System;
using Level.Entity;

namespace Action.Ability {
	public class TemporalStatusEffect {
		public readonly SoftwareTool Source;
		public readonly SoftwareTool Target;
		public readonly string Name;
		public int Counter = -1;
		protected bool Active = true;
		public readonly StatusEffectOverallType OverallType;
		public readonly AttributeTargetType AttributeTargetType;
		public readonly int Value;

		public bool IsActive => Active;

		protected TemporalStatusEffect() { }

		public TemporalStatusEffect(string name, SoftwareTool source, SoftwareTool target, StatusEffectOverallType overallType, AttributeTargetType attributeTargetType, int value, int turnsActive = -1) {
			Name = name;
			Source = source;
			Target = target;
			AttributeTargetType = attributeTargetType;
			Value = value;
			OverallType = overallType;
			Counter = turnsActive;
		}

		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) 
				return false;

			TemporalStatusEffect effect = (TemporalStatusEffect)obj;

			if (Source.Equals(effect.Source) && Target.Equals(effect.Target) && OverallType.Equals(effect.OverallType))
				return false;

			return Name == effect.Name;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public void Tick() {
			// -1 is a permanent effect
			if (Counter == -1)
				return;
			if (Counter == 0) {
				Active = false;
				return;
			}
			Counter--;
		}

		public void Disable() {
			Active = false;
		}

		public int CalculateResult() {
			if (AttributeTargetType == AttributeTargetType.ACTIVE)
				return Value > 0? 1 : 0;
			else if (AttributeTargetType == AttributeTargetType.ABILITIES)
				throw new NotImplementedException();
			if (OverallType == StatusEffectOverallType.BUFF_SET) {
				if (AttributeTargetType == AttributeTargetType.HEALTH) {
					return Value - Target.CurrentHealth;
				} else if (AttributeTargetType == AttributeTargetType.MOVEMENT) {
					return Value - Target.Movement;					
				}
			} else {
				if (AttributeTargetType == AttributeTargetType.HEALTH) {
					return Value;
				} else if (AttributeTargetType == AttributeTargetType.MOVEMENT) {
					return Value;					
				} 
			}
			return 0;
		}
	}
}