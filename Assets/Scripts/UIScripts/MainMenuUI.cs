using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance { get; private set; }

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button backButton;
    [SerializeField] private Button returnMenuButton;

    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject optionsPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ✅ ikinci kopyaları sil
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ tek kopya kalır
    }

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (returnMenuButton != null)
        {
            returnMenuButton.onClick.AddListener(OnReturnMenuClicked);
            returnMenuButton.gameObject.SetActive(false);
        }

        if (menuPanel != null) menuPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsPanel != null)
                optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    private void OnPlayClicked()
    {
        SceneManagerEx.Instance.LoadNext(() =>
        {
            if (menuPanel != null) menuPanel.SetActive(false);
            if (optionsPanel != null) optionsPanel.SetActive(false);

            if (returnMenuButton != null)
                returnMenuButton.gameObject.SetActive(true);

            AudioManager.Instance?.UnmuteAll();
            MusicManager.Instance?.UnmuteAll();
        });
    }

    private void OnOptionsClicked()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnBackClicked()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void OnReturnMenuClicked()
    {
        SceneManagerEx.Instance.Load(0, () =>
        {
            if (returnMenuButton != null)
                returnMenuButton.gameObject.SetActive(false);

            if (optionsPanel != null) optionsPanel.SetActive(false);

            AudioManager.Instance?.MuteAll();
            MusicManager.Instance?.MuteAll();

            if (menuPanel != null) menuPanel.SetActive(true);
        });
    }
}
