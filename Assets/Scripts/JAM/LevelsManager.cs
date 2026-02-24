using UnityEngine;
using System.Collections.Generic;

public class LevelsManager : MonoBehaviour
{
	public List<LevelClass> Levels;

	public int lvlIndex;
	public int camLvlIndex;

	public static LevelsManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    public void NextLevel()
	{
		if (lvlIndex > 1)
		{
			Levels[lvlIndex - 2].gameObject.SetActive(false);
		}

		if (lvlIndex < Levels.Count-1)
		{
			Levels[lvlIndex + 1].gameObject.SetActive(true);
			if (lvlIndex < Levels.Count - 2)
			{
				Levels[lvlIndex + 2].gameObject.SetActive(true);
			}
			lvlIndex++;
		}
		else
		{
			Win();
		}
	}

	public GameObject end;
	void Win()
	{
		end.SetActive(true);
	}
}
