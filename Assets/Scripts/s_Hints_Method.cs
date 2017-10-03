using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Hints_Method { 
    private IEnumerator CheckPotentialMatches()
    {
        yield return new WaitForSeconds(s_Constants.WaitBeforePotentialMatches.Check);
        PotentialMatches = cardTypeUtilities.GetPotentialMatches(shapes);
        if (potentialMatches != null)
        {
            while (true)
            {
                AnimatePotentialMatchesCoroutine = cardTypeUtilities.AnimatePotentialMatches(potentialMatches);
                StartCoroutine(AnimatePotentialMatchesCoroutine);
                yield return new WaitForSeconds(s_Constants.WaitBeforePotentialMatchesCheck); 
            }
        }
    }

    private void ResetOpacityOnPotentialMatches()
    {
        if (PotentialMatches != null)
            foreach (var item in PotentialMatches)
            {
                if (item == null) break;

                Color c = item.GetComponent<SpriteRenderer>().color;
                c.a = 1.0f;
                item.GetComponent<SpriteRenderer>().color = c;
            }
    }

    private void StartCheckForPotentialMatches()
    {
        StopCheckForPotentialMatches();
        CheckPotentialMatchesCoroutine = CheckPotentialMatches();
        StartCoroutine(CheckPotentialMatchesCoroutine);
    }

    private void StopCheckForPotentialMatches()
    {
        if (AnimatePotentialMatchesCoroutine != null)
            StopCoroutine(AnimatePotentialMatchesCoroutine);
        if (CheckPotentialMatchesCoroutine != null)
            StopCoroutine(CheckPotentialMatchesCoroutine);
        ResetOpacityOnPotentialMatches();
    }
}
