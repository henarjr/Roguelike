using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public BoardManager BoardManager;
    public PlayerController PlayerController;
    public TurnManager TurnManager { get; private set; }

    private int m_FoodAmount = 100;
    public UIDocument UIDoc;
    private Label m_FoodLabel;

    private int m_CurrentLevel = 1;

    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;
    private Label m_HowManyDaysMessage;

    private Label m_DayLabel;
    private Label m_ZombiesLabel;
    private int m_ZombiesKilled = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");
        m_HowManyDaysMessage = m_GameOverPanel.Q<Label>("HowManyDaysMessage");

        m_DayLabel = UIDoc.rootVisualElement.Q<Label>("DayLabel");
        m_ZombiesLabel = UIDoc.rootVisualElement.Q<Label>("ZombiesLabel");

        StartNewGame();
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        if (PlayerPrefs.HasKey("SavedLevel"))
        {
            m_CurrentLevel = PlayerPrefs.GetInt("SavedLevel");
            m_FoodAmount = PlayerPrefs.GetInt("SavedFood");
        }
        else
        {
            m_CurrentLevel = 1;
            m_FoodAmount = 20;
        }

        if (m_DayLabel != null) m_DayLabel.text = "Day " + m_CurrentLevel;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        m_ZombiesKilled = 0;
        if (m_ZombiesLabel != null) m_ZombiesLabel.text = "zombies: " + m_ZombiesKilled;

        BoardManager.Clean();

        int extraSize = (m_CurrentLevel / 10) * 2;
        BoardManager.Width = 12 + extraSize;
        BoardManager.Height = 12 + extraSize;

        BoardManager.Init(m_CurrentLevel);
        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    public void NewLevel()
    {
        m_CurrentLevel++;
        BoardManager.Clean();
        if (m_DayLabel != null) m_DayLabel.text = "Day " + m_CurrentLevel;

        int extraSize = (m_CurrentLevel / 10) * 2;
        BoardManager.Width = 12 + extraSize;
        BoardManager.Height = 12 + extraSize;

        BoardManager.Init(m_CurrentLevel);
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    void OnTurnHappen()
    {
        ChangeFood(-1);
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        if (m_FoodAmount <= 0)
        {
            PlayerPrefs.DeleteAll();

            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            m_GameOverMessage.text = "Game Over!";
            m_HowManyDaysMessage.text = "Survived for " + m_CurrentLevel + " days\nPress Enter to try again";
        }
    }

    public void SaveProgress(string sceneName)
    {
        PlayerPrefs.SetString("SavedEnvironment", sceneName);
        PlayerPrefs.SetInt("SavedLevel", m_CurrentLevel);
        PlayerPrefs.SetInt("SavedFood", m_FoodAmount);
        PlayerPrefs.Save();
    }

    public void AddZombieKill()
    {
        m_ZombiesKilled++;
        if (m_ZombiesLabel != null) m_ZombiesLabel.text = "zombies: " + m_ZombiesKilled;
    }
}