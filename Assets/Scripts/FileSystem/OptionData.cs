using System;

[Serializable]
public struct OptionData
{
    public OptionData(string optionText, int votes)
    {
        this.optionText = optionText;
        this.votes = votes;
        this.percentage = 0;
    }

    public void AddVotes(int votes) => this.votes += votes;

    public string optionText;
    public int votes;
    public int percentage;
}