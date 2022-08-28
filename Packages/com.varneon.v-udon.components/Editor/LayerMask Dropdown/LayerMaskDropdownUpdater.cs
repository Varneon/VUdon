using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UdonSharpEditor;
using UnityEditor.Callbacks;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VRC.Udon;
using static UnityEngine.EventSystems.EventTrigger;

namespace Varneon.VUdon.Components.Editor
{
    internal static class LayerMaskDropdownUpdater
    {
        [PostProcessScene(-1)] // Ensure that all of the dropdowns get processed before U# saves the C# component data into UdonBehaviours
        private static void InitializeLayerMaskDropdowns()
        {
            foreach(LayerMaskDropdown dropdown in UnityEngine.Object.FindObjectsOfType<LayerMaskDropdown>())
            {
                UpdateLayerMaskDropdown(dropdown);
            }
        }

        private static void UpdateLayerMaskDropdown(LayerMaskDropdown layerMaskDropdown)
        {
            List<int> layerIndices = new List<int>();

            List<string> optionNames = new List<string>() { "Nothing", "Everything" };

            for (int i = 0; i < 32; i++)
            {
                string name = LayerMask.LayerToName(i);

                bool isDefined = !string.IsNullOrEmpty(name);

                if (isDefined)
                {
                    optionNames.Add(string.Format("{0}: {1}", i, name));

                    layerIndices.Add(i);
                }
            }

            Dropdown dropdown = layerMaskDropdown.GetComponent<Dropdown>();

            IEnumerable<FieldInfo> fields = typeof(LayerMaskDropdown).GetRuntimeFields();

            FieldInfo layerCountField = fields.FirstOrDefault(f => f.Name == "projectLayerCount");

            FieldInfo layersField = fields.FirstOrDefault(f => f.Name == "layers");

            EventTrigger trigger = layerMaskDropdown.GetComponent<EventTrigger>();

            layerCountField.SetValue(layerMaskDropdown, optionNames.Count - 2);

            layersField.SetValue(layerMaskDropdown, layerIndices.ToArray());

            dropdown.options = optionNames.Select(l => new Dropdown.OptionData(l)).ToList();

            UdonBehaviour ub = UdonSharpEditorUtility.GetBackingUdonBehaviour(layerMaskDropdown);

            MethodInfo sendCustomEventInfo = UnityEventBase.GetValidMethodInfo(ub, nameof(UdonBehaviour.SendCustomEvent), new[] { typeof(string) });

            UnityAction<string> sendCustomEventDelegate = Delegate.CreateDelegate(typeof(UnityAction<string>), ub, sendCustomEventInfo, false) as UnityAction<string>;

            UnityEventTools.AddStringPersistentListener(dropdown.onValueChanged, sendCustomEventDelegate, "OnValueChanged");

            Entry pointerClickEntry = new Entry
            {
                eventID = EventTriggerType.PointerClick,
                callback = new TriggerEvent()
            };
            UnityEventTools.AddStringPersistentListener(pointerClickEntry.callback, sendCustomEventDelegate, "OnClick");

            Entry selectEntry = new Entry
            {
                eventID = EventTriggerType.Select,
                callback = new TriggerEvent()
            };
            UnityEventTools.AddStringPersistentListener(selectEntry.callback, sendCustomEventDelegate, "OnSelect");

            trigger.triggers.Add(pointerClickEntry);
            trigger.triggers.Add(selectEntry);
        }
    }
}
