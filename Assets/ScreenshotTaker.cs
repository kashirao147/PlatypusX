using UnityEngine;
using System.IO;

public class ScreenshotTaker : MonoBehaviour
{
    // Key to take a screenshot (default is F12)
    public KeyCode screenshotKey = KeyCode.F12;

    // Base folder to save screenshots (change this path if needed)
    private string baseFolderPath = @"D:\Unity Projects Screenshots";

    void Start()
    {
        // Ensure the base folder exists
        if (!Directory.Exists(baseFolderPath))
        {
            Directory.CreateDirectory(baseFolderPath);
            Debug.Log($"Base folder created at: {baseFolderPath}");
        }

        // Create a project-specific folder
        string projectFolderPath = GetProjectFolderPath();
        if (!Directory.Exists(projectFolderPath))
        {
            Directory.CreateDirectory(projectFolderPath);
            Debug.Log($"Project folder created at: {projectFolderPath}");
        }
    }

    void Update()
    {
        // Check if the specific key is pressed
        if (Input.GetKeyDown(screenshotKey))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        // Get the project-specific folder path
        string projectFolderPath = GetProjectFolderPath();

        // Generate a unique file name with timestamp
        string fileName = $"Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";

        // Full path to save the screenshot
        string fullPath = Path.Combine(projectFolderPath, fileName);

        // Capture the screenshot
        ScreenCapture.CaptureScreenshot(fullPath);

        // Log the path for reference
        Debug.Log($"Screenshot saved to: {fullPath}");
    }

    private string GetProjectFolderPath()
    {
        // Use the current Unity project name to create a folder
        string projectName = Application.productName;
        return Path.Combine(baseFolderPath, projectName);
    }
}
