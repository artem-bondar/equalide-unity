using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Managers;

namespace UI
{
    public class PackList : MonoBehaviour
    {
        public GameObject packButton;

        public Sprite lockOpenSprite;
        public Sprite starSprite;

        private GameManager gameManager;

        private void Awake() => gameManager = GameObject.FindObjectOfType<GameManager>();

        // Receives string that represent packs's states,
        // one char for one entity with next alphabet:
        // 'c' - closed
        // 'o' - opened
        // 's' - solved
        public void Create(string packsStates)
        {
            for (var i = 0; i < packsStates.Length; i++)
            {
                var newButton = Instantiate(packButton).transform;
                newButton.SetParent(gameObject.transform, false);

                if (packsStates[i] != 'c')
                {
                    newButton.Find("PackButtonIcon").gameObject.GetComponent<Image>().sprite =
                        packsStates[i] == 's' ? starSprite : lockOpenSprite;
                }

                newButton.Find("PackButtonText").gameObject
                    .GetComponent<Text>().text = $"Pack {i + 1}";

                var iCopy = i; // Outer variable trap
                newButton.GetComponent<Button>().onClick.AddListener(
                    delegate { gameManager.OnPackSelect(iCopy); });
            }
        }

        // Changes icon of pack button to lock open or star icon
        public void UpdatePackIcon(int packIndex, bool toStar = false)
        {
            gameObject.transform.GetChild(packIndex).Find("PackButtonIcon").gameObject
                .GetComponent<Image>().sprite = toStar ? starSprite : lockOpenSprite;
        }
    }
}
