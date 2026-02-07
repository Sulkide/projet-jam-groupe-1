using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlackScreen : MonoBehaviour
{
	public static BlackScreen instance;
    public Image image;

    private void Awake()
    {
        instance = this;
        Fade(0f);
    }

    public static void Fade(float target)
    {
        instance.StartCoroutine(instance.FadeCoroutine(target));
    }

    IEnumerator FadeCoroutine(float target)
    {
        while (Mathf.Abs(image.color.a - target) > 0.01f)
        {
            image.color = new Color(0,0,0,Mathf.MoveTowards(image.color.a,target,0.01f));
            yield return null;
        }
    }
}
