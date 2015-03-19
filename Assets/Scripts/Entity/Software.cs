// Additional class definitions exist here.
/* E.g., public partial class software{
 *      public int some_id;
*   }  
* 
*/

using System;


namespace SacredSkull.Software {

    public abstract class Attackable {
        private int healthField = 1;

        public int health {
            get {
                return healthField;
            }
            set {
                healthField = value;
            }
        }

        public int movement {
            get;
            set;
        }
    }

    public partial class softwareTool
    {
        public int health
        {
            get;
            set;
        }
    }

    public partial class softwareToolAttacksBasicattack{
        public bool Attack(softwareTool target, softwareTool source)
        {
            if (!(source.isEnemy) && !(target.isEnemy)) {
                return false;
            }
            else if (source.isEnemy && target.isEnemy)
            {
                throw new Exception("Traitor sentry program detected!");
            }
            target.health -= this.damage;
            return true;
        }
    }

    public partial class softwareToolAttacksAttributemodifier{
        public bool Attack(softwareTool target, softwareTool source)
        {
            foreach (softwareToolAttacksAttributemodifierAttribute attribute in this.attribute) {
                if (attribute.name == "movement") {
                    if(attribute.value == 0)
                        target.movement = 0;
                    else
                        target.movement += attribute.value;
                } else if (attribute.name == "health") {
                    target.health += attribute.value;
                } else if (attribute.name == "poison") {
                    throw new NotImplementedException();
                }
            }
            return true;

        }
    }

    public partial class softwareToolAttacksMapmodifier {
        public void Attack(int x, int y, Attackable source) {
            throw new NotImplementedException();
        }
    }
}