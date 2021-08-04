using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoSingleton<ButtonManager>
{
    public List<GameObject> tabList = new List<GameObject>();

    private GameObject _currentTab;
    private GameObject _selectedTab;

    public enum Tab
    { 
        Home,
        Record,
        History,
        Resources
    }

    public Tab activeTab;

    public override void Init()
    {
        base.Init();

        //_currentTab = tabList[0];
    }

    public void OnTabSelected(GameObject parentTab)
    {
        _selectedTab = parentTab;

        _selectedTab.SetActive(true);

        OnTabExited();
    }

    private void OnTabExited()
    {
        if (_selectedTab == null)
        {
            _selectedTab = _currentTab;
        }

        _currentTab.SetActive(false);
    }

    public void TabClicked(int tab)
    {
        
    }
}
