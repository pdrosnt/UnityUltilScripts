using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct statusBehaviours
{

    public int[] trigger;
    public BehaviourEvents passiveBehaviour;
    public BehaviourEvents[] triggeredBehaviour;
    public BehaviourEvents minValueBehaviour;
    public BehaviourEvents maxValueBehaviour;
    public BehaviourEvents onAddChangeBehaviour;
    public BehaviourEvents onSubChangeBehaviour;

}

[Serializable]
public struct status
{   
    public string name;
    public int passiveDecayInSeconds;
    public int maxValue;
    public int minValue;
    public int value;

    public statusBehaviours behavioursToTrigger;

}

[Serializable]
public class CharacterStats
{
    public status stat;

    public float timer = 0;

    public void ChangeValue(int x)
    {
        if(x == 0){return;}

        int sum = stat.value + x;

        if (stat.behavioursToTrigger.trigger != null)
        {

            if (stat.behavioursToTrigger.trigger.Length > 0)
            {
                for (int i = 0; i < stat.behavioursToTrigger.trigger.Length; i++)
                {
                    if (stat.value > stat.behavioursToTrigger.trigger[i] && sum > stat.behavioursToTrigger.trigger[i])
                    {
                        if (stat.behavioursToTrigger.triggeredBehaviour != null)
                        {

                            stat.behavioursToTrigger.triggeredBehaviour[i].Invoke();

                        }

                        break;
                    }

                }
            }

        }
        if (sum >= stat.maxValue)
        { 
            if(stat.value == stat.maxValue)return;
            
            stat.value = stat.maxValue;

            if (stat.behavioursToTrigger.maxValueBehaviour != null)
            {
                stat.behavioursToTrigger.maxValueBehaviour.Invoke();
            }
        }
        else if (sum <= stat.minValue)
        { 
            if(stat.value == stat.minValue)return;

            stat.value = stat.minValue;

            if (stat.behavioursToTrigger.minValueBehaviour != null)
            {
                stat.behavioursToTrigger.minValueBehaviour.Invoke();
            }

        }
        else
        { 
             if (stat.behavioursToTrigger.passiveBehaviour != null)
                {
                    stat.behavioursToTrigger.passiveBehaviour.Invoke();
                }
             
            stat.value = sum;

            if (x > 0)
            {
                if (stat.behavioursToTrigger.onAddChangeBehaviour != null)
                {
                    stat.behavioursToTrigger.onAddChangeBehaviour.Invoke();
                }
            }
            else
            {
                if (stat.behavioursToTrigger.onSubChangeBehaviour != null)
                {
                    stat.behavioursToTrigger.onSubChangeBehaviour.Invoke();
                }
            }
        }
    }
}


