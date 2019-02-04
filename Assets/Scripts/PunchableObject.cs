using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    class PunchableObject : MonoBehaviour
    {
        public float Health;
        public float DamangePerPunch;
        public Transform PrefabToCreate;
        private SpriteRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

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
            StartCoroutine(WaitAfterBounceback());
            _renderer.color = new Color(0.8f, 0.2f, 0.2f);
        }

        private IEnumerator WaitAfterBounceback()
        {
            yield return new WaitForSeconds(.05f);
            _renderer.color = new Color(1, 1, 1);
        }
    }
}
