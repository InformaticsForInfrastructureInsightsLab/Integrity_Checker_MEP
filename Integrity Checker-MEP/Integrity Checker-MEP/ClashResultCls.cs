using Autodesk.Navisworks.Api;
using System;

[Serializable]
public class ClashResultCls
{
    public string DisplayName { get; set; }
    public string Status { get; set; }
    public string Namespace1 { get; set; }
    public string Namespace2 { get; set; }
    public string path1ID { get; set; }
    public string path2ID { get; set; }
    public double distance { get; set; }
    public double[] CenterPt { get; set; }
    public ModelItem Item1 { get; set; }
    public ModelItem Item2 { get; set; }
    public string SourceFile { get; set; }
    public string IfcSystem1 { get; set; }
    public string IfcSystem2 { get; set; }
}
