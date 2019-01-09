using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainMenuControler : MonoBehaviour {

    [SerializeField]
    private PlayerImages playerImages;
    [SerializeField]
    private GameObject gridSelectionPanel;
    [SerializeField]
    private GameObject player1ImageHolder;
    [SerializeField]
    private GameObject player2ImageHolder;
    [SerializeField]
    private GameObject buttonObj;

    void Start () {
        InitializePlayerImages();
        
    }

    public void Grid (int num)
    {
        PlayerPrefs.SetInt("Grid", num);

        gridSelectionPanel.transform.DOMoveX(-8, 1.0f, false);
        player1ImageHolder.transform.parent.transform.DOMoveX(0, 1.0f, false);
    }

    public void SetPlayer1ImageIndex(int index)
    {
        PlayerPrefs.SetInt("Player1ImageIndex", index);

        Button[] btns = player2ImageHolder.GetComponentsInChildren<Button>();
        btns[index].interactable = false;

        player1ImageHolder.transform.parent.transform.DOMoveX(-8, 1.0f, false);
        player2ImageHolder.transform.parent.transform.DOMoveX(0, 1.0f, false);
    }

    public void SetPlayer2ImageIndex(int index)
    {
        PlayerPrefs.SetInt("Player2ImageIndex", index);
        SceneManager.LoadScene("GameView");
    }

    public void InitializePlayerImages()
    {
        for (int i = 0; i < playerImages.sprites.Length; i++)
        {
            int temp = i;
            GameObject button1 = Instantiate(buttonObj, transform.position, Quaternion.identity);
            button1.GetComponent<Button>().onClick.RemoveAllListeners();
            button1.GetComponent<Button>().onClick.AddListener(() => SetPlayer1ImageIndex(temp));
            button1.GetComponent<Image>().sprite = playerImages.sprites[i];
            button1.transform.SetParent(player1ImageHolder.transform, false);

            GameObject button2 = Instantiate(buttonObj, transform.position, Quaternion.identity);
            button2.GetComponent<Button>().onClick.RemoveAllListeners();
            button2.GetComponent<Button>().onClick.AddListener(() => SetPlayer2ImageIndex(temp));
            button2.GetComponent<Image>().sprite = playerImages.sprites[i];
            button2.transform.SetParent(player2ImageHolder.transform, false);
        }
    }
}
