using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;           // «”„ «·’Ê  („À«·: "MainMenuMusic" √Ê "PlayerWalk")
    public AudioClip clip;        // «·„ﬁÿ⁄ «·’Ê Ì ‰›”Â

    [Range(0f, 1f)]
    public float volume = 1f;     // „” ÊÏ «·’Ê 

    [Range(0.1f, 3f)]
    public float pitch = 1f;      // Õœ… «·’Ê 

    public bool loop;             // Â·  —Ìœ  ﬂ—«— «·’Ê ø („›Ìœ ··„Ê”ÌﬁÏ)
}