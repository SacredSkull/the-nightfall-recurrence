using System.Collections.Generic;
using System.Linq;

namespace Action.AI {
	public sealed class GovernorNames {
		private static Dictionary<string, GovernorNames> dict = new Dictionary<string, GovernorNames>(); 
		
		private readonly string name;
		private readonly int value;

		public static readonly GovernorNames STANDARD = new GovernorNames (1, "Standard");
		public static readonly GovernorNames RANGED = new GovernorNames (2, "Ranged");
		public static readonly GovernorNames PLAYER = new GovernorNames (3, "Player");        

		private GovernorNames(int value, string name){
			this.name = name;
			this.value = value;
			dict[name] = this;
		}

		public override string ToString(){
			return name;
		}

		public static GovernorNames Find(string name) {
			return dict.FirstOrDefault(x => x.Key == name).Value;
		}
	}
}