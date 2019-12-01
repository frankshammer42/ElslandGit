using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loadSceneFalling : MonoBehaviour{
    private AsyncOperation _asyncLoading;
    
    
  

 public void StartLoadFallingScene()
    {
//        SceneManager.LoadScene("01Falling");
        StartCoroutine(loadScene());
    }

 public void LoadFallingScene(){
     _asyncLoading.allowSceneActivation = true;
 }
 
 IEnumerator loadScene(){
    _asyncLoading = SceneManager.LoadSceneAsync("01Falling");
    _asyncLoading.allowSceneActivation = false;
    yield return null;
//    while(!async.isDone){
//        yield return null;
//    }
 }
}
