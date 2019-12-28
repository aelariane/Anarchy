public class KickState
{
    private int kickCount;
    private string kickers;
    public int id;
    public string name;

    public void addKicker(string n)
    {
        if (!this.kickers.Contains(n))
        {
            this.kickers += n;
            this.kickCount++;
        }
    }

    public int getKickCount()
    {
        return this.kickCount;
    }

    public void init(string n)
    {
        this.name = n;
        this.kickers = string.Empty;
        this.kickCount = 0;
    }
}