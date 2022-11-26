using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Race : MonoBehaviour
{
    public static Race instance;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(this);
    }

    [SerializeField] int totalLaps = 3;

    float countdown = 0f;
    float raceTime = 0f;
    bool raceActive = false;
    Dictionary<GameObject, Racer> racers = new Dictionary<GameObject, Racer>();
    public List<Checkpoint> checkpoints = new List<Checkpoint>();
    List<TrackBoundsVolume> trackBoundsVolumes = new List<TrackBoundsVolume>();

    public UnityEvent<float> RaceTimeUpdate;
    public UnityEvent<string> CountdownTextUpdate;
    public UnityEvent<Player, int> PlayerLapUpdate;
    public UnityEvent RaceEnded;

    #region Public Interface
    public static void ResetRacerToLastCheckpoint(GameObject racerGameObject){
        if (!instance) return;
        if (!instance.racers.ContainsKey(racerGameObject)) return;

        Transform lastCheckpoint = instance.racers[racerGameObject].lastCheckpoint.transform;
        racerGameObject.transform.position = lastCheckpoint.position;
        racerGameObject.transform.rotation = lastCheckpoint.rotation;
        racerGameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
    #endregion

    private void Start()
    {
        SubscribeToCheckpoints();
        GetRacers();
        GetTrackBoundsVolumes();
        StartCoroutine(StartCountdown());
    }

    private void GetRacers(){
        Player[] players = FindObjectsOfType<Player>();
        foreach(Player player in players){
            Racer newRacer = new Racer();
            newRacer.player = player;
            newRacer.lastCheckpoint = checkpoints[0];
            newRacer.nextCheckpoint = checkpoints[1];

            racers.Add(player.GetCarGameObject(), newRacer);
        }
    }

    private void SubscribeToCheckpoints(){
        foreach(Checkpoint checkpoint in checkpoints){
            checkpoint.OnCheckpointPassed.AddListener(CheckpointPassedCallback);
        }
    }

    void GetTrackBoundsVolumes(){
        trackBoundsVolumes = new List<TrackBoundsVolume>(FindObjectsOfType<TrackBoundsVolume>());
        foreach(TrackBoundsVolume trackBoundsVolume in trackBoundsVolumes){
            trackBoundsVolume.OnGameObjectOutOfBounds.AddListener(GameObjectOutOfBoundsCallback);
        }
    }

    void CheckpointPassedCallback(GameObject gameObject, Checkpoint checkpoint){
        // GameObject is a racer and this checkpoint is the racer's next checkpoint
        if (racers.ContainsKey(gameObject) && racers[gameObject].nextCheckpoint == checkpoint){
            int checkpointIndex = checkpoints.IndexOf(checkpoint);

            if (checkpointIndex == 0){
                RacerCompletedLap(racers[gameObject]);
                
            }

            checkpointIndex++;
            if (checkpointIndex >= checkpoints.Count) checkpointIndex = 0;

            racers[gameObject].lastCheckpoint = racers[gameObject].nextCheckpoint;
            racers[gameObject].nextCheckpoint = checkpoints[checkpointIndex];

            checkpoint.CheckpointPassed();
        }
    }

    void RacerCompletedLap(Racer racer){
        racer.currentLap++;
        racer.NewLapTime(raceTime);

        if (racer.currentLap > totalLaps) {
            racer.player.DisableCarControl();
            racer.raceFinished = true;
        }
        else{
            PlayerLapUpdate.Invoke(racer.player, racer.currentLap);
        }
    }

    void GameObjectOutOfBoundsCallback(GameObject gameObject){
        ResetRacerToLastCheckpoint(gameObject);
    }

    bool GetAllRacersFinished(){
        bool temp = true;
        int index = 0;
        foreach(KeyValuePair<GameObject, Racer> racer in racers){
            
            if (!racer.Value.raceFinished) temp = false;
            break;
        }

        return temp;
    }



    IEnumerator StartCountdown(){
        foreach(KeyValuePair<GameObject, Racer> pair in racers){
            pair.Value.player.DisableCarControl();
        }        

        raceActive = false;
        countdown = 3f;
        
        CountdownTextUpdate.Invoke("3");
        
        yield return new WaitForSeconds(1f);
        while(countdown > 0f){
            countdown -= Time.deltaTime;
            if (countdown > 0f) CountdownTextUpdate.Invoke(countdown.ToString("0"));
            else CountdownTextUpdate.Invoke("Go!");
            yield return null;
        }

        
        StartCoroutine(StartRace());
        yield return new WaitForSeconds(1f);

        CountdownTextUpdate.Invoke("");
    }

    IEnumerator StartRace(){
        foreach(KeyValuePair<GameObject, Racer> pair in racers){
            pair.Value.player.EnableCarControl();
        }     

        raceActive = true;
        while(!GetAllRacersFinished()){
            
            raceTime += Time.deltaTime;
            RaceTimeUpdate.Invoke(raceTime);
            yield return null;
        }
        
        yield return new WaitForSeconds(1f);
        StartCoroutine(EndRace());
    }

    IEnumerator EndRace(){
        RaceEnded.Invoke();
        yield return null;

        foreach(KeyValuePair<GameObject, Racer> racer in racers){
            Debug.Log($"race time: {racer.Value.RaceTime()}");
            Leaderboard.NewLeaderboardEntry(racer.Value.RaceTime());
        }

        Leaderboard.Save();
        yield return new WaitForSeconds(1f);
        Leaderboard.PrintLB();
    }

    [System.Serializable]
    private class Racer
    {
        public Player player;
        public int currentLap = 1;
        public bool raceFinished = false;
        public List<float> lapTimes = new List<float>();
        public Checkpoint lastCheckpoint;
        public Checkpoint nextCheckpoint;

        public void NewLapTime(float newLapTime){
            lapTimes.Add(newLapTime);
           
        }

        public float RaceTime(){
            return lapTimes[lapTimes.Count - 1];
        }
    }

}
