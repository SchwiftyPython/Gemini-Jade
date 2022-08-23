using UnityEngine;
using UnityEngine.UI;

namespace Time
{
    /// <summary>
    /// The time changer class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class TimeChanger : MonoBehaviour
    {
        //todo this belongs in UI namespace
        
        /// <summary>
        /// The tick controller
        /// </summary>
        private TickController _tickController;

        /// <summary>
        /// The pre pause button
        /// </summary>
        private Button _prePauseButton;

        /// <summary>
        /// The pause button
        /// </summary>
        [SerializeField] private Button pauseButton;
        /// <summary>
        /// The normal button
        /// </summary>
        [SerializeField] private Button normalButton;
        /// <summary>
        /// The fast button
        /// </summary>
        [SerializeField] private Button fastButton;
        /// <summary>
        /// The ultra button
        /// </summary>
        [SerializeField] private Button ultraButton;
        
        /// <summary>
        /// Starts this instance
        /// </summary>
        private void Start()
        {
            _tickController = FindObjectOfType<TickController>();
            
            normalButton.Select();
        }

        /// <summary>
        /// Updates this instance
        /// </summary>
        private void Update()
        {
            //todo catch keyboard inputs
        }

        /// <summary>
        /// Toggles the pause
        /// </summary>
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

        /// <summary>
        /// Returns the normal speed
        /// </summary>
        public void ToNormalSpeed()
        {
            _tickController.currentSpeed = _tickController.normalSpeed;
        }

        /// <summary>
        /// Returns the fast speed
        /// </summary>
        public void ToFastSpeed()
        {
            _tickController.currentSpeed = _tickController.fast;
        }

        /// <summary>
        /// Returns the ultra speed
        /// </summary>
        public void ToUltraSpeed()
        {
            _tickController.currentSpeed = _tickController.ultra;
        }
    }
}
