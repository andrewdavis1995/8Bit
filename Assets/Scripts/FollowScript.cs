using UnityEngine;

namespace Assets.Scripts
{
    public class FollowScript : MonoBehaviour
    {
        private float _xPosition = -9999;
        private float _timeCount = 1;

        private PlayerScript _target;
        public PersonInteractionScript interactionScript;

        private SpriteRenderer _renderer;

        public float Interval;
        public float Boundaries;
        public float Speed;


        void Start()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();

            // test
            _target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
            // end test

        }

        public void SetTarget(PlayerScript target)
        {
            _target = target;
        }


        void Update()
        {
            if (_target == null) return;

            if (interactionScript != null && interactionScript.IsInteracting()) return;

            _timeCount += Time.deltaTime;
            if (_timeCount > Interval)
            {
                _timeCount = 0;
                if (_target.Grounded())
                {
                    int skipFactor = UnityEngine.Random.Range(0, 7);
                    if (skipFactor != 0) // randomise
                    {
                        _xPosition = _target.transform.position.x;
                        //Interval = UnityEngine.Random.Range(3f, 7f);
                        //Boundaries = UnityEngine.Random.Range(1f, 2f);
                    }
                }
            }

            if (_xPosition == -9999) return;

            if (transform.position.x < _xPosition - Boundaries)
            {
                transform.Translate(new Vector3(Speed * Time.deltaTime, 0, 0));
                _renderer.flipX = false;
            }
            else if (transform.position.x > _xPosition + Boundaries)
            {
                transform.Translate(new Vector3(-Speed * Time.deltaTime, 0, 0));

                _renderer.flipX = true;
            }
            else
            {
                _xPosition = -9999;
            }
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            // check tag. If stopper, stop
        }

        internal void Reset()
        {
            enabled = false;
            _xPosition = -9999;
        }
    }
}