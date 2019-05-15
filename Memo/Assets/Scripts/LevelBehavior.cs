﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBehavior : MonoBehaviour
{
    List<string> levels = new List<string>() { "8", "12"};
    List<string> category = new List<string>() { "owoce", "warzywa", "kroliczki"};

    public Dropdown dropdownLevels;
    public Dropdown dropdownCategories;

    public Text selectedLevel;
    public Text selectedCategory;
    public void DropdownLevel_changed(int i)
    {
        selectedLevel.text = levels[i];
    }

    public void DropdownCategory_changed(int i)
    {
        selectedCategory.text = category[i];
    }

    public void ButtonOk()
    {
        int levels = System.Convert.ToInt32(selectedLevel.text);
        PlayerPrefs.SetInt("level", levels);

        string category = selectedCategory.text;
        PlayerPrefs.SetString("category", category);

        SceneManager.LoadScene("Game");
    }

    void Start()
    {
        PopulateList();
    }

    void PopulateList()
    {
        dropdownLevels.AddOptions(levels);
        dropdownCategories.AddOptions(category);
    }
}