using UnityEngine;
using UnityEngine.Video;
using System;
using System.Collections.Generic;

public enum MediaType { None, Image, Video, Animation }

[Serializable]
public struct Tutorial {
    [TextArea(5, 10)]
    public string text;

    public MediaType mediaType;
    public Sprite image;
    public VideoClip video;
    public string animationTrigger; // nome do trigger no Animator
}

[CreateAssetMenu(fileName = "TutorialData", menuName = "Scriptable Objects/TutorialData", order = 1)]
public class TutorialData : ScriptableObject
{
    public List<Tutorial> talkScript;
}
