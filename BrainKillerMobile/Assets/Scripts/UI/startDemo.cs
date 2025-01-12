using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startDemo : MonoBehaviour
{
    public TMP_InputField userIdInput;

    public DataManager dataManager;

    public void gotoAnnotationScene() {
        // check for user id
        if (userIdInput.text == "") {
            Debug.LogWarning("Please input user id");
            return;
        }

        int userId;
        if (!int.TryParse(userIdInput.text, out userId)) {
            Debug.LogWarning("User id should be an integer");
            return;
        }
        Debug.Log("User id: " + userId);
        dataManager.userId = userId;
        // check for image path
        // if (openImage.openedImagePath == null) {
        //     Debug.LogWarning("Please open an image");
        //     return;
        // }
        // Debug.Log("Image path: " + openImage.openedImagePath);

        SceneManager.LoadScene("BPCheck");
    }
}
