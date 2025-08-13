using Autodesk.Navisworks.Api.Clash;

public class IfcSystemData
{
    public ClashResult result;
    public string ns;
    public string ifc1;
    public string ifc2;

    public IfcSystemData(ClashResult result, string ifc1, string ifc2, string ns)
    {
        this.result = result;
        this.ifc1 = ifc1;
        this.ifc2 = ifc2;
        this.ns = ns;
    }
}
