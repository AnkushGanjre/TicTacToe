using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace DonzaiGamecorp.TicTacToe
{
    public class InternetCheck : MonoBehaviour
    {
        [Header("Internet Checker")]
        GameObject _noInternetPanel;
        Button _tryAgainBtn;

        [Header("Loading Text")]
        TextMeshProUGUI _loadingText;
        float dotsSpeed = 0.5f;


        private void Awake()
        {
            _noInternetPanel = GameObject.Find("NoInternet_Panel");
            _tryAgainBtn = GameObject.Find("TryAgain_Button").GetComponent<Button>();
            _loadingText = GameObject.Find("Loading_Text").GetComponent<TextMeshProUGUI>();
        }

        private void Start()
        {
            _tryAgainBtn.onClick.AddListener(() => { OnInternetCheck(); });
            OnInternetCheck();
        }

        private async void OnInternetCheck()
        {
            _noInternetPanel.SetActive(false);
            StartCoroutine(AnimateLoadingText());
            await OnConnectionCheck();
        }

        private async Task OnConnectionCheck()
        {
            await Task.Delay(3000); // Using Task.Delay instead of WaitForSeconds

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                _noInternetPanel.SetActive(true);
                StopAllCoroutines();
            }
            else
            {
                await LoadScene(1);
            }
        }

        private async Task LoadScene(int sceneIndex)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

            while (!asyncLoad.isDone)
            {
                await Task.Yield();
            }
        }

        IEnumerator AnimateLoadingText()
        {
            while (true)
            {
                _loadingText.text = "Loading";
                yield return new WaitForSeconds(dotsSpeed);

                _loadingText.text = "Loading.";
                yield return new WaitForSeconds(dotsSpeed);

                _loadingText.text = "Loading..";
                yield return new WaitForSeconds(dotsSpeed);

                _loadingText.text = "Loading...";
                yield return new WaitForSeconds(dotsSpeed);
            }
        }
    }
}

