using Unity.VisualScripting;
using UnityEngine.UIElements;

namespace Units.Enemies
{
    public class RegularEnemy : Pathfinding
    {
        public int health;
        public int id;
        static int idGenerator = 0;
        
        public RegularEnemy(int health)
        {
            this.health = health;
            id = idGenerator++;
        }
        public void Move()
        {
           
        }
        private void Update()
        {
            if(health <= 0)
                Destroy(this);

        }

        public void TakeHit(int damage)
        {
            health -= damage;
        }
    }
}