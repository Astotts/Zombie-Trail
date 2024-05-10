using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode.Transports.UTP;
using TMPro;


public class NetworkSetup : MonoBehaviour
{
    [SerializeField] GameObject menuUI;

    [SerializeField] Button host;
    [SerializeField] Button client;
    [SerializeField] Button server;
    [SerializeField] TextMeshProUGUI ipAddressText;
	[SerializeField] TMP_InputField ip;
    [SerializeField] string ipAddress;
	[SerializeField] UnityTransport transport;
    void Awake() {
		ipAddress = "0.0.0.0\n";
		SetIpAddress(); // Set the Ip to the above address
        host.onClick.AddListener(delegate{StartGame(HostType.Host);});
        client.onClick.AddListener(delegate{StartGame(HostType.Client);});
        server.onClick.AddListener(delegate{StartGame(HostType.Server);});
    }

    void StartGame(HostType type) {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer) return;
        menuUI.SetActive(false);
        switch (type) {
            case HostType.Host:
                NetworkManager.Singleton.StartHost();
		        GetLocalIPAddress();
                break;
            case HostType.Client:
                ipAddress = ip.text;
                SetIpAddress();
                NetworkManager.Singleton.StartClient();
                break;
            case HostType.Server:
                NetworkManager.Singleton.StartServer();
                break;
        }
    }
    
	public string GetLocalIPAddress() {
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList) {
			if (ip.AddressFamily == AddressFamily.InterNetwork) {
				ipAddressText.text = ipAddressText.text + "IP: " + ip.ToString() + "\n";
				ipAddress = ip.ToString();
			}
		}
		return "asda";
		throw new System.Exception("No network adapters with an IPv4 address in the system!");
	}
    
	public void SetIpAddress() {
		transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		transport.ConnectionData.Address = ipAddress;
        ipAddressText.text = "IP: " + ipAddress;
	}
}
