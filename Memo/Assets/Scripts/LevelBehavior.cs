using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelBehavior : MonoBehaviour
{
    List<string> levels = new List<string>() { "6", "12", "18", "24"};
    List<string> category = new List<string>();// { "owoce", "warzywa", "kroliczki"};

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
        LoadCategories();
        dropdownLevels.AddOptions(levels);
        dropdownCategories.AddOptions(category);
    }

    void LoadCategories()
    {
        string[] filePaths = Directory.GetDirectories("Assets/Resources/Sprites/cards/");
        foreach(string fp in filePaths)
        {
            category.Add(Path.GetFileName(fp));
        }
    }
}
