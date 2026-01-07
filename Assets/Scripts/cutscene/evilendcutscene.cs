using System.Collections;
using UnityEngine;

public class WorldTransitionCutscene : MonoBehaviour
{
    [Header("Target Objects")]
    public GameObject playerObject;
    public GameObject jupiterObject;

    [Header("Timing & Movement")]
    public float initialWait = 2f;
    public float slideDuration = 1.5f;
    public float jupiterStartDelay = 1.0f; // Delay after player starts moving
    public float postSequenceWait = 2f;


    /// Triggers the cutscene sequence on Start.

    void Start()
    {
        if (playerObject != null && jupiterObject != null)
        {
            StartCoroutine(CutsceneSequence());
        }
    }


    /// Handles the staggered movement: Player moves, Jupiter waits 1s then moves, then Jupiter is deleted.

    IEnumerator CutsceneSequence()
    {
        // Initial wait before anything happens
        yield return new WaitForSeconds(initialWait);

        // Define Start and End positions
        // Player moves 8 blocks up now (9 - 1)
        Vector3 playerStart = playerObject.transform.position;
        Vector3 playerEnd = playerStart + new Vector3(0, 8, 0);

        Vector3 jupiterStart = jupiterObject.transform.position;
        Vector3 jupiterEnd = jupiterStart + new Vector3(0, 5, 0);

        float elapsedTime = 0;

        // The main movement loop
        // The loop continues until the longest action (Jupiter's delayed finish) is done
        while (elapsedTime < (slideDuration + jupiterStartDelay))
        {
            elapsedTime += Time.deltaTime;

            // --- Player Movement Logic ---
            float playerPct = elapsedTime / slideDuration;
            if (playerPct <= 1.0f)
            {
                playerObject.transform.position = Vector3.Lerp(playerStart, playerEnd, playerPct);
            }

            // --- Jupiter Movement Logic (Staggered by 1s) ---
            float jupiterElapsedTime = elapsedTime - jupiterStartDelay;
            if (jupiterElapsedTime > 0)
            {
                float jupiterPct = jupiterElapsedTime / slideDuration;
                if (jupiterPct <= 1.0f)
                {
                    jupiterObject.transform.position = Vector3.Lerp(jupiterStart, jupiterEnd, jupiterPct);
                }
            }

            // --- LOCK ORIENTATION ---
            // Forces Jupiter to stay upright to prevent shaking/rotation bugs
            if (jupiterObject != null)
            {
                jupiterObject.transform.rotation = Quaternion.identity;
            }

            yield return null; // Wait for next frame
        }

        // Ensure final snapping and orientation lock
        playerObject.transform.position = playerEnd;
        if (jupiterObject != null)
        {
            jupiterObject.transform.position = jupiterEnd;
            jupiterObject.transform.rotation = Quaternion.identity;
        }

        // Final wait and cleanup
        yield return new WaitForSeconds(postSequenceWait);

        if (jupiterObject != null)
        {
            Destroy(jupiterObject);
        }
    }
}