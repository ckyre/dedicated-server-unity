using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else if (instance != this)
        {
            Destroy(this);
        }
    }
    #endregion

    public GameObject mainDiv, playModal;
    public TMP_InputField ipField, portField, pseudoField;
    public GameObject errorDiv;

    void Start()
    {
        ipField.text = PlayerPrefs.GetString("ip", "");
        portField.text = (PlayerPrefs.GetInt("port", -1) == -1) ? "" : PlayerPrefs.GetInt("port", -1).ToString();
        pseudoField.text = PlayerPrefs.GetString("pseudo", "");
    }

    public void ConnectButton ()
    {
        if(ipField.text != "" & portField.text != "" & pseudoField.text != "")
        {
            StartCoroutine(ConnectUICoroutine());
        }
    }

    private IEnumerator ConnectUICoroutine ()
    {
        NetworkManager.instance.Connect(ipField.text, int.Parse(portField.text));
        playModal.SetActive(false);

        PlayerPrefs.SetString("ip", ipField.text);
        PlayerPrefs.SetInt("port", int.Parse(portField.text));
        PlayerPrefs.SetString("pseudo", pseudoField.text);

        mainDiv.GetComponent<Animator>().SetBool("fullscreen", true);
        yield return new WaitForSeconds(1.5f);
        mainDiv.GetComponent<Animator>().SetBool("closed", true);
    }

    public void ShowError (string message)
    {
        StopCoroutine(ErrorDivCoroutine());
        errorDiv.SetActive(false);
        errorDiv.GetComponentInChildren<TMP_Text>().text = message;
        StartCoroutine(ErrorDivCoroutine());
    }
    private IEnumerator ErrorDivCoroutine ()
    {
        errorDiv.SetActive(true);
        yield return new WaitForSeconds(4f);
        errorDiv.SetActive(false);
    }
}
