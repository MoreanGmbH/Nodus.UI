using UnityEngine;
using UnityEngine.UIElements;

namespace Nodus.UI {
    [CreateAssetMenu(menuName = "Nodus/UI/View Definition", fileName = "View")]
    public sealed class ViewDefinition : ScriptableObject {
        [SerializeField] public VisualTreeAsset Template;
    }
}
