using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalInputManager : MonoBehaviour
{
    public TMPro.TMP_InputField inputField;
    public MakeTerminalLines terminalLinesMaker;

    private int upLine = -1;
    private List<string> history = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        inputField.onSubmit.AddListener(OnInputEnter);
        inputField.onDeselect.AddListener(OnInputDeselect);
        inputField.ActivateInputField();
    }

    void Update()
    {
        bool up = Input.GetKeyDown(KeyCode.UpArrow);
        if (up)
        {
            upLine += 1;
            upLine = Mathf.Min(history.Count-1, upLine);
        }
        bool down = Input.GetKeyDown(KeyCode.DownArrow);
        if (down)
        {
            upLine -= 1;
            upLine = Mathf.Max(0, upLine);
        }
        if (up || down)
        {
            inputField.text = history[upLine];
        }
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
        upLine = 0; 
    }

    private void ProcessInput(string input)
    {
        history.Insert(0, input);
        string[] inputWords = input.Split(' ');
        string command = inputWords[0];
        string response = "Nothing to do";
        if (command.Equals("load-hull"))
        {
            response = LoadCustomSprite(inputWords[1]);
        }
        if (command.Equals("dummy") || command.Equals("build"))
        {
            List<VesselPart> parts = new List<VesselPart>();
            for (int i = 0; i < inputWords.Length; i++)
            {
                string word = inputWords[i];
                bool bay = word.Equals("-bay");
                bool engine = word.Equals("-engine");
                if (bay || engine)
                {
                    float newSize = float.Parse(inputWords[i + 1]);
                    float newQuality1 = float.Parse(inputWords[i + 2]);
                    float newQuality2 = float.Parse(inputWords[i + 3]);
                    float xPos = float.Parse(inputWords[i + 4]);
                    float yPos = float.Parse(inputWords[i + 5]);
                    float newFacing = float.Parse(inputWords[i + 6]);

                    VesselPartType newPartType;
                    if (bay)
                        newPartType = VesselPartType.Bay;
                    else
                        newPartType = VesselPartType.Engine;

                    VesselPart newPart = new VesselPart()
                    {
                        partType = newPartType,
                        position = new Vector2(xPos, yPos),
                        facing = newFacing,
                        size = newSize,
                        quality1 = newQuality1,
                        quality2 = newQuality2
                    };
                    parts.Add(newPart);
                }
            }
            List<Vessel> sourceVessels;
            string spriteName;
            float size;
            float durability;
            string designation;
            
            bool dummy = (command.Equals("dummy")) ;
            if (dummy)
            {
                sourceVessels = new List<Vessel>();
                spriteName = inputWords[1];
                size = 1f;
                durability = 1f;
                designation = "king";
            }
            else
            {
                sourceVessels = PlayerManager.Instance().GetSelection();
                spriteName = inputWords[1];
                size = float.Parse(inputWords[2]);
                durability = float.Parse(inputWords[3]);
                designation = inputWords[4];
            }
            response = VesselManager.Instance().BuildVessel(!dummy, sourceVessels, spriteName, size, durability, designation, parts);
        }
        if (command.Equals("select") || command.Equals("s"))
        {
            List<string> designations = new List<string>(inputWords);
            designations.RemoveAt(0);
            PlayerManager.Instance().Select(designations);
            response = "Selected unique designations: " + designations.Count;
        }
        if (command.Equals("ignite") || command.Equals("quench"))
        {
            bool on = command.Equals("ignite");
            int[] indexes = new int[inputWords.Length - 1];
            for (int i = 1; i < inputWords.Length; i++)
            {
                indexes[i - 1] = int.Parse(inputWords[i]);
            }
            foreach (Vessel selection in PlayerManager.Instance().GetSelection())
            {
                if (on)
                    selection.IgniteEngines(indexes);
                else
                    selection.QuenchEngines(indexes);
            }
            response = command + ": " + indexes.Length + " engines.";
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
