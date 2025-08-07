using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Hathora.Core.Scripts.Runtime.Common.Utils;
using Hathora.Core.Scripts.Runtime.Server;
using Hathora.Core.Scripts.Runtime.Server.Models;
using HathoraCloud.Models.Shared;
using Fusion.Photon.Realtime;
using Fusion.Sockets;

namespace Fusion.Addons.Hathora
{
    public class SimplefiedHathoraServer : HathoraServerMgr
    {
        [SerializeField]
        private NetworkRunner _runnerPrefab;

        private List<ServerPeer> _serverPeers = new List<ServerPeer>();

        public async Task<bool> Initialize(NetworkRunner runnerPrefab)
        {
            _runnerPrefab = runnerPrefab;
            
            DebugUtils.Log("Initializing Hathora server...");
            // Get the Hathora server context
            HathoraServerContext serverContext = await GetCachedServerContextAsync();
            if (serverContext == null)
            {
                DebugUtils.LogError("Server initialization failed! Missing HathoraServerContext.");
                return false;
            }
            DebugUtils.Log("Server Async is not null");
            // Refresh active servers periodically
            RefreshServers();
            return true;
        }

        private async void RefreshServers()
        {
            DebugUtils.Log("Inside Refresh Servers");
            while (_runnerPrefab != null)
            {
                HathoraServerContext serverContext = await GetCachedServerContextAsync();
                if (serverContext == null)
                {
                    await Task.Delay(1000);
                    continue;
                }

                // Check for destroyed rooms and deinitialize server peers
                foreach (var peer in _serverPeers)
                {
                    if (FindActiveRoom(serverContext, peer.RoomId) == null)
                    {
                        DebugUtils.Log($"Room destroyed: {peer.RoomId}");
                        await Deinitialize(peer);
                    }
                }

                // Initialize new server for active rooms
                foreach (var room in serverContext.ActiveRoomsForProcess)
                {
                    if (FindServerPeer(room.RoomId) == null)
                    {
                        DebugUtils.Log($"New room created: {room.RoomId}");
                        await InitializeServerPeer(_runnerPrefab, serverContext, room);
                    }
                }

                await Task.Delay(1000);
            }
        }

        private async Task<bool> InitializeServerPeer(NetworkRunner runnerPrefab, HathoraServerContext serverContext, RoomWithoutAllocations room)
        {
            DebugUtils.Log("Inside InitializeServerPeer");
            if (!FindUnusedPort(serverContext, out ProcessV3ExposedPort exposedPort))
            {
                DebugUtils.Log("No available ports to initialize a new server.");
                return false;
            }

            // Setup server and network addresses
            IPAddress ip = await HathoraUtils.ConvertHostToIpAddress(exposedPort.Host);
            ushort publicPort = (ushort)exposedPort.Port;
            NetAddress address = NetAddress.Any((ushort)exposedPort.Port);

            // Configure Fusion server settings, set max players to 10
            StartGameArgs startGameArgs = new StartGameArgs
            {
                SessionName = room.RoomId,
                GameMode = GameMode.Server,
                Address = address,
                PlayerCount = 10,  // Max players set to 10
                IsVisible = true,
                IsOpen = true
            };

            // Set best possible region
            FusionAppSettings photonAppSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
            photonAppSettings.FixedRegion = HathoraRegionUtility.HathoraToPhoton(serverContext.EnvVarRegion);
            startGameArgs.CustomPhotonAppSettings = photonAppSettings;

            // Set scene info for successful server creation
            const int multiplayerSceneIndex = 2; // Your target scene index
            NetworkSceneInfo sceneInfo = new NetworkSceneInfo();
            sceneInfo.AddSceneRef(SceneRef.FromIndex(multiplayerSceneIndex), LoadSceneMode.Single, LocalPhysicsMode.None, true);
            startGameArgs.Scene = sceneInfo;

            ServerPeer serverPeer = new ServerPeer(room.RoomId, publicPort, (ushort)exposedPort.Port, exposedPort.Name);
            _serverPeers.Add(serverPeer);

            serverPeer.Runner = Instantiate(runnerPrefab);
            StartGameResult result = await serverPeer.Runner.StartGame(startGameArgs);

            if (!result.Ok)
            {
                DebugUtils.LogError($"Failed to start server for RoomId: {room.RoomId}");
                await Deinitialize(serverPeer);
                return false;
            }

            DebugUtils.Log($"Server started for RoomId: {room.RoomId} at IP: {ip} on Port: {publicPort}. Max Players: 10 and region: {startGameArgs.CustomPhotonAppSettings.FixedRegion}");

            // Load the game scene if the server creation is successful
            SceneManager.LoadScene(multiplayerSceneIndex, LoadSceneMode.Single);
            DebugUtils.Log("Scene loaded successfully.");
            return true;
        }

        private async Task Deinitialize(ServerPeer serverPeer)
        {
            Debug.Log($"Deinitializing server for RoomId: {serverPeer.RoomId}");
            _serverPeers.Remove(serverPeer);

            if (serverPeer.Runner != null)
            {
                await serverPeer.Runner.Shutdown();
                serverPeer.Runner = null;
            }
        }


        private RoomWithoutAllocations FindActiveRoom(HathoraServerContext serverContext, string roomId)
        {
            return serverContext?.ActiveRoomsForProcess?.Find(room => room.RoomId == roomId);
        }

        private ServerPeer FindServerPeer(string roomId)
        {
            return _serverPeers.Find(peer => peer.RoomId == roomId);
        }

        private bool FindUnusedPort(HathoraServerContext serverContext, out ProcessV3ExposedPort port)
        {
            port = serverContext?.ProcessInfo.ExposedPort;
            return port != null && FindServerPeer(port.Name) == null;
        }

        // Simple class for server peer management
        private class ServerPeer
        {
            public string RoomId { get; }
            public ushort PublicPort { get; }
            public ushort ContainerPort { get; }
            public string PortName { get; }
            public NetworkRunner Runner { get; set; }

            public ServerPeer(string roomId, ushort publicPort, ushort containerPort, string portName)
            {
                RoomId = roomId;
                PublicPort = publicPort;
                ContainerPort = containerPort;
                PortName = portName;
            }
        }

    }
}
