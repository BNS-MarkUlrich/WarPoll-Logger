using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogFilter : MonoBehaviour
{
    [SerializeField]
    private Button filterLogButton;

    [SerializeField]
    private string startPollCommand = "--poll";

    [SerializeField]
    private string logFile;

    [SerializeField]
    private List<PollEntry> pollEntries = new List<PollEntry>();

    private FileSelector fileSelector;
    private AdminTracker adminTracker;

    private bool isCheckingPoll;
    private List<string> votedUsers = new();

    private void Awake() 
    {
        adminTracker = GetComponent<AdminTracker>();
        fileSelector = GetComponent<FileSelector>();
        fileSelector.OnFileSelected.AddListener(OnFileSelected);
        filterLogButton.onClick.AddListener(ReadLogFile);
    }

    private void OnFileSelected(string path) => logFile = path;

    private void ReadLogFile()
    {
        string[] lines = System.IO.File.ReadAllLines(logFile);
        string pollEndTime = string.Empty;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            if (line.Contains("with ID:") && line.Contains("administrator rights."))
            {
                int idStart = line.IndexOf("ID:") + 3;
                int idEnd = line.IndexOf(" and");
                string idString = line[idStart..idEnd];
                int id = int.Parse(idString);

                int nameStart = line.IndexOf("- ") + 2;
                int nameEnd = line.IndexOf(" has joined");
                string name = line[nameStart..nameEnd];

                adminTracker.AddAdmin(id, name);
            }

            if (line.Contains("--endpoll") && adminTracker.IsSentByAdmin(line))
            {
                isCheckingPoll = false;
                votedUsers.Clear();
                Debug.Log($"Poll ended: {line}");
            }

            if (isCheckingPoll)
            {
                GetVoteCount(line, pollEntries[pollEntries.Count - 1]);
                continue;
            }

            if (!line.Contains(startPollCommand))
                continue;

            if (!adminTracker.IsSentByAdmin(line))
                continue;

            GetPollInfo(line, out pollEndTime);
        }

        Debug.Log("Finished reading log file.");
    }

    private void GetPollInfo(string startLine, out string pollEndTime)
    {
        Debug.Log($"Poll started: {startLine}");

        string fileDate = System.IO.File.GetLastWriteTime(logFile).ToString();
        string pollStartTime = startLine.Substring(1, 9);
        pollEndTime = AddSeconds(pollStartTime, 45);
        //Debug.Log($"Poll end time: {pollEndTime}");

        int pollStarterStart = startLine.IndexOf('[');
        int pollStarterEnd = startLine.IndexOf("--") - 1;
        string pollStarter = startLine[pollStarterStart..pollStarterEnd];

        int pollTitleStart = startLine.IndexOf(startPollCommand) + startPollCommand.Length + 1;
        int pollTitleEnd = startLine.IndexOf(".");
        string pollTitle = startLine[pollTitleStart..pollTitleEnd];

        // retrieve the poll options
        List<OptionData> options = new();
        string[] optionParts = startLine.Split(": ")[1].Split(',');
        for (int i = 0; i < optionParts.Length; i++)
        {
            string optionPart = optionParts[i];
            // int optionTextStart = optionPart.IndexOf("for");
            // int optionTextEnd = optionPart.IndexOf(',');
            //Debug.Log($"Option text: {optionPart[optionTextStart..optionTextEnd]}");
            //string optionText = $"[{i+1}] {optionPart[optionTextStart..optionTextEnd]}";
            string optionText = $"{optionPart}";
            options.Add(new OptionData(optionText, 0));
        }

        pollEntries.Add(new PollEntry(pollTitle, pollStarter, fileDate, options.ToArray(), 0));
        isCheckingPoll = true;
    }

    private void GetVoteCount(string line, PollEntry pollEntry)
    {
        if (!votedUsers.Any(user => line.Contains(user)))
        {
            for (int i = 1; i < pollEntry.Options.Length + 1; i++)
            {
                string voteLine = line.Substring(line.IndexOf("] ") + 2);

                if (voteLine.Contains(i.ToString()))
                {
                    Debug.Log($"Voted for option {i} in line {line}");
                    pollEntry.Options[i-1].AddVotes(1);
                    pollEntry.AddVotes(1);
                    votedUsers.Add(line.Substring(1, 9));
                    break;
                }
            }
        }
    }

    private string AddSeconds(string time, int seconds)
    {
        string[] timeParts = time.Split(':');
        int hours = int.Parse(timeParts[0]);
        int minutes = int.Parse(timeParts[1]);
        int newSeconds = int.Parse(timeParts[2]) + seconds;

        if (newSeconds >= 60)
        {
            newSeconds -= 60;
            minutes++;
        }

        if (minutes >= 60)
        {
            minutes -= 60;
            hours++;
        }

        return $"{hours}:{minutes}:{newSeconds}";
    }
}
