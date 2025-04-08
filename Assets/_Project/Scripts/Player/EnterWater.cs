using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AS.Player
{
    public class EnterWater : MonoBehaviour
    {
        public bool inWater = false;
        public float thresholdTime = 5f;
        public float drainRate;
        public bool drained = false;

        public GameObject waterObj;
        public Slider waterThresholdSlider;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Water")
            {
                inWater = true;
                waterObj = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Water")
            {
                inWater = false;
                waterObj = null;
            }
        }

        public void DrainWaterThreshold()
        {
            drainRate = waterThresholdSlider.maxValue / thresholdTime;

            if (waterThresholdSlider.value > 0)
            {
                waterThresholdSlider.value -= drainRate * Time.deltaTime;
            }
        }

        public void RefillWaterThreshold()
        {
            waterThresholdSlider.value += drainRate * Time.deltaTime;
        }
    }
}
