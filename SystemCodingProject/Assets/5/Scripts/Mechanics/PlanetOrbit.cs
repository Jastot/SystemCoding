using Data;
using Network;
using UnityEngine;

namespace Mechanics
{
    public class PlanetOrbit : NetworkMovableObject
    {
        protected override float speed => PlanetOrbitData.smoothTime;
        public PlanetOrbitData PlanetOrbitData;
        public Transform aroundPoint;
        [HideInInspector]
        public float rotationSpeed;
        [HideInInspector]
        public float circleInSecond = 1f;
        
        private float dist;
        private float currentAng;
        private Vector3 currentPositionSmoothVelocity;
        private float currentRotationAngle;

        private const float circleRadians = Mathf.PI * 2;

        public void Start()
        {
            aroundPoint = FindObjectOfType<Solar>().transform;
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
            p.x += Mathf.Sin(currentAng) * dist * PlanetOrbitData.offsetSin;
            p.z += Mathf.Cos(currentAng) * dist * PlanetOrbitData.offsetCos;
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
