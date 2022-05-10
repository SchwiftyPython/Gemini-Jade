using UnityEngine;
using UnityEngine.UI;

namespace Time
{
    public class TimeChanger : MonoBehaviour
    {
        private TickController _tickController;

        private Button _prePauseButton;

        [SerializeField] private Button pauseButton;
        [SerializeField] private Button normalButton;
        [SerializeField] private Button fastButton;
        [SerializeField] private Button ultraButton;
        
        private void Start()
        {
            _tickController = FindObjectOfType<TickController>();
            
            normalButton.Select();
        }

        private void Update()
        {
            //todo catch keyboard inputs
        }

        public void TogglePause()
        {
            //todo if pause is selected, need to deselect pause and select previous speed button

            if (_tickController.currentSpeed == _tickController.paused)
            {
                _prePauseButton.Select();
            }
            else
            {
                if (_tickController.currentSpeed == _tickController.normalSpeed)
                {
                    _prePauseButton = normalButton;
                }
                else if (_tickController.currentSpeed == _tickController.fast)
                {
                    _prePauseButton = fastButton;
                }
                else if (_tickController.currentSpeed == _tickController.ultra)
                {
                    _prePauseButton = ultraButton;
                }
            }
            
            _tickController.TogglePause();
        }

        public void ToNormalSpeed()
        {
            _tickController.currentSpeed = _tickController.normalSpeed;
        }

        public void ToFastSpeed()
        {
            _tickController.currentSpeed = _tickController.fast;
        }

        public void ToUltraSpeed()
        {
            _tickController.currentSpeed = _tickController.ultra;
        }
    }
}
