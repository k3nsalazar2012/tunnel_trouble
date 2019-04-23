using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TunnelTrouble
{
    public class Player : MonoBehaviour
    {
        [SerializeField]
        private TunnelSystem tunnelSystem;

        [SerializeField]
        private float velocity;

        [SerializeField]
        private float rotationVelocity;

        private Tunnel currentTunnel;
        private float distanceTraveled;
        private float deltaToRotation;
        private float systemRotation;
        private Transform world;
        private float worldRotation, avatarRotation;
        private Transform rotater;

        private void Start()
        {
            world = tunnelSystem.transform.parent;
            rotater = transform.GetChild(0);
            currentTunnel = tunnelSystem.SetupFirstTunnel();
            SetupCurrentTunnel();
        }

        private void FixedUpdate()
        {
            float delta = velocity * Time.deltaTime;
            distanceTraveled += delta;
            systemRotation += deltaToRotation * deltaToRotation;

            if (systemRotation >= currentTunnel.CurveAngle)
            {
                delta = (systemRotation - currentTunnel.CurveAngle) / deltaToRotation;
                currentTunnel = tunnelSystem.SetupNextTunnel();
                SetupCurrentTunnel();
                systemRotation = delta * deltaToRotation;
            }

            tunnelSystem.transform.localRotation = Quaternion.Euler(0f, 0f, systemRotation);
            UpdateAvatarRotation();
        }

        private void UpdateAvatarRotation()
        {
            avatarRotation +=
                rotationVelocity * Time.deltaTime * Input.GetAxis("Horizontal");
            if (avatarRotation < 0f)
            {
                avatarRotation += 360f;
            }
            else if (avatarRotation >= 360f)
            {
                avatarRotation -= 360f;
            }
            rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
        }
        private void SetupCurrentTunnel()
        {
            deltaToRotation = 360f / (2f * Mathf.PI * currentTunnel.CurveRadius);
            worldRotation += currentTunnel.RelativeRotation;
            if (worldRotation < 0f)
            {
                worldRotation += 360f;
            }
            else if (worldRotation >= 360f)
            {
                worldRotation -= 360f;
            }
            world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
        }
    }
}