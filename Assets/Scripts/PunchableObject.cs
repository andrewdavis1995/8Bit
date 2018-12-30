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
                    var obj = Instantiate(PrefabToCreate, transform.position, Quaternion.identity);
                    obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-15f, 15f), 200));
                }
                // give out any collectables
                Destroy(gameObject);
            }
        }

    }
}
