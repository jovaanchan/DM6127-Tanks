using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;            // The number of rounds a single player has to win to win the game.
    public float m_StartDelay = 3f;             // The delay between the start of RoundStarting and RoundPlaying phases.
    public float m_EndDelay = 3f;               // The delay between the end of RoundPlaying and RoundEnding phases.
    public CameraControl m_CameraControl;       // Reference to the CameraControl script for control during different phases.
    public Text m_MessageText;                  // Reference to the overlay Text to display winning text, etc.
    public GameObject m_TankPrefab;             // Reference to the prefab the players will control.
    public GameObject m_AITankPrefab;           // Reference to AI Tank prefab
    public AITankManager[] m_AITanks;           // A collection of managers for AITanks
    public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.

    public Slider AmmoSlider;
    private bool stopTimer;
    private DateTime shootingEnabledTime;
    public float AmmoTime;

    private int m_RoundNumber;                  // Which round the game is currently on.
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst the round starts.
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst the round or game ends.
    private TankManager m_RoundWinner;          // Reference to the winner of the current round.  Used to make an announcement of who won.
    private TankManager m_GameWinner;           // Reference to the winner of the game.  Used to make an announcement of who won.


    private void Start()
    {
        Debug.Log("HelloStart");
        // Create the delays so they only have to be made once.
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        // Once the tanks have been created and the camera is using them as targets, start the game.
        StartCoroutine (GameLoop ());
    }


    private void SpawnAllTanks()
    {
        // For all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... create them, set their player number and references needed for control.
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }

        for (int i = 0; i < m_AITanks.Length; i++)
          {
            // create enemy tanks
            m_AITanks[i].m_Instance =
                Instantiate(m_AITankPrefab, m_AITanks[i].m_SpawnPoint.position, m_AITanks[i].m_SpawnPoint.rotation) as GameObject;
            m_AITanks[i].m_AINumber = i + 1;
            m_AITanks[i].Setup();
          }
    }

    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of tanks.
        Transform[] targets = new Transform[m_Tanks.Length];

        // For each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate tank transform.
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // These are the targets the camera should follow.
        m_CameraControl.m_Targets = targets;
    }


    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop ()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundStarting ());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine (RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine (RoundEnding());

        StartCoroutine (GameLoop ());
    }


    private IEnumerator RoundStarting ()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllTanks ();
        DisableTankControl ();
        ResetPoints();

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetStartPositionAndSize ();

        // Increment the round number and display text showing the players what round it is.
        m_MessageText.text = "ROUND START";

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying ()
    {
        // As soon as the round begins playing let the players control the tanks.
        EnableTankControl ();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;

        // Dictionary to store
        Dictionary<Int32, DateTime> deadTanks = new Dictionary<Int32, DateTime>();

        // While there is not one tank left...
        while (!PlayersStillAlive())
        {
            RespawnDeadTanks(deadTanks);
            ActivateOtherTanks();
            IncreaseSpeedOfTanks();
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding ()
    {
        // Stop tanks from moving.
        DisableTankControl ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage ();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }


    // This is used to check if there is one or fewer tanks remaining and thus the round should end.
    private bool PlayersStillAlive()
    {
        // Start the count of tanks left at zero.
        int numTanksLeft = 0;

        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if they are active, increment the counter.
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        // If there are one or fewer tanks remaining return true, otherwise return false.
        return numTanksLeft <= 0;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private TankManager GetRoundWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }


    // This function is to find out if there is a winner of the game.
    private TankManager GetGameWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        // If no tanks have enough rounds to win, return null.
        return null;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        int score = PointsManager.instance.GetScore();
        string message = "You got " + score + " points!";

        int highScore = PointsManager.instance.GetHighScore();
        if (score == highScore)
        {
          message += "\n\n";
          message += "It's a new highscore!";
        }

        return message;
    }


    // This function is used to turn all the tanks back on and reset their positions and properties.
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
        for (int i = 0; i < m_AITanks.Length; i++)
        {
          m_AITanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
            m_Tanks[i].DisableShooting();

        }
        
        m_AITanks[0].EnableControl();
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
        
        for (int i = 0; i < m_AITanks.Length; i++)
        {
            m_AITanks[i].DisableControl();
        }
    }

    private void ResetPoints()
    {
        PointsManager.instance.ResetScore();
        PointsManager.instance.Start();
    }

    private void RespawnDeadTanks(Dictionary<Int32, DateTime> deadTanks)
    {
      for (int i = 0; i < m_AITanks.Length; i++)
      {
          if (!(m_AITanks[i].m_Instance.activeSelf) && !(deadTanks.ContainsKey(i)))
          {
              var datetime = DateTime.Now;
              deadTanks.Add(i, datetime.AddSeconds(10));
          }
      }
      List<int> indexToDelete = new List<int>();
      foreach(var item in deadTanks)
      {
        if (item.Value.CompareTo(DateTime.Now) <= 0)
        {
            m_AITanks[item.Key].Reset();
            indexToDelete.Add(item.Key);
        }
      }

      for (int i = 0; i < indexToDelete.Count; i++)
      {
        deadTanks.Remove(indexToDelete[i]);
      }
    }

    private void ActivateOtherTanks()
    {
      int[] pointsToAchieve = new int[] {15, 30, 45, 60};
      int score = PointsManager.instance.GetScore();
      for (int i = 0; i < pointsToAchieve.Length; i++)
      {
        if (score < pointsToAchieve[i])
          break;
        m_AITanks[i+1].EnableControl();
      } 
    }

    public void IncreaseSpeedOfTanks()
    {
      int score = PointsManager.instance.GetScore();
      if (score % 5 == 0 && score != 0)
      {
        for (int i = 0; i < m_AITanks.Length; i++)
        {
          if (!m_AITanks[i].m_Instance.activeSelf)
          {
            break;
          }
          m_AITanks[i].SetSpeed(score);
        }
      }
    }

    public void EnableShooting()
    {
        m_Tanks[0].EnableShooting();

        // record the starting time of the shooting ability
        shootingEnabledTime = DateTime.Now;

        // fill the ammo indicator
        fillAmmoSlider();
    }

    public void fillAmmoSlider()
    {
        AmmoSlider.maxValue = AmmoTime;
        AmmoSlider.value = AmmoTime;
    }

    public void updateAmmoSlider()
    {
        float timeElapsed = (float)(DateTime.Now - shootingEnabledTime).TotalSeconds;

        if (AmmoSlider.value > 0)
        {
            AmmoSlider.value = AmmoTime - timeElapsed;
        } else {
            m_Tanks[0].DisableShooting();
        }
    }
}
