using UnityEngine;
using System;
using System.Collections.Generic; 


[Serializable]
public struct Tutorial {

	[TextArea(5, 10)]
	public string text;
}

[CreateAssetMenu(fileName = "TutorialData", menuName = "Scriptable Objects/TutorialData", order = 1)]
public class TutorialData : ScriptableObject
{
    public List<Tutorial> talkScript;
}
