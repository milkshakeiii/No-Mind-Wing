using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalInputManager : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public MakeTerminalLines terminalLinesMaker;

    // Start is called before the first frame update
    void Start()
    {
        inputField.onSubmit.AddListener(OnInputEnter);
        inputField.onDeselect.AddListener(OnInputDeselect);
        inputField.ActivateInputField();
    }

    private void OnInputDeselect(string text)
    {
        inputField.ActivateInputField();
    }

    private void OnInputEnter(string submitted)
    {
        terminalLinesMaker.PushLine(submitted);
        inputField.text = "";
        inputField.ActivateInputField();
        ProcessInput(submitted);
    }

    private void ProcessInput(string input)
    {
        string[] inputWords = input.Split(' ');
        string command = inputWords[0];
        string response = "";
        if (command.Equals("load-hull"))
        {
            response = LoadCustomSprite(inputWords[1]);
        }
        if (command.Equals("dummy"))
        {
            List<VesselPart> parts = new List<VesselPart>();
            for (int i = 0; i < inputWords.Length; i++)
            {
                string word = inputWords[i];
                if (word.Equals("-bay"))
                {
                    float newSize = float.Parse(inputWords[i + 1]);
                    float newQuality1 = float.Parse(inputWords[i + 2]);
                    float newQuality2 = float.Parse(inputWords[i + 3]);
                    float xPos = float.Parse(inputWords[i + 4]);
                    float yPos = float.Parse(inputWords[i + 5]);

                    VesselPart newPart = new VesselPart()
                    {
                        partType = VesselPartType.Bay,
                        position = new Vector2(xPos, yPos),
                        size = newSize,
                        quality1 = newQuality1,
                        quality2 = newQuality2
                    };
                    parts.Add(newPart);
                }
            }
            response = VesselManager.Instance().BuildVessel(inputWords[1], 1f, 1f, "king", parts);
        }
        if (command.Equals("select") || command.Equals("s"))
        {
            List<string> designations = new List<string>(inputWords);
            designations.RemoveAt(0);
            PlayerManager.Instance().Select(designations);
            response = "Selected unique designations: " + designations.Count;
        }
        terminalLinesMaker.PushLine(response);
    }

    private string LoadCustomSprite(string path)
    {
        if (!System.IO.File.Exists(path))
            return "File " + path + " not found.";
        string fileName = System.IO.Path.GetFileName(path);
        string destinationFolder = System.IO.Path.Combine(Application.persistentDataPath, "Sprites");
        string destinationFilePath = System.IO.Path.Combine(destinationFolder, fileName);
        if (System.IO.File.Exists(destinationFilePath))
            System.IO.File.Delete(destinationFilePath);
        if (!System.IO.Directory.Exists(destinationFolder))
            System.IO.Directory.CreateDirectory(destinationFolder);
        System.IO.File.Copy(path, destinationFilePath);
        SpriteManager.Instance().LoadSprites();
        return "Hull loaded. Name: " + fileName;
    }
}
