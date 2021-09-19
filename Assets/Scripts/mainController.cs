using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class mainController : MonoBehaviour
{
    [SerializeField]
    private List<Transform> destinations;
    private List<GameObject> distances;
    [SerializeField]
    private TextMeshProUGUI timerText;
    [SerializeField]
    private List<TextMeshProUGUI> listText;

    [SerializeField]
    private GameObject finishUI;
    [SerializeField]
    private TextMeshProUGUI winnerAnnouncement;
    // Start is called before the first frame update
    private float timer = 30f;
    private float countdown;
    private bool isGameEnd;
    void Start()
    {
        finishUI.SetActive(false);
        isGameEnd = false;
        distances = new List<GameObject>();
        foreach (GameObject gg in GameObject.FindGameObjectsWithTag("Enemy"))
            distances.Add(gg);
        distances.Add(GameObject.FindGameObjectWithTag("Player"));
        restartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            int count = 0;
            winnerAnnouncement.text = "Winner List\n";
            for(int i = 0; i < distances.Count; i++) 
            {
                if (distances[i].GetComponent<playerController>().isStop && count < 3) 
                {
                    winnerAnnouncement.text += (count + 1) + ". " + distances[i].name + "\n";
                    count++;
                }
                distances[i].GetComponent<playerController>().StopIT(true);
            }
            finishUI.SetActive(true);
            isGameEnd = true;
            
            return;
        }

        distances.Sort(delegate (GameObject a, GameObject b) {
            return (a.GetComponent<playerController>().destinationDistance).CompareTo(b.GetComponent<playerController>().destinationDistance);
        });
        for (int i = 0; i < distances.Count; i++)
        {
            listText[i].text = (i + 1) + " - " + distances[i].name + " - " + distances[i].GetComponent<playerController>().destinationDistance + " Meters";
        }
    }
    private void restartGame() 
    {
        countdown = timer;
        List<Vector3> tmpdestinations = new List<Vector3>();
        foreach (Transform ts in destinations)
        {
            tmpdestinations.Add(ts.position);
        }

        foreach (GameObject nma in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            int decision = UnityEngine.Random.Range(0, tmpdestinations.Count);
            nma.GetComponent<NavMeshAgent>().SetDestination(tmpdestinations[decision]);
            nma.GetComponent<playerController>().destinatoin = destinations[decision];
            tmpdestinations.RemoveAt(decision);
            nma.GetComponent<playerController>().restartPlayer();
            nma.GetComponent<playerController>().StopIT(false);
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>().restartPlayer();
        GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>().StopIT(false);
        foreach(TextMeshProUGUI tmpro in listText) 
        {
            tmpro.text = "Waiting ...";
        }
        isGameEnd = false;
    }
    public void playAgain() 
    {
        finishUI.SetActive(false);
        this.restartGame();
    }
    public void exitGame() 
    {
        Application.Quit();
    }
}
