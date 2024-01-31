using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.TicTacToe
{
    public class RoomPublic : MonoBehaviour
    {
        [Header("Gameobjects")]
        public GameObject PublicRoomPanel;

        [Header("Button")]
        Button _publicRoomBtn;
        Button _publicRoomBackBtn;

        private void Awake()
        {
            PublicRoomPanel = GameObject.Find("RoomPublic_Panel");
            _publicRoomBtn = GameObject.Find("RoomPublic_Btn").GetComponent<Button>();
            _publicRoomBackBtn = GameObject.Find("RoomPublicBack_Btn").GetComponent<Button>();
        }

        private void Start()
        {
            _publicRoomBtn.onClick.AddListener(() => { OnPublicRoomBtnClick(); });
            _publicRoomBackBtn.onClick.AddListener(() => { OnPublicRoomBackBtnClick(); });
            PublicRoomPanel.SetActive(false);
        }

        private void OnPublicRoomBtnClick()
        {
            PublicRoomPanel.SetActive(true);
            FusionManager.Instance.GameRoomAutoMatch();
        }

        public void OnPublicRoomBackBtnClick()
        {
            PublicRoomPanel.SetActive(false);
            FusionManager.Instance.Runner.Shutdown();
        }
    }
}

