using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> destinations;
    [SerializeField]
    private Transform pDestination;
    [SerializeField]
    private Transform wallDestination;

    private List<CharacterController> runners;
    [SerializeField]
    private TextMeshProUGUI timerText;

    public event EventHandler<EventArgs> TimeUpEvent;
    public event EventHandler<GamePhaseChangeEventArgs> PhaseChangeEvent;


    private static GameManager instance;

    public static GameManager Instance { get => instance; }
    public CharacterController[] Runners { get => runners.ToArray(); }

    // Start is called before the first frame update
    private float timer = 30f;
    private float countdown;
    private bool isGameEnd;
    private void Awake()
    {
        if(Instance == null)
        {
            instance = this;
            initializeRunners();
            DontDestroyOnLoad(gameObject);

            return;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }
    void Start()
    {
        isGameEnd = false;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().PlayerFinishRun += PlayerFinishRunningListener;
        restartGame();
    }

    private void FixedUpdate()
    {
        if (isGameEnd)
            return;
        if(countdown > 0f) 
        {
            countdown -= Time.deltaTime;
            timerText.text = "Time: " + countdown;
        }
        else 
        {
            timerText.text = "Time: " + 0;
            TimeUpEvent?.Invoke(this, new EventArgs());
            for(int i = 0; i < runners.Count; i++) 
            {
                runners[i].StopIT(true);
            }
            isGameEnd = true;
            
            return;
        }

    }
    private void initializeRunners()
    {
        runners = new List<CharacterController>();
        foreach (GameObject gg in GameObject.FindGameObjectsWithTag("Enemy"))
            runners.Add(gg.GetComponent<CharacterController>());
        runners.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>());
    }
    private void restartGame() 
    {
        countdown = timer;
        List<Vector3> tmpdestinations = new List<Vector3>();
        foreach (Transform ts in destinations)
        {
            tmpdestinations.Add(ts.position);
        }

        foreach (CharacterController nma in runners)
        {

            if (nma.gameObject.CompareTag("Player"))
            {
                nma.DestinationPoint = pDestination.position;
            }
            else
            {
                int decision = UnityEngine.Random.Range(0, tmpdestinations.Count);
                nma.DestinationPoint = tmpdestinations[decision];
                tmpdestinations.RemoveAt(decision);
            }
            nma.restartPlayer();
            nma.StopIT(false);

        }
        isGameEnd = false;
    }
    public void playAgain() 
    {
        PhaseChangeEvent?.Invoke(this, new GamePhaseChangeEventArgs(GameObject.FindGameObjectWithTag("Player").transform, 1, new Vector3(0, 8, -10)));
        restartGame();
    }

    public void PlayerFinishRunningListener(object sender, EventArgs e)
    {
        PhaseChangeEvent?.Invoke(this, new GamePhaseChangeEventArgs(wallDestination, 2, new Vector3(0, 2, -7)));
    }

}

public class GamePhaseChangeEventArgs : EventArgs
{
    private Transform _currentTrack;
    private int _gamePhaseIndex;
    private Vector3 _currentTrackOffset = Vector3.zero;

    public int CurrentGamePhase { get => _gamePhaseIndex; }
    public Transform CurrentCameraTrack { get => _currentTrack; }
    public Vector3 CurrentTrackOffset { get => _currentTrackOffset; }

    public GamePhaseChangeEventArgs(Transform t, int GamePhase, Vector3 cameraOffset)
    {
        _currentTrack = t;
        _gamePhaseIndex = GamePhase;
        _currentTrackOffset = cameraOffset;
    }


}
