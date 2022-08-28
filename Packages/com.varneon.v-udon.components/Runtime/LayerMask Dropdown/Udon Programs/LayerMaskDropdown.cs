using UdonSharp;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.Udon;

namespace Varneon.VUdon.Components
{
    [RequireComponent(typeof(Dropdown))]
    [RequireComponent(typeof(EventTrigger))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LayerMaskDropdown : UdonSharpBehaviour
    {
        [SerializeField, HideInInspector]
        private int projectLayerCount;

        [SerializeField, HideInInspector]
        private int[] layers;

        [SerializeField]
        private UdonBehaviour target;

        [SerializeField]
        private string variable;

        private LayerMask mask;

        private Dropdown cullingMaskDropdown;

        private int maskInt;

        private Toggle[] cullingMaskToggles;

        private Text dropdownValueLabel;

        private bool isOpen;

        private void Start()
        {
            cullingMaskDropdown = GetComponent<Dropdown>();

            dropdownValueLabel = cullingMaskDropdown.captionText;

            maskInt = mask;

            SetTargetVariable();

            CheckMixedLayers();
        }

        private void SetTargetVariable()
        {
            if(target != null && !string.IsNullOrWhiteSpace(variable))

            target.SetProgramVariable(variable, mask);
        }

        private void OpenCullingMaskMenu()
        {
            isOpen = true;

            cullingMaskToggles = cullingMaskDropdown.GetComponentsInChildren<Toggle>(true);

            for (int i = 0; i < projectLayerCount; i++)
            {
                cullingMaskToggles[i + 4].SetIsOnWithoutNotify((maskInt & (1 << layers[i])) != 0);
            }
        }

        public void OnClick()
        {
            OpenCullingMaskMenu();
        }

        public void OnSelect()
        {
            if (isOpen) { UpdateCullingMask(); }
        }

        public void OnValueChanged()
        {
            UpdateCullingMask();
        }

        private void UpdateCullingMask()
        {
            int selectedLayerOption = cullingMaskDropdown.value;

            switch (selectedLayerOption)
            {
                case 0: // Nothing
                    maskInt = 0;
                    break;
                case 1: // Everything
                    maskInt = -1;
                    break;
                default:
                    int mask = 1 << layers[selectedLayerOption - 2]; // Create bitmask of the layer that was clicked

                    if (isOpen)
                    {
                        if ((maskInt & mask) != 0) // If same bit is set on both the culling mask and the selection mask
                        {
                            maskInt &= ~mask; // Reset the bit
                        }
                        else
                        {
                            maskInt |= mask; // Set the bit
                        }
                    }

                    CheckMixedLayers();
                    break;
            }

            mask = maskInt;

            SetTargetVariable();

            isOpen = false;
        }

        private void CheckMixedLayers()
        {
            int layerCount = 0;

            int optionIndex = 0;

            for (int i = 0; i < projectLayerCount; i++)
            {
                if ((maskInt & (1 << layers[i])) != 0)
                {
                    optionIndex = i;

                    layerCount += 1;
                }
            }

            if (layerCount == projectLayerCount)
            {
                maskInt = -1;

                cullingMaskDropdown.SetValueWithoutNotify(1);
            }
            else if (layerCount > 1) // If multiple layers are selected, set the preview label as "Mixed..."
            {
                dropdownValueLabel.text = "Mixed...";
            }
            else if (layerCount == 1) // If only one layer is selected, override the dropdown with the remaining layer
            {
                cullingMaskDropdown.SetValueWithoutNotify(optionIndex + 2);
            }
            else if (layerCount == 0) // If no layers have been selected, override the dropdown to Nothing / 0
            {
                cullingMaskDropdown.SetValueWithoutNotify(0);
            }
        }
    }
}
