using UnityEngine;
using Fusion;

namespace DonzaiGamecorp.TicTacToe
{
    public class FusionManager : MonoBehaviour
    {
        public static FusionManager Instance { get; private set; }

        [Header("Network Runner")]
        [SerializeField] private NetworkRunner _networkRunnerPrefab = null;
        public NetworkRunner Runner = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        private void CreateRunner()
        {
            Runner = FindObjectOfType<NetworkRunner>();
            if (Runner == null)
            {
                Runner = Instantiate(_networkRunnerPrefab);
            }
            // Let the Fusion Runner know that we will be providing user input
            Runner.ProvideInput = true;
        }

        public async void GameRoomHost(string sessionName)
        {
            CreateRunner();

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Host,
                SessionName = sessionName,
                PlayerCount = 2,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await Runner.StartGame(startGameArgs);
        }

        public async void GameRoomClient(string sessionName)
        {
            CreateRunner();

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                PlayerCount = 2,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await Runner.StartGame(startGameArgs);
            if (Runner.SessionInfo.PlayerCount == 1)
            {
                Debug.Log("Invalid Room ID");
                await Runner.Shutdown();
            }
        }

        public async void GameRoomAutoMatch()
        {
            CreateRunner();

            var startGameArgs = new StartGameArgs()
            {
                GameMode = GameMode.AutoHostOrClient,
                PlayerCount = 2,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await Runner.StartGame(startGameArgs);
        }
    }
}

