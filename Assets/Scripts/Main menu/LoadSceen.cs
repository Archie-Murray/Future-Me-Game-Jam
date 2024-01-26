using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceen : MonoBehaviour {
    public Button _Button;
    // Start is called before the first frame update
    void Start() {
        Button btn = _Button.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }


    // Update is called once per frame
    void Update() {

    }
    void TaskOnClick() {
        SceneManager.LoadScene("Testing");
    }
}
