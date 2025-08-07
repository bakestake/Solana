using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sample.DedicatedServer.Utils;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using System.Net;
using Fusion.Photon.Realtime;

public abstract class ServerManagerBase : MonoBehaviour
{
    /// <summary>
    /// Network Runner Prefab used to Spawn a new Runner used by the Server
    /// </summary>
    [SerializeField] protected NetworkRunner _runnerPrefab;

    protected Task<StartGameResult> StartSimulation(NetworkRunner runner, DedicatedServerConfig serverConfig) => StartSimulation(
      runner,
      serverConfig.SessionName,
      serverConfig.SessionProperties,
      serverConfig.Port,
      serverConfig.Lobby,
      serverConfig.Region,
      serverConfig.PublicIP,
      serverConfig.PublicPort
    );

    protected Task<StartGameResult> StartSimulation(
      NetworkRunner runner,
      string SessionName,
      Dictionary<string, SessionProperty> customProps = null,
      ushort containerPort = 0,
      string customLobby = null,
      string customRegion = null,
      string customPublicIP = null,
      ushort customPublicPort = 0
    )
    {
        string logPrefix = $"[{nameof(ServerManagerBase)}.{nameof(StartSimulation)}]";

        // Build Custom Photon Config
        var photonSettings = PhotonAppSettings.Global.AppSettings.GetCopy();

        if (string.IsNullOrEmpty(customRegion) == false)
        {
            photonSettings.FixedRegion = customRegion.ToLower();
        }

        // Build Custom External Addr
        NetAddress? externalAddr = null;

        // Parse custom public IP
        if (string.IsNullOrEmpty(customPublicIP) == false && customPublicPort > 0)
        {
            if (IPAddress.TryParse(customPublicIP, out var _))
            {
                Log.Info($"{logPrefix} Preparing to parse ip:port from env vars: `{customPublicIP}:{customPublicPort}`");
                externalAddr = NetAddress.CreateFromIpPort(customPublicIP, customPublicPort);
            }
            else
            {
                Log.Error($"{logPrefix} Unable to parse 'Custom Public IP' - " +
                    "we may run as a relay instead of a direct connection");
            }
        }

        Log.Info($"{logPrefix} {nameof(externalAddr)} == `{externalAddr}` | " +
            $"{nameof(containerPort)} == {containerPort}");

        // Start Runner
        return runner.StartGame(new StartGameArgs()
        {
            PlayerCount = 10,
            SessionName = SessionName,
            GameMode = GameMode.Server,
            SessionProperties = customProps,

            #region >> Important for Hathora Deployment >>

            Address = NetAddress.Any(containerPort), // Default == 7777
            CustomPublicAddress = externalAddr,

            #endregion // << Important for Hathora Deployment <<

            CustomLobbyName = customLobby,
            CustomPhotonAppSettings = photonSettings,
        });
    }

}
