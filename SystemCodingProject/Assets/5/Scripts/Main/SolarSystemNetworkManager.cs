using System;
using System.Collections;
using System.Collections.Generic;
using Characters;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        private string playersName;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.name = playersName;
            player.GetComponent<ShipController>().PlayerName =playersName;
            
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
        
        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            //
            Debug.Log("OnClientConnect");
            StartCoroutine(SetClientParams());
        }

        private IEnumerator SetClientParams()
        {
            yield return new WaitForSeconds(1);
            Debug.Log("after 1 sec");
            foreach (var c in ClientScene.localPlayers)
            {
                var shipC = c.gameObject.GetComponent<ShipController>();
                if (shipC) {
                    shipC.name = playersName;
                    shipC.PlayerName =playersName;
                }
            }
        }
        public void SetName(string name)
        {
            playersName = name;
        }
    }
}
