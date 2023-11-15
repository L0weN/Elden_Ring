using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

namespace MANAGERS
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;

        [Header("Menus")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;

        [Header("Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button mainMenuLoadGameButton;
        [SerializeField] Button deleteCharacterPopUpConfirmButton;

        [Header("Pop ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] Button noCharacterSlotsOkayButton;
        [SerializeField] GameObject deleteCharacterSlotPopUp;

        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }

        public void OpenLoadGameMenu()
        {
            titleScreenMainMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            loadMenuReturnButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            titleScreenLoadMenu.SetActive(false);
            titleScreenMainMenu.SetActive(true);
            mainMenuLoadGameButton.Select();
        }

        public void DisplayNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(true);
            noCharacterSlotsOkayButton.Select();
        }

        public void CloseNoFreeCharacterSlotsPopUp()
        {
            noCharacterSlotsPopUp.SetActive(false);
            mainMenuNewGameButton.Select();
        }

        public void SelectCharacterSlot(CharacterSlot characterSlot)
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot()
        {
            currentSelectedSlot = CharacterSlot.NO_SLOT;
        }

        public void AttemptToDeleteCharacterSlot()
        {
            if (currentSelectedSlot != CharacterSlot.NO_SLOT)
            {
                deleteCharacterSlotPopUp.SetActive(true);
                deleteCharacterPopUpConfirmButton.Select();
            }
        }

        public void DeleteCharacterSlot()
        {
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
            CloseDeleteCharacterPopUp();
        }

        public void CloseDeleteCharacterPopUp()
        {
            deleteCharacterSlotPopUp.SetActive(false);
            loadMenuReturnButton.Select();
        }
    }
}