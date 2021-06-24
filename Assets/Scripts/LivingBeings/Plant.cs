using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnimalManagement{
    public class Plant : LivingBeing
    {
        Renderer[] rs;
        float timeForNew;
        public bool isActive;
        GameObject icon;

        public void Init(Coord coord, float spawnTime)
        {
            base.Init(coord);
            isActive = true;
            rs = GetComponentsInChildren<Renderer>();
            icon = GetComponentsInChildren<Image>()[0].transform.parent.gameObject;
            timeForNew = spawnTime;

        }

        public override void Deactivate()
        {
            foreach(Renderer r in rs)
                r.enabled = false;

            icon.SetActive(false);

            isActive = false;
            StartCoroutine(Reactivate());
        }

        IEnumerator Reactivate()
        {
            yield return new WaitForSeconds(timeForNew);

            foreach(Renderer r in rs)
                r.enabled = true;

            icon.SetActive(true);

            isActive = true;
        }
        
    }
}