using TMPro;
using UnityEngine;
using World.Things;

namespace UI
{
   public class LabelComponent : MonoBehaviour
   {
      private const float SphereRadius = 2f;
      
      private StackThing _stackThing;

      private RectTransform _rectTransform;

      private TextMeshPro _textMeshPro;

      private GameObject _label;

      private void Awake()
      {
         transform.position = new Vector3(0, 0, (int)MapLayer.Label * -1);
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
         if (_label == null)
         {
            SetLabelObject(gameObject.transform.GetChild(0).gameObject);
         }
         
         _stackThing = thing;

         if (_rectTransform == null)
         {
            _rectTransform = (RectTransform) transform;
         }

         _rectTransform.offsetMin = new Vector2(_stackThing.Position.X - 0.5f, _stackThing.Position.Y - 1.7f);

         _rectTransform.offsetMax = new Vector2(_stackThing.Position.X + 0.5f, _stackThing.Position.Y + +1.2f);

         if (_textMeshPro == null)
         {
            _textMeshPro = _label.GetComponent<TextMeshPro>();
         }

         _textMeshPro.text = _stackThing.Count.ToString();
         
         Hide();
      }

      public void SetLabelObject(GameObject labelObject)
      {
         _label = labelObject;
         
         _textMeshPro = _label.AddComponent<TextMeshPro>();

         _rectTransform = gameObject.AddComponent<RectTransform>();
         
         _rectTransform.offsetMin = Vector2.zero;

         _rectTransform.offsetMax = Vector2.one;

         _textMeshPro.alignment = TextAlignmentOptions.MidlineGeoAligned;

         _textMeshPro.fontStyle = FontStyles.Bold;

         _textMeshPro.fontSize = 3;
      }

      public void Show(bool showNeighbors = true)
      {
         _label.SetActive(true);

         if (!showNeighbors)
         {
            return;
         }

         Collider[] neighbors = new Collider[9];

         var numColliders = Physics.OverlapSphereNonAlloc(transform.position, SphereRadius, neighbors);

         if (numColliders > 1)
         {
            foreach (var neighbor in neighbors)
            {
               if (neighbor == null)
               {
                  continue;
               }
               
               var neighborLabel = neighbor.gameObject.GetComponent<LabelComponent>();

               if (neighborLabel == null)
               {
                  continue;
               }

               neighborLabel.Show(false);
            }
         }
      }

      public void Hide(bool hideNeighbors = true)
      {
         _label.SetActive(false);

         if (!hideNeighbors)
         {
            return;
         }

         HideNeighbors();
      }

      private void HideNeighbors()
      {
         Collider[] neighbors = new Collider[9];

         var numColliders = Physics.OverlapSphereNonAlloc(transform.position, SphereRadius, neighbors);

         if (numColliders > 1)
         {
            foreach (var neighbor in neighbors)
            {
               if (neighbor == null)
               {
                  continue;
               }
               
               var neighborLabel = neighbor.gameObject.GetComponent<LabelComponent>();

               if (neighborLabel == null)
               {
                  continue;
               }

               neighborLabel.Hide(false);
            }
         }
      }

      private void OnMouseEnter()
      {
         Debug.Log($"Mouse entered label");
         
         Show();
      }

      private void OnMouseExit()
      {
         Debug.Log($"Mouse exited label");
         
         Hide();
      }
   }
}
