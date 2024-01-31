using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DonzaiGamecorp.TicTacToe
{
    public class PlayerInfoEdit : MonoBehaviour
    {
        [Header("Script Reference")]
        PlayerDataManager _playerDataManager;

        [Header("Gameobjects")]
        GameObject _playerInfoEditPanel;
        Transform _playerAvatarList;

        [Header("UI Elements")]
        TextMeshProUGUI _playerNameDisplayText;
        Image _playerDisplayAvatar;

        TMP_InputField _playerNameInputfield;
        Button _playerIndoEditBtn;
        Button _playerIndoEditSubmitBtn;
        Button _playerIndoEditBackBtn;

        int _selectedAvatarNum;

        private void Awake()
        {
            _playerDataManager = FindObjectOfType<PlayerDataManager>();
            _playerInfoEditPanel = GameObject.Find("PlayerInfoEdit_Panel");
            _playerAvatarList = GameObject.Find("Player_Avatars_List").GetComponent<Transform>();

            _playerNameDisplayText = GameObject.Find("PlayerName_DisplayText").GetComponent<TextMeshProUGUI>();
            _playerDisplayAvatar = GameObject.Find("Player_Display_Avatar").GetComponent<Image>();

            _playerNameInputfield = GameObject.Find("PlayerName_InputField").GetComponent<TMP_InputField>();

            _playerIndoEditBtn = GameObject.Find("PlayerInfoEdit_Btn").GetComponent<Button>();
            _playerIndoEditSubmitBtn = GameObject.Find("PyInfoSubmit_Btn").GetComponent<Button>();
            _playerIndoEditBackBtn = GameObject.Find("PyInfoBack_Btn").GetComponent<Button>();
        }

        void Start()
        {
            //PlayerPrefs.DeleteAll();
            if (PlayerPrefs.HasKey("PlayerNickname"))
            {
                _playerDataManager.NickName = PlayerPrefs.GetString("PlayerNickname");
            }
            else if (string.IsNullOrWhiteSpace(_playerDataManager.NickName))
            {
                var rngPlayerNumber = Random.Range(0, 999);
                _playerDataManager.NickName = $"Player_{rngPlayerNumber.ToString("000")}";
            }

            if (PlayerPrefs.HasKey("PlayerAvatarNum"))
            {
                _playerDataManager.PlayerAvatarNum = PlayerPrefs.GetInt("PlayerAvatarNum");
            }
            else
            {
                int num = 1;
                _playerDataManager.PlayerAvatarNum = num;
            }


            _playerNameDisplayText.text = _playerDataManager.NickName;
            _playerDisplayAvatar.sprite = _playerAvatarList.GetChild(_playerDataManager.PlayerAvatarNum).GetComponent<Image>().sprite;
            //OnAvatarClicked(_playerDataManager.PlayerAvatarNum);

            _playerIndoEditBtn.onClick.AddListener(() => { OnPlayerEditBtnClick(); });
            _playerIndoEditSubmitBtn.onClick.AddListener(() => { OnSubmitBtnClicked(); });
            _playerIndoEditBackBtn.onClick.AddListener(() => { OnBackBtnClicked(); });

            for (int i = 1; i < _playerAvatarList.childCount; i++)
            {
                int a = i;
                Button btn = _playerAvatarList.GetChild(i).GetComponent<Button>();
                btn.onClick.AddListener(() => { OnAvatarClicked(a); });
            }

            _playerInfoEditPanel.SetActive(false);
        }

        private void OnPlayerEditBtnClick()
        {
            _playerInfoEditPanel.SetActive(true);
            OnAvatarClicked(_playerDataManager.PlayerAvatarNum);
        }

        private void OnAvatarClicked(int avatarNum)
        {
            StartCoroutine(AvatarClick(avatarNum));
        }

        private IEnumerator AvatarClick(int avatarNum)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            Vector3 position = _playerAvatarList.GetChild(avatarNum).position;
            _playerAvatarList.GetChild(0).position = position;
            _selectedAvatarNum = avatarNum;
        }

        private void OnSubmitBtnClicked()
        {
            if (!string.IsNullOrWhiteSpace(_playerNameInputfield.text))
            {
                PlayerPrefs.SetString("PlayerNickname", _playerNameInputfield.text);
                _playerNameDisplayText.text = _playerNameInputfield.text;
                _playerDataManager.NickName = _playerNameInputfield.text;
            }

            _playerDisplayAvatar.sprite = _playerAvatarList.GetChild(_selectedAvatarNum).GetComponent<Image>().sprite;
            _playerDataManager.PlayerAvatarNum = _selectedAvatarNum;

            PlayerPrefs.SetInt("PlayerAvatarNum", _selectedAvatarNum);
            PlayerPrefs.Save(); // Save the PlayerPrefs to persist the data
            _playerNameInputfield.text = "";
            _playerInfoEditPanel.SetActive(false);
        }

        private void OnBackBtnClicked()
        {
            _playerNameInputfield.text = "";
            _playerInfoEditPanel.SetActive(false);
        }
    }
}

