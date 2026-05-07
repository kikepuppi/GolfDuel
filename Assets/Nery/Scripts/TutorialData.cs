using UnityEngine;
using System;
using System.Collections.Generic;

public enum MediaType { None, Image, SpriteAnimation, Animation }

[Serializable]
public struct Tutorial {
    [TextArea(5, 10)]
    public string text;

    public MediaType mediaType;
    public Sprite image;
    public string animationTrigger;
    public Sprite[] animationFrames;
    public float frameDelay;
}

[CreateAssetMenu(fileName = "TutorialData", menuName = "Scriptable Objects/TutorialData", order = 1)]
public class TutorialData : ScriptableObject
{
    public List<Tutorial> talkScript;
}
