using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour {


    //^ Animation
    //private const string NUMBER_POPUP = "NumberPopup";

    [SerializeField] private TextMeshProUGUI countdownText;

    //private Animator animator;
    private int previousCountdownNumber;

    private void Awake() {
        //animator = GetComponent<Animator>();
    }

    private void Start() {
        GameStateAndTimer.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (GameStateAndTimer.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        int countdownNumber = Mathf.CeilToInt(GameStateAndTimer.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();

        if (previousCountdownNumber != countdownNumber) {
            previousCountdownNumber = countdownNumber;
            //animator.SetTrigger(NUMBER_POPUP);
            //!SoundManager.Instance.PlayCountDownSound();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
