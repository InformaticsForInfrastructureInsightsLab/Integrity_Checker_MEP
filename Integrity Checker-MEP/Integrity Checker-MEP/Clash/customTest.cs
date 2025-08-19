class customTest
{
    string name;
    float toler;
    public customTest(string name = "테스트 1", float toler = 0.01f)
    {
        this.toler = toler;
        this.name = name;
    }

    public string GetName() { return name; }
    public float GetToler() { return toler; }
}
