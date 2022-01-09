using Data;
using Network;
using UnityEngine;
using UnityEngine.Networking;

namespace Mechanics
{
    public class PlanetOrbit : NetworkMovableObject
    {
        [SyncVar]
        public string Name;
        protected override float speed => smoothTime;
        [SyncVar]
        public float smoothTime = .3f;
        [SyncVar]
        public float offsetSin = 1;
        [SyncVar]
        public float offsetCos = 1;
        [SyncVar]
        public int materialNumber;
        [SyncVar]
        public Vector3 scale;
        [SyncVar] 
        public Vector3 startPos;
        
        public Transform aroundPoint;
        
        public float rotationSpeed;
        
        public float circleInSecond = 0.1f;
        
        private float dist;
        private float currentAng;
        private Vector3 currentPositionSmoothVelocity;
        private float currentRotationAngle;

        private const float circleRadians = Mathf.PI * 2;

        public void Start()
        {
            aroundPoint = FindObjectOfType<Solar>().transform;
            transform.position = startPos;
            circleInSecond = speed / 100f;
            if (isServer)
            {
                dist = (transform.position - aroundPoint.position).magnitude;
            }
            Initiate(UpdatePhase.FixedUpdate);
        }

        protected override void HasAuthorityMovement()
        {
            if (!isServer)
            {
                return;
            }
            Vector3 p = aroundPoint.position;
            p.x += Mathf.Sin(currentAng) * dist * offsetSin;
            p.z += Mathf.Cos(currentAng) * dist * offsetCos;
            transform.position = p;
            currentRotationAngle += Time.deltaTime * rotationSpeed;
            currentRotationAngle = Mathf.Clamp(currentRotationAngle, 0, 361);
            if (currentRotationAngle >= 360)
            {
                currentRotationAngle = 0;
            }
            transform.rotation = Quaternion.AngleAxis(currentRotationAngle, transform.up);
            currentAng += circleRadians * circleInSecond * Time.deltaTime;

            SendToServer();
        }

        protected override void SendToServer()
        {
            serverPosition = transform.position;
            serverEuler = transform.eulerAngles;
        }

        protected override void FromServerUpdate()
        {
            if (!isClient)
            {
                return;
            }
            transform.position = Vector3.SmoothDamp(transform.position,
                serverPosition, ref currentPositionSmoothVelocity, speed);
            transform.rotation = Quaternion.Euler(serverEuler);
        }
    }
}
