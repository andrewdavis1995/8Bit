using UnityEngine;

namespace Assets.Scripts
{
    class PunchableObject : MonoBehaviour
    {
        public float Health;
        public float DamangePerPunch;
        public Transform PrefabToCreate;

        public void Punched()
        {
            Health -= DamangePerPunch;
            if(Health <= 0)
            {
                if (PrefabToCreate)
                {
                    var creator = GameObject.Find("CollectableCreator").GetComponent<CollectableCreationScript>();
                    creator.Create(PrefabToCreate, transform.position);
                }
                // give out any collectables
                Destroy(gameObject);
            }
        }

    }
}
