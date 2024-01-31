using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.TicTacToe
{
    public class UIController : MonoBehaviour
    {
        [Header("Singleton Reference")]
        public static UIController Instance;
        PlayerDataManager _playerDataManager;

        [Header("Gameobjects")]
        GameObject _menuCanvas;
        GameObject _transitionPanel;

        [Header("UI Elements")]
        public TextMeshProUGUI PlayerNameText;
        public TextMeshProUGUI OpponentNameText;

        public TextMeshProUGUI GameScoreText;
        public TextMeshProUGUI GameInfoText;

        public Image PlayerAvatarImg;
        public Image OpponentAvatarImg;
        public Image PlayerSymbolImg;
        public Image OpponentSymbolImg;

        [SerializeField] Sprite _symbolXSprite;
        [SerializeField] Sprite _symbolOSprite;

        public Button GamePlayQuitButton;
        public Button GameRematchButton;
        private GameObject _errorMessagePanel;
        private TextMeshProUGUI _errorMessageText;
        public Button _quitButton;

        public List<Sprite> AllAvatars = new List<Sprite>();

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

            //_playerDataManager = FindObjectOfType<PlayerDataManager>();

            _menuCanvas = GameObject.Find("Menu_Canvas");
            _transitionPanel = GameObject.Find("Transition_Panel");

            PlayerNameText = GameObject.Find("PlayerName_Text").GetComponent<TextMeshProUGUI>();
            OpponentNameText = GameObject.Find("OpponentName_Text").GetComponent<TextMeshProUGUI>();

            GameScoreText = GameObject.Find("GameScore_Text").GetComponent<TextMeshProUGUI>();
            GameInfoText = GameObject.Find("GameInfo_Text").GetComponent<TextMeshProUGUI>();

            PlayerAvatarImg = GameObject.Find("Player_Avatar").GetComponent<Image>();
            OpponentAvatarImg = GameObject.Find("Opponent_Avatar").GetComponent<Image>();
            PlayerSymbolImg = GameObject.Find("PlayerSymbol_Img").GetComponent<Image>();
            OpponentSymbolImg = GameObject.Find("OpponentSymbol_Img").GetComponent<Image>();

            GamePlayQuitButton = GameObject.Find("GamePlayQuit_Button").GetComponent<Button>();
            GameRematchButton = GameObject.Find("GameRematch_Button").GetComponent<Button>();
            _errorMessagePanel = GameObject.Find("ErrorMessage_Panel");
            _errorMessageText = GameObject.Find("ErrorMessage_Text").GetComponent<TextMeshProUGUI>();
            _quitButton = GameObject.Find("Quit_Button").GetComponent<Button>();
        }

        void Start()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            GamePlayQuitButton.onClick.AddListener(() => { OnQuitButtonClick(); });
            GameRematchButton.onClick.AddListener(() => { OnRematchRequest(); });
            _quitButton.onClick.AddListener(() => { Application.Quit(); });
            StartCoroutine(StartFading());
            _errorMessagePanel.SetActive(false);
        }

        #region GameStart & Quit

        public void OnBothPlayersReady()
        {
            FindObjectOfType<RoomPublic>().PublicRoomPanel.SetActive(false);
            FindObjectOfType<RoomHost>().HostRoomPanel.SetActive(false);
            FindObjectOfType<RoomJoin>().OnJoinRoomBackBtnClick();
            _menuCanvas.SetActive(false);
            StartFadeEffect();
        }

        public void OnQuitButtonClick()
        {
            _menuCanvas.SetActive(true);
            StartFadeEffect();
            FusionManager.Instance.Runner.Shutdown();
        }

        private void OnRematchRequest()
        {
            if (FusionManager.Instance.Runner.IsServer)
            {
                _playerDataManager.RemotePlayerObj.GetComponent<PlayerDataNetworked>().RPC_RematchByHost();
            }
            else
            {
                _playerDataManager.LocalPlayerObj.GetComponent<PlayerDataNetworked>().RPC_RematchByClient();
            }

            _playerDataManager.didHostReqRematch = true;
            GameRematchButton.gameObject.SetActive(false);
        }

        #endregion

        #region Initial SetUp

        public void SetLocalPlayerSymbolX()
        {
            PlayerSymbolImg.sprite = _symbolXSprite;
            OpponentSymbolImg.sprite = _symbolOSprite;
        }

        public void SetLocalPlayerSymbolO()
        {

            PlayerSymbolImg.sprite = _symbolOSprite;
            OpponentSymbolImg.sprite = _symbolXSprite;
        }

        #endregion

        #region Transition Effect

        private void StartFadeEffect()
        {
            _transitionPanel.SetActive(true);
            StartCoroutine(StartFading());
        }

        private IEnumerator StartFading()
        {
            yield return new WaitForSeconds(1f);
            _transitionPanel.SetActive(false);

        }

        #endregion

        #region Error Message

        public void DisplayErrorMessage(string msg)
        {
            _errorMessagePanel.SetActive(true);
            _errorMessageText.text = msg;
        }

        #endregion
    }
}

