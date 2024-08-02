using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.UI;

public class FileSelector : MonoBehaviour
{
    [Serializable]
    public class FileSelectedEvent : UnityEvent<string> { }

    [Header("File Selection Event")]
    [SerializeField]
    private Button fileSelectButton;

    [SerializeField]
    private FileSelectedEvent onFileSelected;
    
    [SerializeField]
    private string selectedFilePath;

    public string SelectedFilePath => selectedFilePath;
    public FileSelectedEvent OnFileSelected => onFileSelected;

    private void Start()
    {
        onFileSelected ??= new FileSelectedEvent();
        fileSelectButton.onClick.AddListener(OpenFileSelector);
    }

    [ContextMenu("Open File Selector")]
    public void OpenFileSelector()
    {
        string path = EditorUtility.OpenFilePanel("Select a txt file", "", "txt");

        if (!string.IsNullOrEmpty(path))
        {
            selectedFilePath = path;
            onFileSelected.Invoke(path);
        }
    }
}
