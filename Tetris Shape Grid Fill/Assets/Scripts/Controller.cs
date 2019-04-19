using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
