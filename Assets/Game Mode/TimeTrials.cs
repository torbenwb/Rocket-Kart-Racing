using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeTrials : MonoBehaviour
{
    Player player;
    [SerializeField] float countdownTime = 3f;
    [SerializeField] List<Checkpoint> checkpoints; // Store checkpoints in sequential order
    [SerializeField] int checkpointIndex; // Keep track of next target checkpoint

    [SerializeField] string trackName;
    [SerializeField] int totalLaps = 3;
    int currentLap = 0;
    float startTime;
    bool raceActive = false;
    float GetRaceTime() => (Time.time - startTime) - countdownTime;
    float GetCountdownTime() => countdownTime - (Time.time - startTime);


    private void Start()
    {
        PreRaceSetup();
    }

    private void Update()
    {
        if (!raceActive) return;
        if (Input.GetKeyDown(KeyCode.Z)) ResetPlayer();
    }

    void OnCheckpointPassed(Checkpoint checkpoint, GameObject gameObject)
    {
        checkpoint.SetCheckpointEnabled(false);

        // If passed first checkpoint - new lap.
        if (checkpointIndex == 0)
        {
            currentLap++;
            if (currentLap > totalLaps) EndRace();
        }

        checkpointIndex++;
        if (checkpointIndex >= checkpoints.Count) checkpointIndex = 0;
        checkpoints[checkpointIndex].SetCheckpointEnabled(true);
    }

    void OnBoundsVolumeEnter(Rigidbody rigidbody)
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        int lastCheckpointIndex = checkpointIndex - 1;
        if (lastCheckpointIndex < 0) lastCheckpointIndex = checkpoints.Count - 1;

        rigidbody.transform.position = checkpoints[lastCheckpointIndex].transform.position;
        rigidbody.transform.rotation = Quaternion.LookRotation(-checkpoints[lastCheckpointIndex].transform.up);
    }

    public void ResetPlayer()
    {
        Rigidbody rigidbody = player.gameObject.GetComponentInChildren<Rigidbody>();

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;

        int lastCheckpointIndex = checkpointIndex - 1;
        if (lastCheckpointIndex < 0) lastCheckpointIndex = checkpoints.Count - 1;

        rigidbody.transform.position = checkpoints[lastCheckpointIndex].transform.position;
        rigidbody.transform.rotation = Quaternion.LookRotation(-checkpoints[lastCheckpointIndex].transform.up);
    }

    private void PreRaceSetup()
    {
        player = FindObjectOfType<Player>();
        player.SetControlEnabled(false);

        Checkpoint.OnCheckpointPassed.AddListener(OnCheckpointPassed);
        // Added after Bounds Volume script has been written
        //BoundsVolume.OnBoundsVolumeEnter.AddListener(OnBoundsVolumeEnter);

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetCheckpointEnabled(false);
        }
        checkpoints[checkpointIndex].SetCheckpointEnabled(true);

        startTime = Time.time;
        raceActive = true;

        StartCountdown();
    }

    private void StartCountdown()
    {
        Invoke("StartRace", countdownTime);
    }

    private void StartRace()
    {
        player.SetControlEnabled(true);
    }

    private void EndRace()
    {
        raceActive = false;
        player.SetControlEnabled(false);

        HighScore();
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
    }

    private void HighScore()
    {
        float highScore = PlayerPrefs.GetFloat($"{trackName}_HighScore", 0.0f);
        float raceTime = GetRaceTime();
        if (raceTime < highScore || highScore == 0f)
        {
            highScore = raceTime;
            PlayerPrefs.SetFloat($"{trackName}_HighScore", highScore);
            PlayerPrefs.Save();
        }
        Invoke("Restart", 3f);
    }
}