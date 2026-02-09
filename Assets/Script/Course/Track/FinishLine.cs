using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private bool isStartLine = true;
    [SerializeField] private float minTimeBetweenPasses = 2f; // Réduit à 2s pour tester
    
    private RaceTimer raceTimer;
    private float lastPassTime;

    void Start()
    {
        raceTimer = FindFirstObjectByType<RaceTimer>();
        if (raceTimer == null) Debug.LogError("!!! RACE TIMER NON TROUVÉ DANS LA SCÈNE !!!");
        
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // LOG DE TEST : Est-ce que quelque chose touche la ligne ?
        Debug.Log($"[FinishLine] Objet détecté : {other.name} | Tag : {other.tag}");

        if (other.CompareTag("Player") && raceTimer != null)
        {
            Debug.Log($"[FinishLine] Joueur détecté ! isStartLine: {isStartLine} | isRunning: {raceTimer.IsRunning()}");

            // Cas 1 : Départ
            if (isStartLine && !raceTimer.IsRunning())
            {
                Debug.Log("[FinishLine] Tentative de StartRace...");
                raceTimer.StartRace();
                lastPassTime = Time.time;
            }
            // Cas 2 : Passage de tour ou Arrivée
            else if (raceTimer.IsRunning())
            {
                if (Time.time > lastPassTime + minTimeBetweenPasses)
                {
                    Debug.Log("[FinishLine] Tentative de PassLine...");
                    raceTimer.PassLine();
                    lastPassTime = Time.time;
                }
                else
                {
                    Debug.LogWarning("[FinishLine] Passage trop rapide ! (Anti-double déclenchement)");
                }
            }
        }
    }

    public int GetCurrentLap() => raceTimer != null ? raceTimer.GetCurrentLap() : 0;
    public int GetRequiredLaps() => raceTimer != null ? raceTimer.GetRequiredLaps() : 0;
}