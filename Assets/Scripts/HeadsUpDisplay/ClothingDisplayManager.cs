using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothingDisplayManager : MonoBehaviour
{
    [SerializeField] private PlayerTemperature playerTemp;
    [SerializeField] private Transform tShirt;
    [SerializeField] private Transform winterCoat;
    private ClothingType currentClothing = ClothingType.None;


    void Start()
    {
        if (playerTemp == null)
        {
            Debug.LogError("[ClothingDisplayManager] PlayerTemperature script not found!");
            return;
        }
    }

    void Update()
    {
        currentClothing = playerTemp.currentClothing;
        DisplayCurrentClothing(currentClothing);
    }

    void DisplayCurrentClothing(ClothingType currentClothing){
        switch (currentClothing)
        {
            case ClothingType.None:
                break;
            case ClothingType.TShirt:
                ShowClothing(tShirt);
                break;
            case ClothingType.WinterCoat:
                ShowClothing(winterCoat);
                break;
        }
    }

    void ShowClothing(Transform clothingToShow)
    {
        tShirt.gameObject.SetActive(clothingToShow == tShirt);
        winterCoat.gameObject.SetActive(clothingToShow == winterCoat);
    }
}
