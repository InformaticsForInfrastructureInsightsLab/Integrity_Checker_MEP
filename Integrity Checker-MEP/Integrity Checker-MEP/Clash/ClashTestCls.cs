using Autodesk.Navisworks.Api.Clash;
using System;

[Serializable]
public class ClashTestCls
{
    public string DisplayName { get; set; }
    public ClashResultCls[] ClashResults { get; set; }
    public ClashTestType testType { get; set; }
}
