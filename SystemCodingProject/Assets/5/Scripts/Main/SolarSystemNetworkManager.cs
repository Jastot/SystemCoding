using System.Collections;
using Characters;
using Mechanics;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Main
{
    public class SolarSystemNetworkManager : NetworkManager
    {
        [SerializeField] private TMP_InputField _playersName;
        public override void OnServerSceneChanged(string sceneName)
        {
            //base.OnServerSceneChanged(sceneName);
            Debug.Log("OnServerSceneChanged");
            CreateCustomSolarSystem();
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
        {
            var spawnTransform = GetStartPosition();

            var player = Instantiate(playerPrefab, spawnTransform.position, spawnTransform.rotation);
            player.name = conn+"";
            var ship = player.GetComponent<ShipController>();
            ship.PlayerName =conn+"";
            ship.ColisionDetected += deletePlayer;
            if (extraMessageReader != null)
            {
                var playerName = extraMessageReader.ReadMessage<StringMessage>().value;
                if (!string.IsNullOrEmpty(playerName))
                {
                    ship.PlayerName = playerName;
                }
            }
           
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        private void deletePlayer(string name)
        {
            foreach (var networkId in ClientScene.objects.Values)
            {
                if (networkId.gameObject.name == name && networkId.isClient)
                {
                    NetworkServer.Destroy(networkId.gameObject);
                    networkId.GetComponent<ShipController>().ColisionDetected -= deletePlayer;
                }
                    
            }
        }
        public override void OnClientConnect(NetworkConnection conn)
        {
            var message = new StringMessage();
            message.value = _playersName.text;
            _playersName.gameObject.SetActive(false);
            ClientScene.AddPlayer(conn, 0, message);
            StartCoroutine(SetClientParams());
        }

        private IEnumerator SetClientParams()
        {
            yield return new WaitForFixedUpdate();
            foreach (var c in ClientScene.localPlayers)
            {
                var shipC = c.gameObject.GetComponent<ShipController>();
                if (shipC) {
                    shipC.name = _playersName.text;
                    shipC.PlayerName =_playersName.text;
                }
            }

            foreach (var c in ClientScene.objects.Values)
            {
                var planet = c.GetComponent<PlanetOrbit>();
                if (planet)
                {
                    var obj = planet.gameObject;
                    obj.name = planet.Name;
                    var mat = Resources.LoadAll<Material>("Materials/");
                    planet.GetComponent<MeshRenderer>().material = mat[planet.materialNumber];
                    obj.transform.localScale = planet.scale;
                }
            }
        }

        private void CreateCustomSolarSystem()
        {
            var res = Resources.Load<MapData>("MapData");
            var planetPrefab = spawnPrefabs[0];
            foreach (var planetStruct in res.PlanetStructs)
            {
                var planet = Instantiate(planetPrefab,
                    new Vector3(planetStruct.Position.x,0,planetStruct.Position.y), Quaternion.identity);
                var orbit = planet.GetComponent<PlanetOrbit>();
                planet.name = planetStruct.PlanetName;
                planet.transform.localScale = new Vector3(planetStruct.Radius,planetStruct.Radius,planetStruct.Radius);

                orbit.startPos = new Vector3(planetStruct.Position.x, 0, planetStruct.Position.y);
                orbit.scale = planet.transform.localScale;
                orbit.offsetCos = planetStruct.PlanetOrbitData.offsetCos;
                orbit.offsetSin = planetStruct.PlanetOrbitData.offsetSin;
                orbit.smoothTime = planetStruct.PlanetOrbitData.smoothTime;
                orbit.rotationSpeed = planetStruct.Speed;
                orbit.materialNumber = planetStruct.Material; 
                orbit.Name = planet.name;
                var mat = Resources.LoadAll<Material>("Materials/");
                planet.GetComponent<MeshRenderer>().material = mat[planetStruct.Material];
                NetworkServer.Spawn(planet);
            }
        }

        public override void OnServerReady(NetworkConnection conn)
        {
        }
        
        public override void OnClientSceneChanged(NetworkConnection conn)
        {
        }
    }
}
