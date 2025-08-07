using UnityEngine;
using Fusion;
using Fusion.Addons.Hathora;
using Fusion.Menu;

namespace Bakeland
{
	public class HathoraManager : FusionMenuConnectionPlugin
	{
		public NetworkRunner RunnerPrefab;
		//public HathoraServer ServerPrefab;
		public SimplefiedHathoraServer ServerPrefab;
		public HathoraClient ClientPrefab;

		//private HathoraServer _serverInstance;
		private SimplefiedHathoraServer _serverInstance;
		private HathoraClient _clientInstance;

        public SimplefiedHathoraServer GetOrCreateServerInstance()
        {
            if (_serverInstance == null)
            {
                _serverInstance = Instantiate(ServerPrefab);
            }

            return _serverInstance;
        }

        public HathoraClient GetOrCreateClientInstance()
		{
			if (_clientInstance == null)
			{
				_clientInstance = Instantiate(ClientPrefab);
			}

			return _clientInstance;
		}

		public override IPhotonMenuConnection Create(FusionMenuConnectionBehaviour connectionBehaviour) => new HathoraMenuConnection(connectionBehaviour, this);

		private async void Awake()
		{
			if (Application.isEditor == false)
			{
				if (Application.platform == RuntimePlatform.LinuxServer || Application.platform == RuntimePlatform.WindowsServer || Application.platform == RuntimePlatform.OSXServer || Application.isBatchMode)
				{
					//HathoraServer hathoraServer = GetOrCreateServerInstance();
					SimplefiedHathoraServer hathoraServer = GetOrCreateServerInstance();
					if (hathoraServer != null)
					{
						bool result = await hathoraServer.Initialize(RunnerPrefab);

						if (result == false)
						{
							Application.Quit();
						}
					}
				}
			}
		}
	}
}
