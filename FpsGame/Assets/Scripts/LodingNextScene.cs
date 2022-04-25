using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LodingNextScene : MonoBehaviour
{
    // ���� ���� �񵿱� ������� �ε��ϰ� �ʹ�.
    // ����, ���� ������ �ε� ������� �ð������� ǥ���ϰ� �ʹ�.

    // ������ �� ��ȣ
    public int sceneNumber = 2;

    // �ε� �����̴� ��
    public Slider loadingBar;

    // �ε� ���� �ؽ�Ʈ
    public Text loadingText;

    void Start()
    {
        // �񵿱� �� �ε� �ڷ�ƾ�� �����Ѵ�.
        StartCoroutine(TransitionNextScene(sceneNumber));
    }

    // �񵿱� �� �ε� �ڷ�ƾ
    IEnumerator TransitionNextScene(int num)
    {
        // ������ ���� �񵿱� �������� �ε��Ѵ�.
        AsyncOperation ao = SceneManager.LoadSceneAsync(num);

        // �ε�Ǵ� ���� ����� ȭ�鿡 ������ �ʰ� �Ѵ�.
        ao.allowSceneActivation = false;

        // �ε��� �Ϸ�� ������ �ݺ��ؼ� ���� ��ҵ��� �ε��ϰ� ���� ������ ȭ�鿡 ǥ���Ѵ�.
        while (!ao.isDone)
        {
            // �ε� ������� �����̴� �ٿ� �ؽ�Ʈ�� ǥ���Ѵ�.
            loadingBar.value = ao.progress;
            loadingText.text = (ao.progress * 100f).ToString() + "%";

            // ����, �� �ε� ������� 90%�� �Ѿ��...
            if (ao.progress >= 0.9f)
            {
                // �ε�� ���� ȭ�鿡 ���̰� �Ѵ�.
                ao.allowSceneActivation = true;
            }

            // ���� �������� �� ������ ��ٸ���.
            yield return null;
        }
    }
}
