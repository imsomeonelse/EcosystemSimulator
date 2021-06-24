using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnimalManagement{
    public class MeterBar : MonoBehaviour
    {
        public Slider slider;
        public Animal owner;

        private bool stopTimer = false;
        private float currentHunger = 0;
        private float startingTime;

        private float prevTime = 0;
        private float currTime = 0;

        private void Update() 
        {
            if(slider != null)
            {
                prevTime = currTime;
                currTime = Time.time;

                if( Time.time >= startingTime + slider.maxValue )
                {
                    startingTime = Time.time;
                }

                if( currentHunger >= slider.maxValue )
                {
                    stopTimer = true;
                }

                if( !stopTimer ){
                    currentHunger += currTime - prevTime;
                    slider.value = slider.maxValue - currentHunger;
                    //slider.value = slider.maxValue - (currentHunger + time);
                }
            }
        }

        public void SetMaxValue(float maxVal)
        {
            owner = transform.parent.parent.GetComponent<Animal>();
            slider = GetComponent<Slider>();
            slider.maxValue = maxVal;
            startingTime = Time.time;
            prevTime = Time.time;
            currTime = Time.time;

        }

        public void DecreaseValue(float val)
        {
            currentHunger = Mathf.Clamp(currentHunger - val, 0, slider.maxValue);
        }

        public float GetValue()
        {
            return currentHunger;
        }
    }
}