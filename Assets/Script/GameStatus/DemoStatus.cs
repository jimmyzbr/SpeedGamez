using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStatus : IApplicationStatus
{
    public override void OnEnterStatus()
    {
        var cfg = ConfigManager.Get<ExecelDemoConfig>();
        Debug.Log(cfg.GetStudent(19911307));
        ConfigManager.UnLoad<ExecelDemoConfig>();
        Debug.Log(cfg.GetStudent(19911307));
        
        GuideSyetem.Instance.Start();
        OpenUI<MainWindow>();
    }
}
