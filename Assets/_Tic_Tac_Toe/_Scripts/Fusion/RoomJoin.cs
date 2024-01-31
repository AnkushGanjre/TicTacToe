using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.TicTacToe
{
    public class RoomJoin : MonoBehaviour
    {
        [Header("Gameobjects")]
        GameObject _joinRoomPanel;

        [Header("UI Elements")]
        TMP_InputField _joinRoomIdInputField;
        Button _joinRoomBtn;
        Button _joinRoomSubmitBtn;
        Button _joinRoomBackBtn;

        private void Awake()
        {
            _joinRoomPanel = GameObject.Find("RoomJoin_Panel");
            _joinRoomIdInputField = GameObject.Find("RoomJoin_InputField").GetComponent<TMP_InputField>();
            _joinRoomBtn = GameObject.Find("RoomJoin_Btn").GetComponent<Button>();
            _joinRoomSubmitBtn = GameObject.Find("RoomJoinSubmit_Btn").GetComponent<Button>();
            _joinRoomBackBtn = GameObject.Find("RoomJoinBack_Btn").GetComponent<Button>();
        }

        void Start()
        {
            _joinRoomBtn.onClick.AddListener(() => { _joinRoomPanel.SetActive(true); });
            _joinRoomSubmitBtn.onClick.AddListener(() => { OnJoinRoomSubmitBtnClick(); });
            _joinRoomBackBtn.onClick.AddListener(() => { OnJoinRoomBackBtnClick(); });
            _joinRoomPanel.SetActive(false);
        }

        private void OnJoinRoomSubmitBtnClick()
        {
            string roomID = _joinRoomIdInputField.text.ToUpper();
            Debug.Log(roomID);

            FusionManager.Instance.GameRoomClient(roomID);
        }

        public void OnJoinRoomBackBtnClick()
        {
            _joinRoomIdInputField.text = "";
            _joinRoomPanel.SetActive(false);
        }
    }
}

