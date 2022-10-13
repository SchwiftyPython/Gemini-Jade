using Repos;
using UnityEngine;
using Utilities;

namespace UI
{
    public class ProgressBar
    {
        private Outline _outline;

        private GameObject _gameObject;

        private Transform _transform;

        private Transform _background;

        private Transform _bar;
    
        public class Outline
        {
            public float size = 1f;
        
            public Color color = Color.black;
        }

        public ProgressBar(Transform parent, Vector3 localPosition, Vector3 localScale, Color? backgroundColor,
            Color barColor, float sizeRatio, int sortingOrder, Outline outline = null)
        {
            _outline = outline;
            
            SetupParent(parent, localPosition);

            if (outline != null)
            {
                SetupOutline(outline, localScale, sortingOrder - 1);
            }

            if (backgroundColor != null)
            {
                SetupBackground((Color) backgroundColor, localScale, sortingOrder);
            }
            
            SetupBar(barColor, localScale, sortingOrder + 1);
            
            SetSize(sizeRatio);
        }
        
        private void SetupParent(Transform parent, Vector3 localPosition) 
        {
            _gameObject = new GameObject($"Progress Bar: {parent.gameObject.name} {localPosition}");
            
            _transform = _gameObject.transform;
            
            _transform.SetParent(parent);
            
            _transform.localPosition = localPosition;
        }

        private void SetupOutline(Outline outline, Vector3 localScale, int sortingOrder)
        {
            var graphicsRepo = Object.FindObjectOfType<GraphicsRepo>();

            UnityUtils.CreateWorldSprite(_transform, "Outline", graphicsRepo.whiteSquare, Vector3.zero,
                localScale + new Vector3(outline.size, outline.size), sortingOrder, outline.color);
        }

        private void SetupBackground(Color backgroundColor, Vector3 localScale, int sortingOrder)
        {
            var graphicsRepo = Object.FindObjectOfType<GraphicsRepo>();

            _background = UnityUtils.CreateWorldSprite(_transform, "Background", graphicsRepo.whiteSquare,
                Vector3.zero, localScale, sortingOrder, backgroundColor).transform;
        }

        private void SetupBar(Color barColor, Vector3 localScale, int sortingOrder)
        {
            var barObject = new GameObject("Bar");

            _bar = barObject.transform;
            
            _bar.SetParent(_transform);

            _bar.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
            
            _bar.localScale = Vector3.one;
            
            var graphicsRepo = Object.FindObjectOfType<GraphicsRepo>();

            UnityUtils.CreateWorldSprite(_bar, "InnerBar", graphicsRepo.whiteSquare,
                new Vector3(localScale.x / 2f, 0),
                localScale, sortingOrder, barColor);
        }

        public void SetSize(float sizeRatio)
        {
            _bar.localScale = new Vector3(sizeRatio, 1, 1);
        }

        public void SetLocalScale(Vector3 localScale)
        {
            if (_transform.Find("Outline") != null)
            {
                _transform.Find("Outline").localScale = localScale + new Vector3(_outline.size, _outline.size);
            }

            _background.localScale = localScale;
            
            _bar.localPosition = new Vector3(-localScale.x / 2f, 0, 0);
            
            var innerBar = _bar.Find("InnerBar");
            
            innerBar.localScale = localScale;
            
            innerBar.localPosition = new Vector3(localScale.x / 2f, 0);
        }
        
        public void SetColor(Color color) 
        {
            _bar.Find("BarIn").GetComponent<SpriteRenderer>().color = color;
        }
        
        public void Show()
        {
            _gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            _gameObject.SetActive(false);
        }

        public void DestroySelf() 
        {
            if (_gameObject != null) 
            {
                Object.Destroy(_gameObject);
            }
        }
    }
}
