using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

class Emotion
{
    public string name;
    public double continousDuration;
    public double interruptionDuration;

    public Emotion(string name) {
        this.name = name;
        this.continousDuration = 0.0;
        this.interruptionDuration = 0.0;
    }
}

public class EmotionAccumulator
{
    private List<Emotion> emotions;
    private const double interruptionTolerance = 3.0;

    public EmotionAccumulator()
    {
        emotions = new List<Emotion>();
        emotions.Add(new Emotion("positive"));
        emotions.Add(new Emotion("negative"));
        emotions.Add(new Emotion("neutral"));
    }

    public void update(string emotionName, double deltaTime)
    {
        foreach (Emotion emotion in emotions)
        {
            if (emotion.name.Equals(emotionName))
            {
                emotion.continousDuration += deltaTime;
                emotion.interruptionDuration = 0.0;
            }
            else
            {
                emotion.interruptionDuration += deltaTime;
                if (emotion.interruptionDuration > interruptionTolerance)
                {
                    emotion.continousDuration = 0.0;
                }
            }
        }
    }

    public void reset(string emotionName)
    {
        foreach (Emotion emotion in emotions)
        {
            if (emotion.name.Equals(emotionName))
            {
                emotion.continousDuration = 0.0;
                emotion.interruptionDuration = 0.0;
            }
        }
    }

    public double getEmotionDuration(string emotionName)
    {
        foreach(Emotion emotion in emotions)
        {
            if(emotion.name.Equals(emotionName))
            {
                return emotion.continousDuration;
            }
        }
        return -1;
    }
}
