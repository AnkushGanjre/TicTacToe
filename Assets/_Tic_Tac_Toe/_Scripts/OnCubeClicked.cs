using UnityEngine;

namespace DonzaiGamecorp.TicTacToe
{
    public class OnCubeClicked : MonoBehaviour
    {
        public int CubeNum;
        CubeController _cubeController;


        private void Start()
        {
            _cubeController = FindObjectOfType<CubeController>();
        }

        private void OnMouseDown()
        {
            _cubeController.OnCubeClicked(CubeNum);
        }
    }
}

