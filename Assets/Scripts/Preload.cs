using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preload : MonoBehaviour
{
    public GameObject loadWindows;
    void Start()
    {
        loadWindows.SetActive(true);
        StartCoroutine(PreLoading());
    }

    IEnumerator PreLoading()
    {
        yield return new WaitForSeconds(6f);
        loadWindows.SetActive(false);
    }
}
