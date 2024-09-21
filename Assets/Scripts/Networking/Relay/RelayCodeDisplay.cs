using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RelayCodeDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text display;
    [SerializeField] Button button;
    // Start is called before the first frame update

    void Start()
    {
        if (RelayManager.JoinCode == null){
            Destroy(gameObject);
            return;
        }
        
        display.text = "Code: " + RelayManager.JoinCode;
        button.onClick.AddListener(CopyCodeToClipboard);
    }

    void CopyCodeToClipboard() {
        GUIUtility.systemCopyBuffer = RelayManager.JoinCode;
        Debug.Log("Copied to clipboard");
    }
}
