using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkylinesBulldoze
{
    public class Mod : IUserMod
    {
        public string Description
        {
            get { return "Better bulldoze tool"; }
        }

        public string Name
        {
            get { return "Better bulldoze Tool"; }
        }
    }
    public class LoadingExtension : LoadingExtensionBase
    {
        public BetterBulldozeTool bulldozeTool;

        public override void OnLevelLoaded(LoadMode mode)
        {
            bulldozeTool = GameObject.FindObjectOfType<BetterBulldozeTool>();
            if(bulldozeTool == null)
            {
                GameObject gameController = GameObject.FindWithTag("GameController");
                bulldozeTool = gameController.AddComponent<BetterBulldozeTool>();
            }
            bulldozeTool.InitGui();
            bulldozeTool.enabled = false;
        }

    }
    
}
