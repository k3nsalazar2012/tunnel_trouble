using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TunnelTrouble
{
    public class TunnelSystem : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private Tunnel tunnelPrefab;

        [SerializeField]
        private int tunnelCount;

        private Tunnel[] tunnels; 
        #endregion

        private void Awake()
        {
            tunnels = new Tunnel[tunnelCount];
            for (int i = 0; i < tunnelCount; i++)
            {
                Tunnel tunnel = tunnels[i] = Instantiate<Tunnel>(tunnelPrefab);
                tunnel.transform.SetParent(transform, false);
                tunnel.Generate();
                if (i > 0)
                    tunnel.AlignWith(tunnels[i - 1]);
            }
            AlignNextTunnelWithOrigin();
        }

        public Tunnel SetupFirstTunnel()
        {
            transform.localPosition = new Vector3(0f, -tunnels[1].CurveRadius);
            return tunnels[1];
        }

        public Tunnel SetupNextTunnel()
        {
            ShiftTunnels();
            AlignNextTunnelWithOrigin();
            tunnels[tunnels.Length - 1].Generate();
            tunnels[tunnels.Length - 1].AlignWith(tunnels[tunnels.Length - 2]);
            transform.localPosition = new Vector3(0f, -tunnels[1].CurveRadius);
            return tunnels[1];
        }

        private void ShiftTunnels()
        {
            Tunnel temp = tunnels[0];
            for (int i = 1; i < tunnels.Length; i++)
            {
                tunnels[i - 1] = tunnels[i];
            }
            tunnels[tunnels.Length - 1] = temp;
        }

        private void AlignNextTunnelWithOrigin()
        {
            Transform transformToAlign = tunnels[1].transform;
            for (int i = 0; i < tunnels.Length; i++)
            {
                if (i != 1)
                {
                    tunnels[i].transform.SetParent(transformToAlign);
                }
            }

            transformToAlign.localPosition = Vector3.zero;
            transformToAlign.localRotation = Quaternion.identity;

            for (int i = 0; i < tunnels.Length; i++)
            {
                if (i != 1)
                {
                    tunnels[i].transform.SetParent(transform);
                }
            }
        }
    }
}