using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Utilities;

namespace UIHexColor
{
    public class Palette : MonoBehaviour
    {
        public GameObject paletteButton;
        public float longPressDuration = 1.5f;
        private readonly List<Transform> paletteButtons = new List<Transform>();

        private int pencilPosition;
        private int heldButton;
        private HexInput hexInput;

        private void Awake()
        {
            hexInput = GameObject.FindObjectOfType<HexInput>();
        }

        public Color paintColor
        {
            get
            {
                return Colors.cellColors[pencilPosition];
            }
        }
        public char paintColorChar
        {
            get
            {
                return (char)(pencilPosition + '0');
            }
        }

        public void Create(int size)
        {
            if (size <= 0 || size > Colors.cellColors.Length)
            {
                Debug.Log("Incorrect palette size was passed!");
                return;
            }

            for (var i = 0; i < size; i++)
            {
                var button = Instantiate(paletteButton).transform;

                button.SetParent(gameObject.transform, false);
                button.Find("PaletteButtonFill").GetComponent<Image>().color = Colors.cellColors[i];

                var iCopy = i; // Outer variable trap
                button.gameObject.GetComponent<Button>()
                    .onClick.AddListener(delegate { ChangePencilPosition(iCopy); });

                var trigger = button.gameObject.AddComponent<EventTrigger>();

                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(delegate { OnPointerDown(iCopy); });
                trigger.triggers.Add(entry);

                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerUp;
                entry.callback.AddListener(delegate { OnPointerUp(iCopy); });
                trigger.triggers.Add(entry);

                paletteButtons.Add(button);
            }

            /*
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(delegate { PointerDown(i, j); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(delegate { PointerEnter(i, j); });
            trigger.triggers.Add(entry);
            */
            pencilPosition = size / 2;
            paletteButtons[pencilPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = true;
        }

        public void OnPointerDown(int i)
        {
            heldButton = i;
            Invoke("OnLongPress", longPressDuration);
        }

        public void OnPointerUp(int i)
        {
            CancelInvoke("OnLongPress");
        }

        private void OnLongPress()
        {     
            hexInput.colorIndex = heldButton;
            hexInput.Activate();
        }

        public void Destroy()
        {
            foreach (var button in paletteButtons)
            {
                Destroy(button.gameObject);
            }

            paletteButtons.Clear();
        }

        public void ReColor()
        {
            for (var i = 0; i < paletteButtons.Count; i++)
            {
                paletteButtons[i].Find("PaletteButtonFill").GetComponent<Image>().color = Colors.cellColors[i];
            }
        }

        private void ChangePencilPosition(int newPosition)
        {
            if (pencilPosition == newPosition)
            {
                return;
            }

            paletteButtons[pencilPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = false;

            paletteButtons[newPosition].Find("PaletteButtonIcon")
                .gameObject.GetComponent<Image>().enabled = true;

            pencilPosition = newPosition;
        }
    }
}
