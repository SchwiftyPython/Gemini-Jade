using TMPro;
using UnityEngine;
using World.Things;

namespace UI
{
   [RequireComponent(typeof(TextMeshPro))]
   public class LabelComponent : MonoBehaviour
   {
      private StackThing _stackThing;

      private RectTransform _rectTransform;

      private TextMeshPro _textMeshPro;

      private void Awake()
      {
         transform.position = new Vector3(0, 0, (int)MapLayer.Label * -1);

         _textMeshPro = gameObject.GetComponent<TextMeshPro>();

         _rectTransform = (RectTransform) transform;
         
         _rectTransform.offsetMin = Vector2.zero;

         _rectTransform.offsetMax = Vector2.one;

         _textMeshPro.alignment = TextAlignmentOptions.MidlineGeoAligned;

         _textMeshPro.fontStyle = FontStyles.Bold;

         _textMeshPro.fontSize = 3;
      }

      private void Update()
      {
         if (_stackThing == null)
         {
            return;
         }

         if (_stackThing.Count < 1)
         {
            _textMeshPro.text = string.Empty;
            
            return;
         }

         _textMeshPro.text = _stackThing.Count.ToString();
      }

      public void SetStackThing(StackThing thing)
      {
         _stackThing = thing;

         _rectTransform.offsetMin = new Vector2(_stackThing.Position.X - 0.5f, _stackThing.Position.Y - 1.7f);

         _rectTransform.offsetMax = new Vector2(_stackThing.Position.X + 0.5f, _stackThing.Position.Y + +1.2f);

         _textMeshPro.text = _stackThing.Count.ToString();
      }
   }
}
