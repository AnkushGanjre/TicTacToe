using Fusion;

namespace DonzaiGamecorp.TicTacToe
{
    public class PlayerDataNetworked : NetworkBehaviour
    {
        [Networked] public string NickName { get; set; }
        [Networked] public int AvatarNum { get; set; }
        [Networked] public int TrophyCount { get; set; }
        [Networked] public int HostWonCount { get; set; } = 0;
        [Networked] public int ClientWonCount { get; set; } = 0;


        PlayerDataManager _playerDataManager;
        ChangeDetector _changeDetector;
        NetworkRunner _runnerInstance;
        int lastGame = 0;

        public override void Spawned()
        {
            _runnerInstance = Object.Runner;
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            _playerDataManager = FindObjectOfType<PlayerDataManager>();

            if (Object.HasInputAuthority)
            {
                _playerDataManager.LocalPlayerObj = Object;
                RPC_SetPlayerDetails(_playerDataManager.NickName, _playerDataManager.PlayerAvatarNum);
                UIController.Instance.PlayerNameText.text = _playerDataManager.NickName;
                UIController.Instance.PlayerAvatarImg.sprite = UIController.Instance.AllAvatars[_playerDataManager.PlayerAvatarNum - 1];
            }
            else
            {
                _playerDataManager.RemotePlayerObj = Object;
                if (Object.HasStateAuthority) return;
                _playerDataManager.OpponentName = Object.GetComponent<PlayerDataNetworked>().NickName;
                _playerDataManager.OpponentAvatarNum = Object.GetComponent<PlayerDataNetworked>().AvatarNum;

                UIController.Instance.OpponentNameText.text = _playerDataManager.OpponentName;
                UIController.Instance.OpponentAvatarImg.sprite = UIController.Instance.AllAvatars[_playerDataManager.OpponentAvatarNum - 1];
            }

            if (Object.HasStateAuthority)
            {
                UIController.Instance.SetLocalPlayerSymbolX();
                UIController.Instance.GameInfoText.text = "Your Turn";
                UIController.Instance.GameScoreText.text = $"{HostWonCount} - {ClientWonCount}";
            }
            else
            {
                UIController.Instance.SetLocalPlayerSymbolO();
                UIController.Instance.GameInfoText.text = "<size=30>Wait For Your Turn";
                UIController.Instance.GameScoreText.text = $"{ClientWonCount} - {HostWonCount}";
            }
            UIController.Instance.GameRematchButton.gameObject.SetActive(false);
            CubeController.Instance.RestartTheGame();
        }

        public override void FixedUpdateNetwork()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(HostWonCount):
                        if (Object.HasStateAuthority)
                        {
                            UIController.Instance.GameScoreText.text = $"{HostWonCount} - {ClientWonCount}";
                        }
                        else
                        {
                            UIController.Instance.GameScoreText.text = $"{ClientWonCount} - {HostWonCount}";
                        }
                        break;
                    case nameof(ClientWonCount):
                        if (Object.HasStateAuthority)
                        {
                            UIController.Instance.GameScoreText.text = $"{HostWonCount} - {ClientWonCount}";
                        }
                        else
                        {
                            UIController.Instance.GameScoreText.text = $"{ClientWonCount} - {HostWonCount}";
                        }
                        break;
                }
            }
        }

        public void OnBothPlayerSpawned()
        {
            _playerDataManager._isOurTurnToPlay = true;
        }


        #region RPC

        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetPlayerDetails(string nickName, int avatarNum, RpcInfo info = default)
        {
            if (string.IsNullOrEmpty(nickName)) return;
            NickName = nickName;
            UIController.Instance.OpponentNameText.text = nickName;
            _playerDataManager.OpponentName = nickName;

            if (avatarNum == 0) return;
            AvatarNum = avatarNum;
            UIController.Instance.OpponentAvatarImg.sprite = UIController.Instance.AllAvatars[avatarNum - 1];
            _playerDataManager.OpponentAvatarNum = avatarNum;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_CubeSelectedByHost(int cubeNum, RpcInfo info = default)
        {
            _playerDataManager._isOurTurnToPlay = false;
            UIController.Instance.GameInfoText.text = "<size=30>Wait For Your Turn";
            if (!_runnerInstance.IsServer)
            {
                CubeController.Instance.HostCubeSimulate(cubeNum);
                UIController.Instance.GameInfoText.text = "Your Turn";
                _playerDataManager._isOurTurnToPlay = true;
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_CubeSelectedByClient(int cubeNum, RpcInfo info = default)
        {
            _playerDataManager._isOurTurnToPlay = false;
            UIController.Instance.GameInfoText.text = "<size=30>Wait For Your Turn";
            if (_runnerInstance.IsServer)
            {
                CubeController.Instance.ClientCubeSimulate(cubeNum);
                UIController.Instance.GameInfoText.text = "Your Turn";
                _playerDataManager._isOurTurnToPlay = true;
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_HostWon(RpcInfo info = default)
        {
            _playerDataManager._isOurTurnToPlay = false;
            if (_runnerInstance.IsServer)
            {
                HostWonCount++;
                lastGame = 1;
                UIController.Instance.GameInfoText.text = "You Won";
            }
            else
            {
                UIController.Instance.GameInfoText.text = "You Lost";
            }
            UIController.Instance.DisplayRematchButton();
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_ClientWon(RpcInfo info = default)
        {
            _playerDataManager._isOurTurnToPlay = false;
            if (_runnerInstance.IsServer)
            {
                ClientWonCount++;
                lastGame = 2;
                UIController.Instance.GameInfoText.text = "You Lost";
            }
            else
            {
                UIController.Instance.GameInfoText.text = "You Won";
            }
            UIController.Instance.DisplayRematchButton();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_DrawCallByHost(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "Draw";
            UIController.Instance.DisplayRematchButton();
            if (_runnerInstance.IsServer)
            {
                lastGame = 1;
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_DrawCallByClient(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "Draw";
            UIController.Instance.DisplayRematchButton();
            if (_runnerInstance.IsServer)
            {
                lastGame = 2;
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_RematchByHost(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "<size=30>Rematch Requested";
            if (!_runnerInstance.IsServer)
            {
                UIController.Instance.GameInfoText.text = $"<size=30>{_playerDataManager.OpponentName} Wants Rematch";
            }
            if (_runnerInstance.IsServer && _playerDataManager.didClientReqRematch == true)
            {
                if (lastGame == 1)
                {
                    RPC_StartRematch("Client");
                }
                else if (lastGame == 2)
                {
                    RPC_StartRematch("Host");
                }
            }
        }

        [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
        public void RPC_RematchByClient(RpcInfo info = default)
        {
            UIController.Instance.GameInfoText.text = "<size=30>Rematch Requested";
            if (_runnerInstance.IsServer)
            {
                UIController.Instance.GameInfoText.text = $"<size=30>{_playerDataManager.OpponentName} Wants Rematch";
                _playerDataManager.didClientReqRematch = true;
            }
            if (_runnerInstance.IsServer && _playerDataManager.didHostReqRematch == true)
            {
                if (lastGame == 1)
                {
                    RPC_StartRematch("Client");
                }
                else if (lastGame == 2)
                {
                    RPC_StartRematch("Host");
                }
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_StartRematch(string turn, RpcInfo info = default)
        {
            CubeController.Instance.RestartTheGame();
            UIController.Instance.GameRematchButton.gameObject.SetActive(false);

            _playerDataManager.didHostReqRematch = false;
            _playerDataManager.didClientReqRematch = false;

            if (turn == "Client")
            {
                if (!_runnerInstance.IsServer)
                {
                    UIController.Instance.GameInfoText.text = "Your Turn";
                    _playerDataManager._isOurTurnToPlay = true;
                }
                else
                {
                    UIController.Instance.GameInfoText.text = "Wait For Your Turn";
                }
            }
            else if (turn == "Host")
            {
                if (_runnerInstance.IsServer)
                {
                    UIController.Instance.GameInfoText.text = "Your Turn";
                    _playerDataManager._isOurTurnToPlay = true;
                }
                else
                {
                    UIController.Instance.GameInfoText.text = "Wait For Your Turn";
                }
            }
            lastGame = 0;
        }



        #endregion
    }
}

