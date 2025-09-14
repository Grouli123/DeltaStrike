using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core.Config
{
    [CreateAssetMenu(menuName = "Game/UpgradeConfig", fileName = "UpgradeConfig")]
    public sealed class UpgradeConfig : ScriptableObject
    {
        public List<UpgradeDef> Upgrades = new();
    }

    [Serializable]
    public sealed class UpgradeDef
    {
        public StatType type;
        public string displayName = "Speed";
        public string locKey = "ui.stat.speed"; 
        public float baseValue = 5f;
        public float perPointAdd = 0.25f;
        public int maxLevel = 20;
        public string unit = ""; 
        public Sprite icon;
    }
}