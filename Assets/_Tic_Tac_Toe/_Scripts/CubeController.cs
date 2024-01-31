using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace DonzaiGamecorp.TicTacToe
{
    public class CubeController : MonoBehaviour
    {
        public static CubeController Instance;
        PlayerDataManager _playerDataManager;

        [Header("Transforms")]
        Transform _TicTacToeTransform;
        Transform _strikeTransform;

        float rotationTime = 1f;
        Vector3 _rotateTo_O = new Vector3(-90f, 0f, -90f);
        Vector3 _rotateTo_X = new Vector3(-90f, 0f, 90f);

        List<int> _cubeRotatedToX = new List<int>();
        List<int> _cubeRotatedToO = new List<int>();
        List<int> _availableCube = new List<int>();

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

            _TicTacToeTransform = GameObject.Find("TIc_Tac_Toe").GetComponent<Transform>();
        }

        void Start()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            AssignCubeNum();
            _strikeTransform = _TicTacToeTransform.GetChild(0);
            _strikeTransform.gameObject.SetActive(false);
            RestartTheGame();
        }

        private void AssignCubeNum()
        {
            for (int i = 1; i < _TicTacToeTransform.childCount; i++)
            {
                int a = i;
                _TicTacToeTransform.GetChild(i).GetComponent<OnCubeClicked>().CubeNum = a;
            }
        }

        public void OnCubeClicked(int cubeNum)
        {
            if (_playerDataManager._isOurTurnToPlay)
            {
                if (!_availableCube.Contains(cubeNum))
                    return;

                if (FusionManager.Instance.Runner.IsServer)
                {
                    _playerDataManager.RemotePlayerObj.GetComponent<PlayerDataNetworked>().RPC_CubeSelectedByHost(cubeNum);
                    StartCoroutine(RotateCube(_TicTacToeTransform.GetChild(cubeNum), _rotateTo_X));
                    _cubeRotatedToX.Add(cubeNum);
                    _availableCube.Remove(cubeNum);
                    CheckForStrike(_cubeRotatedToX, out bool didPlayerWin);
                    if (didPlayerWin) _playerDataManager.RemotePlayerObj.GetComponent<PlayerDataNetworked>().RPC_HostWon();
                }
                else
                {
                    _playerDataManager.LocalPlayerObj.GetComponent<PlayerDataNetworked>().RPC_CubeSelectedByClient(cubeNum);
                    StartCoroutine(RotateCube(_TicTacToeTransform.GetChild(cubeNum), _rotateTo_O));
                    _cubeRotatedToO.Add(cubeNum);
                    _availableCube.Remove(cubeNum);
                    CheckForStrike(_cubeRotatedToO, out bool didPlayerWin);
                    if (didPlayerWin) _playerDataManager.LocalPlayerObj.GetComponent<PlayerDataNetworked>().RPC_ClientWon();
                }
            }
        }

        public void HostCubeSimulate(int cubeNum)
        {
            StartCoroutine(RotateCube(_TicTacToeTransform.GetChild(cubeNum), _rotateTo_X));
            _cubeRotatedToX.Add(cubeNum);
            _availableCube.Remove(cubeNum);
            CheckForStrike(_cubeRotatedToX, out bool didPlayerWin);
        }

        public void ClientCubeSimulate(int cubeNum)
        {
            StartCoroutine(RotateCube(_TicTacToeTransform.GetChild(cubeNum), _rotateTo_O));
            _cubeRotatedToO.Add(cubeNum);
            _availableCube.Remove(cubeNum);
            CheckForStrike(_cubeRotatedToO, out bool didPlayerWin);
        }

        private IEnumerator RotateCube(Transform cube, Vector3 rotateTo)
        {
            // Store the initial rotation
            Quaternion startRotation = cube.rotation;

            // Time elapsed
            float elapsed = 0f;

            while (elapsed < rotationTime)
            {
                // Increment elapsed time based on real time
                elapsed += Time.deltaTime;

                // Calculate the interpolation factor
                float t = Mathf.Clamp01(elapsed / rotationTime);

                // Interpolate the rotation
                cube.rotation = Quaternion.Slerp(startRotation, Quaternion.Euler(rotateTo), t);

                // Wait for the next frame
                yield return null;
            }
        }

        private void CheckForStrike(List<int> list, out bool didWin)
        {
            didWin = false;

            int[,] winPatterns = new int[,]
            {
                {1, 2, 3}, {4, 5, 6}, {7, 8, 9}, // Rows
                {1, 4, 7}, {2, 5, 8}, {3, 6, 9}, // Columns
                {1, 5, 9}, {3, 5, 7}              // Diagonals
            };

            for (int i = 0; i < winPatterns.GetLength(0); i++)
            {
                int pos1 = winPatterns[i, 0];
                int pos2 = winPatterns[i, 1];
                int pos3 = winPatterns[i, 2];

                if (list.Contains(pos1) && list.Contains(pos2) && list.Contains(pos3))
                {
                    StartCoroutine(SetStrikeTransform(i));
                    didWin = true;
                    return;
                }
            }

            if (_availableCube == null || _availableCube.Count == 0)
            {
                if (FusionManager.Instance.Runner.IsServer)
                {
                    _playerDataManager.RemotePlayerObj.GetComponent<PlayerDataNetworked>().RPC_DrawCallByHost();
                }
                else
                {
                    _playerDataManager.LocalPlayerObj.GetComponent<PlayerDataNetworked>().RPC_DrawCallByClient();
                }
            }
        }

        private IEnumerator SetStrikeTransform(int patternIndex)
        {
            float xPos = 0f, yPos = 1f, zPos = -1.1f;
            float zRotation = 0f;

            switch (patternIndex)
            {
                case 0: yPos = 3.1f; break;   // Row 1
                case 1: yPos = 1f; break;     // Row 2
                case 2: yPos = -1.1f; break;   // Row 3
                case 3: xPos = -2.1f; zRotation = 90f; break;  // Column 1
                case 4: zRotation = 90f; break;              // Column 2
                case 5: xPos = 2.1f; zRotation = 90f; break;  // Column 3
                case 6: zRotation = -45f; break;             // Diagonal 1
                case 7: zRotation = 45f; break;              // Diagonal 2
            }

            _strikeTransform.position = new Vector3(xPos, yPos, zPos);
            _strikeTransform.rotation = Quaternion.Euler(0f, 0f, zRotation);

            yield return new WaitForSeconds(1f);
            _strikeTransform.gameObject.SetActive(true);
        }

        public void RestartTheGame()
        {
            foreach (Transform t in _TicTacToeTransform)
            {
                t.rotation = Quaternion.Euler(-90f, 0f, 0f);
            }
            _strikeTransform.rotation = Quaternion.identity;
            _strikeTransform.gameObject.SetActive(false);

            _cubeRotatedToX.Clear();
            _cubeRotatedToO.Clear();
            _availableCube.Clear();
            _availableCube.Add(1);
            _availableCube.Add(2);
            _availableCube.Add(3);
            _availableCube.Add(4);
            _availableCube.Add(5);
            _availableCube.Add(6);
            _availableCube.Add(7);
            _availableCube.Add(8);
            _availableCube.Add(9);
        }

    }
}

