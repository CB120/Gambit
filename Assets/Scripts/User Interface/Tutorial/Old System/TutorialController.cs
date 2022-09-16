using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [SerializeField] private GameObject slide1;
    [SerializeField] private GameObject slide2;
    [SerializeField] private GameObject slide3;
    private int slideNo = 1;
    [SerializeField] private Button prevSlide;
    [SerializeField] private Button nextSlide;

    // Update is called once per frame
    void Update()
    {
        if(slideNo == 1)
        {
            prevSlide.interactable = false;
            nextSlide.interactable = true;
        }
        else if(slideNo == 3)
        {
            nextSlide.interactable = false;
            prevSlide.interactable = true;
        }
        else
        {
            nextSlide.interactable = true;
            prevSlide.interactable = true;
        }
    }

    public void NextSlide()
    {
        slideNo++;
        ChangeSlides();
    }

    public void PrevSlide()
    {
        slideNo--;
        ChangeSlides();

    }

    public void ChangeSlides()
    {
        if(slideNo == 1)
        {
            slide1.SetActive(true);
            slide2.SetActive(false);
            slide3.SetActive(false);
        }
        else if (slideNo == 2)
        {
            slide1.SetActive(false);
            slide2.SetActive(true);
            slide3.SetActive(false);
        }
        else
        {
            slide1.SetActive(false);
            slide2.SetActive(false);
            slide3.SetActive(true);
        }
    }
}
