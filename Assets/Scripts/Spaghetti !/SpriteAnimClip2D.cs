using UnityEngine;

[CreateAssetMenu(fileName = "SpriteAnimClip2D", menuName = "RPG 2D/Sprite Anim Clip")]
public class SpriteAnimClip2D : ScriptableObject
{
    public enum PlaybackMode { Loop, OneShot, PingPong }

    [Header("Frames (ordre = lecture)")]
    public Sprite[] frames;

    [Header("Lecture")]
    [Min(0.1f)] public float framesPerSecond = 8f;
    public PlaybackMode playback = PlaybackMode.Loop;
    public bool randomStartFrame = false;

    [Tooltip("Pour OneShot: conserver la dernière image une fois terminé (jusqu’au prochain changement de règle)")]
    public bool holdLastOnOneShot = true;

    public bool IsValid => frames != null && frames.Length > 0;
}