using System;
using System.Collections;
using UnityEngine;

public class CooldownHelper : MonoBehaviour
{
    public event EventHandler onTimerIsUp;

    public void StartCooldownTimer(float duration)
    {
        StartCoroutine(Countdown(duration));
    }

    private IEnumerator Countdown(float duration)
    {
        yield return new WaitForSeconds(duration);
        onTimerIsUp?.Invoke(this, EventArgs.Empty);
    }
}
