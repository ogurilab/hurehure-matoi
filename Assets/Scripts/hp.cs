using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class hp : MonoBehaviour
{
    public int maxHp;
    private int currentHp;
    public Slider slider;

    void Start()
    {
        slider.value = (float)maxHp;
        currentHp = maxHp;
    }

    void Update()
    {

    }

    public void damage(int dmg)
    {
        currentHp = currentHp - dmg;
        slider.value = (float)currentHp / (float)maxHp;
    }
}