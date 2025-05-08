#region Includes
using UnityEngine;
using UnityEngine.UI;
#endregion

namespace TS.PageSlider.Demo
{
    public class SliderDot : MonoBehaviour
    {
        #region Variables

        [Header("Configuration")]
        [SerializeField] private Sprite _imageDefault;
        [SerializeField] private Sprite _imageSelected;

        private Image _image;
        private PageDot _dot;

        #endregion

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.sprite = _imageDefault;
            _dot = GetComponent<PageDot>();
            _dot.OnActiveStateChanged.AddListener(PageDot_ActiveStateChanged);
        }

        private void PageDot_ActiveStateChanged(bool active)
        {
            _image.sprite = active ? _imageSelected : _imageDefault;
        }
    }
}