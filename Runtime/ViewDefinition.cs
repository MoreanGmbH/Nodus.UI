using UnityEngine;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [CreateAssetMenu(menuName = "Nodus/UI/View Definition", fileName = "View")]
    public sealed class ViewDefinition : ScriptableObject {
        [SerializeField] string id;
        [SerializeField] VisualTreeAsset template;
        public string Id => id;
        public VisualTreeAsset Template => template;
    }
}
