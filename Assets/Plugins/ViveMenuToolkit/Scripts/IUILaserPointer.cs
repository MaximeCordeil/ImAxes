using UnityEngine;
using UnityEngine.EventSystems;

namespace Wacki {
    abstract public class IUILaserPointer : MonoBehaviour {

        public float LaserThickness = 0.002f;
        public float LaserHitScale = 0.02f;
        public bool LaserAlwaysOn = false;
        public bool DisableLaser = false;
        public Color Color;

        private GameObject hitPoint;
        private GameObject pointer;

        // Used to move menus, locks the hitPoint at a given distance when the move button is pressed.
        private float _lockDistance;
        private bool _locked;
        public void Lock(bool b){ _locked = b; }
        public Transform GetHitPoint() { return hitPoint.transform; }

        private bool initialisedLaser = false;


        private float _distanceLimit;

        // Use this for initialization
        void Start()
        {
            if (!DisableLaser)
            {
                InitialiseLaser();

                // initialize concrete class
                Initialize();

                // register with the LaserPointerInputModule
                if (LaserPointerInputModule.instance == null)
                {
                    var es = FindObjectOfType<EventSystem>();
                    if (es != null)
                    {
                        es.gameObject.AddComponent<LaserPointerInputModule>();
                    }
                    else
                    {
                        new GameObject().AddComponent<LaserPointerInputModule>();
                    }
                }

                LaserPointerInputModule.instance.AddController(this);
            }
        }

        // 
        private void InitialiseLaser()
        {
            if (!initialisedLaser)
            {
                pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pointer.transform.SetParent(transform, false);
                pointer.transform.localScale = new Vector3(LaserThickness, LaserThickness, 100.0f);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

                hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                hitPoint.transform.SetParent(transform, false);
                hitPoint.transform.localScale = new Vector3(LaserHitScale, LaserHitScale, LaserHitScale);
                hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

                hitPoint.SetActive(false);

                // remove the colliders on our primitives
                Object.DestroyImmediate(hitPoint.GetComponent<SphereCollider>());
                Object.DestroyImmediate(pointer.GetComponent<BoxCollider>());

                Material newMaterial = new Material(Shader.Find("Wacki/LaserPointer"));

                //pointer.AddComponent<MeshRenderer>();
                //hitPoint.AddComponent<MeshRenderer>();

                newMaterial.SetColor("_Color", Color);
                pointer.GetComponent<MeshRenderer>().material = newMaterial;
                hitPoint.GetComponent<MeshRenderer>().material = newMaterial;

                this.initialisedLaser = true;
            }
        }

        void OnDestroy()
        {
            if(LaserPointerInputModule.instance != null)
                LaserPointerInputModule.instance.RemoveController(this);
        }

        // This method allows the laser to be turned on and off at runtime.
        public void SetLaserAlwaysOn(bool value)
        {
            if(value) // Turning laser on.
            {
                InitialiseLaser();
            } else // Turning laser off.
            {
                pointer.GetComponent<MeshRenderer>().enabled = false;
                hitPoint.GetComponent<MeshRenderer>().enabled = false;
            }
            this.LaserAlwaysOn = value;
        }

        protected virtual void Initialize() { }
        public virtual void OnEnterControl(GameObject control) { }
        public virtual void OnExitControl(GameObject control) { }
        abstract public bool ButtonDown();
        abstract public bool ButtonUp();

        protected virtual void Update()
        {
            if (!DisableLaser)
            {
                Ray ray = new Ray(transform.position, transform.forward);

                RaycastHit hitInfo;
                bool bHit = Physics.Raycast(ray, out hitInfo);

                float distance = 100.0f;

                if (bHit)
                {
                    if (!_locked)
                    {
                        distance = hitInfo.distance;
                    }
                    else
                    {
                        distance = _lockDistance;
                    }
                }

                // ugly, but has to do for now
                if (_distanceLimit > 0.0f)
                {
                    distance = _locked ? _lockDistance : Mathf.Min(distance, _distanceLimit);
                    bHit = true;
                }

                pointer.transform.localScale = new Vector3(LaserThickness, LaserThickness, distance);
                pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

                if (bHit)
                {
                    _lockDistance = distance; // For moving menus.
                    hitPoint.SetActive(true);
                    hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
                }
                else
                {
                    hitPoint.SetActive(false);
                }

                //
                if (!LaserAlwaysOn)
                {
                    var show = hitInfo.collider != null && hitInfo.collider.gameObject != null && hitInfo.collider.gameObject.CompareTag("Menu");
                    pointer.GetComponent<MeshRenderer>().enabled = show;
                    hitPoint.GetComponent<MeshRenderer>().enabled = show;
                }

                // reset the previous distance limit
                _distanceLimit = -1.0f;
            }
        }

        // limits the laser distance for the current frame
        public virtual void LimitLaserDistance(float distance)
        {
            if(distance < 0.0f)
                return;

            if(_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }
    }

}
