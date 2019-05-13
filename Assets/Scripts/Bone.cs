    using UnityEngine;

    public class Bone
    {
        public GameObject GameObject { get; }
        public Transform InitialPosition { get; }

        public Bone (GameObject gameObject)
        {
            if (gameObject != null)
            {
                GameObject = gameObject;
                InitialPosition = gameObject.transform;    
            }
        }
        
    }
