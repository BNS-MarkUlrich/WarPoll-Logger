using System;
using UnityEngine;

[Serializable]
public struct PollEntry
{
    public PollEntry(string pollTitle, string pollStarter, string pollDate, OptionData[] options, int totalVotes)
    {
        this.pollTitle = pollTitle;
        this.pollStarter = pollStarter;
        this.pollDate = pollDate;
        this.options = options;
        this.totalVotes = totalVotes;
    }

    public void AddVotes(int votes) => totalVotes += votes;

    [SerializeField]
    private string pollTitle;
    [SerializeField]
    private string pollStarter;
    [SerializeField]
    private string pollDate;
    [SerializeField]
    private OptionData[] options;
    [SerializeField]
    private int totalVotes;

    public string PollTitle => pollTitle;
    public string PollStarter => pollStarter;
    public string PollDate => pollDate;
    public OptionData[] Options => options;
    public int TotalVotes => totalVotes;
}
