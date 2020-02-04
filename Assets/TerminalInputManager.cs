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
        //do all the things
    }
}
