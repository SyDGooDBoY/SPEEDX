using System.Collections;
using UnityEngine;

public class StartUI : MonoBehaviour
{
    public Transform cameraTransform; // 摄像机的Transform组件，需要在编辑器中指定
    public float moveDistance = 20f; // 摄像机移动的距离
    public float duration = 2f; // 摄像机移动持续时间

    public void StartGame()
    {
        StartCoroutine(MoveCameraAndLoadScene());
    }

    IEnumerator MoveCameraAndLoadScene()
    {
        Vector3 startPosition = cameraTransform.position;
        Vector3 endPosition = startPosition + cameraTransform.forward * moveDistance; // 计算终点位置

        float elapsed = 0f;
        while (elapsed < duration)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        cameraTransform.position = endPosition; // 确保摄像机确切到达终点位置

        UnityEngine.SceneManagement.SceneManager.LoadScene(1); // 加载下一个场景
    }

    public void Options()
    {
        Debug.LogWarning("Options function is not implemented yet.");
    }
    public void OpenLink()
    {
        Application.OpenURL("https://forms.gle/1TNmd8fEALQssY1J8");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Exit game in editor"); //Quit the game function for editor
#else
        Application.Quit(); //Quit the game function for build
#endif
    }
}