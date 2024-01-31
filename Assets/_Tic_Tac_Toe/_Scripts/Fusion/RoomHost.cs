using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.TicTacToe
{
    public class RoomHost : MonoBehaviour
    {
        [Header("Gameobjects")]
        public GameObject HostRoomPanel;

        [Header("UI Elements")]
        TextMeshProUGUI _roomHostIdText;
        Button _hostRoomBtn;
        Button _hostRoomBackBtn;

        private const string _letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // List of available letters
        private int _sessionNameLength = 6;


        private void Awake()
        {
            HostRoomPanel = GameObject.Find("RoomHost_Panel");
            _roomHostIdText = GameObject.Find("RoomHostID_Text").GetComponent<TextMeshProUGUI>();
            _hostRoomBtn = GameObject.Find("RoomHost_Btn").GetComponent<Button>();
            _hostRoomBackBtn = GameObject.Find("RoomHostBack_Btn").GetComponent<Button>();
        }

        void Start()
        {
            _hostRoomBtn.onClick.AddListener(() => { OnHostRoomBtnClick(); });
            _hostRoomBackBtn.onClick.AddListener(() => { OnHostRoomBackBtnClick(); });
            HostRoomPanel.SetActive(false);
        }

        private void OnHostRoomBtnClick()
        {
            HostRoomPanel.SetActive(true);
            GenerateUniqueSessionName(out string roomID);
            _roomHostIdText.text = "Room ID- " + roomID;
            Debug.Log(roomID);

            FusionManager.Instance.GameRoomHost(roomID);
        }

        private void OnHostRoomBackBtnClick()
        {
            HostRoomPanel.SetActive(false);
            FusionManager.Instance.Runner.Shutdown();
        }

        private void GenerateUniqueSessionName(out string result)
        {
            string sessionName = "";

            for (int i = 0; i < _sessionNameLength; i++)
            {
                int randomIndex = Random.Range(0, _letters.Length);
                sessionName += _letters[randomIndex];
            }

            result = sessionName;
        }
    }
}

