using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdminTracker : MonoBehaviour
{
    private Dictionary<int, string> adminIDs = new Dictionary<int, string>();

    public void AddAdmin(int id, string name) 
    {
        if (!adminIDs.ContainsKey(id))
            adminIDs.Add(id, name);
    }

    public string GetAdminName(int id) => adminIDs[id];

    public List<string> GetAdminNames() => adminIDs.Values.ToList();

    public bool IsAdmin(int id) => adminIDs.ContainsKey(id);

    public bool IsAdmin(string name) => adminIDs.ContainsValue(name);

    public bool IsSentByAdmin(string message)
    {
        foreach (string name in GetAdminNames())
        {
            if (message.Contains(name))
                return true;
        }

        return false;
    }
}
