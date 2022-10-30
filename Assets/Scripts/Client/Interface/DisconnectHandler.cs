using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ClientManager.Instance == null)
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
        if(ClientManager.Instance.Client.ConnectionState == DarkRift.ConnectionState.Disconnected)
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
    }
}
