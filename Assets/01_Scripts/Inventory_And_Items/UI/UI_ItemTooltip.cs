using AniDrag.Core;
using TMPro;
using UnityEngine;

namespace AniDrag.Inventory
{
    public class UI_ItemTooltip: MonoBehaviour
    {
        [SerializeField] private GameObject _tooltipPanel;
        [SerializeField] private TMP_Text _tooltipText;
        [SerializeField] private RectTransform _tooltipRect;
        [SerializeField] private Vector2 _offset = new Vector2(10, -10);

        private static UI_ItemTooltip _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            _tooltipPanel.SetActive(false);
        }

        public static void Show(Item item, Vector2 screenPosition)
        {
            if (_instance == null) return;
            _instance._tooltipText.text = item.GetTooltipText();
            _instance._tooltipPanel.SetActive(true);

            // Position near mouse, clamped to screen edges
            Vector2 newPos = screenPosition + _instance._offset;
            newPos.x = Mathf.Clamp(newPos.x, 0, Screen.width - _instance._tooltipRect.rect.width);
            newPos.y = Mathf.Clamp(newPos.y, _instance._tooltipRect.rect.height, Screen.height);
            _instance._tooltipRect.position = newPos;
        }

        public static void Hide()
        {
            if (_instance == null) return;
            _instance._tooltipPanel.SetActive(false);
        }
    }
}