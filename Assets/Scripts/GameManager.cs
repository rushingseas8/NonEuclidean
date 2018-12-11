using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

// Testing dynamic scene loading.
public class GameManager : MonoBehaviour {

    private static bool alreadyLoaded = false;

	// Use this for initialization
	void Start () {
        StartCoroutine(LoadScene());
	}

    private IEnumerator LoadScene() {

        if (!alreadyLoaded)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync("Tea Room", LoadSceneMode.Additive);
            //op.allowSceneActivation = false;
            alreadyLoaded = true;

            yield return new WaitUntil(() => op.isDone);
            Debug.Log("Done!");

            op.completed += (a) =>
            {
                //op.allowSceneActivation = false   ;

                Debug.Log("Loaded!");
            };

        }
        yield return null;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
