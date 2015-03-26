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
            get { return "Destroy!!! 1."; }
        }

        public string Name
        {
            get { return "Destroy Tool"; }
        }
    }
    public class LoadingExtension : LoadingExtensionBase
    {
        public BetterBulldozeTool bulldozeTool;

        public override void OnLevelLoaded(LoadMode mode)
        {
            GameObject gameController = GameObject.FindWithTag("GameController");
            bulldozeTool = gameController.AddComponent<BetterBulldozeTool>();
        }
    }
}
