using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SlideShow : MonoBehaviour
{
    public List<Sprite> images;
    public Image r;
    public static SlideShow instance;

    private void Start()
    {
        instance = this;
        StartCoroutine(Slideshow());
    }
    public IEnumerator Slideshow()
    {
        //Time.timeScale = 0f;
        foreach(Sprite s in images)
        {
            r.sprite = s;

            BlackScreen.Fade(0f);
            yield return new WaitForSeconds(1.5f);
            while (!(Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.IsPressed()))
            {
                yield return null;
            }
            Debug.Log("Next");
			BlackScreen.Fade(1f);

			yield return new WaitForSeconds(1.5f);

		}

        BlackScreen.Fade(0f);
        r.gameObject.SetActive(false);
        Time.timeScale = 1f;

	}
}
