using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text isReadyText;
    [SerializeField] Image characterImage;
    [SerializeField] Button kickButton;

    const string READY_TEXT = "<color=green>READY";
    const string NOT_READY_TEXT = "<color=red>NOT READY";

    public void UpdateProfile(Player player) {

        nameText.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;

        if (player.Data[LobbyManager.KEY_IS_READY].Value == "True")
            isReadyText.text = READY_TEXT;
        else
            isReadyText.text = NOT_READY_TEXT;

        switch (player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value) {
            case "Jack":
                characterImage.sprite = SpritesManager.Instance.jack;
                break;
            case "John":
                characterImage.sprite = SpritesManager.Instance.john;
                break;
            case "Jane":
                characterImage.sprite = SpritesManager.Instance.jane;
                break;
        }

        kickButton.onClick.AddListener(() => {
            LobbyManager.Instance.KickPlayer(player.Id);
        });
    }

    public void SetKickButtonVisible(bool isVisible) {
        kickButton.gameObject.SetActive(isVisible);
    }

    public void SetOwnerIcon(bool isOwner) {
        if (isOwner) {
            icon.sprite = SpritesManager.Instance.ownerIcon;
        } else {
            icon.sprite = SpritesManager.Instance.playerIcon;
        }
    }
}

