using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InputDecoder
{
    public static List<Character> CharacterList = new List<Character> ();
    public static bool PausedHere = false;

    public static GameObject InterfaceElements;


    //find and define the Background image
    //private static GameObject Background = GameObject.Find("Background");
    //private static Image BackgroundImage = Background.GetComponent<Image>();
    public static GameObject canvas;
    public static GameObject ImageInst;
    public static GameObject PI;
    public static GameObject PII;
    public static GameObject DialogueTextObject;
    public static GameObject NamePlateTextObject;
    public static GameObject CharImage;

    public static List<Label> labels = new List<Label>();

    public static List<string> Commands = new List<string>();

    public string inputLine;
    public static int CommandLine = 0;
    public static string LastCommand = "";

    public static void ParseInputLine(string StringToParse)
    {
        string withOutTabs = StringToParse.Replace("\t", "");
        StringToParse = withOutTabs;

        if(StringToParse.StartsWith("\"")) 
        {
            Say(StringToParse);
        }
        string[] SeparatingString = {" ", "'", "\"", "(", ")"};
        string[] args = StringToParse.Split(SeparatingString, StringSplitOptions.RemoveEmptyEntries); 
        foreach(Character character in CharacterList)
        {
            if(args[0] == character.shortName)
            {
                SplitToSay(StringToParse, character);
                if(character.sideImage!=null)
                {
                    CharImage.SetActive(true);
                    showAvatar(character.sideImage);
                }
                else
                {
                    CharImage.SetActive(false);
                }
            }
        }

        if(args[0] == "show")
        {
            showImage(StringToParse);
        }

        if(args[0] == "clrscr")
        {
            ClearScreen();
        }

        if(args[0] == "Character")
        {
            CreateNewCharacter(StringToParse);
        }

        if(args[0] == "screen")
        {
            ScreenClear(StringToParse);
        }

        if(args[0] == "jump")
        {
            jumpTo(StringToParse);
        }

        if(args[0] == "end")
        {
            end();
        }
    }

    public static void end()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    #region Say Stuff

    public static void SplitToSay(string StringToParse, Character character)
    {
        int toQuote = StringToParse.IndexOf("\"") +1;
        int endQuote = StringToParse.Length - 1;
        string StringToOutput = StringToParse.Substring(toQuote, endQuote - toQuote);
        Say(character.fullName, StringToOutput);
    } 

    public static void Say(string what)
    {
        if(!InterfaceElements.activeInHierarchy) InterfaceElements.SetActive(true);
        DialogueTextObject.GetComponent<TextMeshProUGUI>().text = what;
        PausedHere = true;
    }

    public static void Say(string who, string what)
    {
        if(!InterfaceElements.activeInHierarchy) InterfaceElements.SetActive(true);
        DialogueTextObject.GetComponent<TextMeshProUGUI>().text = what;
        NamePlateTextObject.GetComponent<TextMeshProUGUI>().text = who;
        PausedHere = true;
    }
    
    #endregion

    #region Image Stuff

    public static void showImage(string StringToParse)
    {
        string ImageToShow = null;
        bool FadeEffect = false;
        var ImageToUSe = new Regex(@"show (?<ImageFileName>[^.]+)");
        var ImageToUseTransition = new Regex(@"show (?<ImageFileName>[^.]+) with (?<TransitionName>[^.]+)");

        var matches = ImageToUSe.Match(StringToParse);
        var altMatches = ImageToUseTransition.Match(StringToParse);

        if(altMatches.Success)
        {
            ImageToShow = altMatches.Groups["ImageFileName"].ToString();
            FadeEffect = true;
        }
        else if(matches.Success)
        {
            ImageToShow = matches.Groups["ImageFileName"].ToString();
        }

        GameObject PictureInstance = GameObject.Instantiate(ImageInst);
        PictureInstance.transform.SetParent(canvas.transform, false);
        PictureInstance.GetComponent<ImageInstance>().FadeIn = FadeEffect;
        PictureInstance.GetComponent<Image>().color = Color.white;
        PictureInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + ImageToShow);

        GameObject CharInstanceI = GameObject.Instantiate(PI);
        CharInstanceI.transform.SetParent(canvas.transform, false);
        CharInstanceI.GetComponent<Image>().color = Color.white;
        CharInstanceI.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + CharacterList[CharacterList.Count-2].sideImage);

        GameObject CharInstanceII = GameObject.Instantiate(PII);
        CharInstanceII.transform.SetParent(canvas.transform, false);
        CharInstanceII.GetComponent<Image>().color = Color.white;
        CharInstanceII.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + CharacterList[CharacterList.Count-1].sideImage);
    }

    public static void showAvatar(string StringToParse)
    {
        CharImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + StringToParse);
    }

    public static void ClearScreen()
    {
        foreach(Transform t in canvas.transform)
        {               
            MonoBehaviour.Destroy(t.gameObject, 3f);
            InterfaceElements.SetActive(false);
        }
    }

    public static void ScreenClear(string StringToParse)
    {
        string ImageToShow = null;
        bool FadeEffect = false;
        var ImageToUSe = new Regex(@"screen (?<ImageFileName>[^.]+)");
        var ImageToUseTransition = new Regex(@"screen (?<ImageFileName>[^.]+) with (?<TransitionName>[^.]+)");

        var matches = ImageToUSe.Match(StringToParse);
        var altMatches = ImageToUseTransition.Match(StringToParse);

        if(altMatches.Success)
        {
            ImageToShow = altMatches.Groups["ImageFileName"].ToString();
            FadeEffect = true;
        }
        else if(matches.Success)
        {
            ImageToShow = matches.Groups["ImageFileName"].ToString();
        }

        GameObject PictureInstance = GameObject.Instantiate(ImageInst);
        PictureInstance.transform.SetParent(canvas.transform, false);
        PictureInstance.GetComponent<ImageInstance>().FadeIn = FadeEffect;
        PictureInstance.GetComponent<Image>().color = Color.white;
        PictureInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + ImageToShow);

        GameObject CharInstanceI = GameObject.Instantiate(PI);
        CharInstanceI.transform.SetParent(canvas.transform, false);
        CharInstanceI.GetComponent<Image>().color = Color.white;
        CharInstanceI.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + CharacterList[CharacterList.Count-2].sideImage);

        GameObject CharInstanceII = GameObject.Instantiate(PII);
        CharInstanceII.transform.SetParent(canvas.transform, false);
        CharInstanceII.GetComponent<Image>().color = Color.white;
        CharInstanceII.GetComponent<Image>().sprite = Resources.Load<Sprite>("images/" + CharacterList[CharacterList.Count-1].sideImage);

        foreach(Transform t in canvas.transform)
        {
            if(t != PictureInstance.transform)
            {                
                MonoBehaviour.Destroy(t.gameObject, 3f);
            }
            InterfaceElements.SetActive(false);
        }

        end();
    }

    #endregion

    #region New Character

    public static void CreateNewCharacter(string StringToParse)
    {
        string newCharShortName = null;
        string newCharFullName = null;
        Color newCharColor = Color.white;
        string newCharSideImage = null;

        var characterExpression = new Regex(@"Character\((?<shortName>[a-zA-Z0-9_]+), (?<fullName>[a-zA-Z0-9_]+), color=(?<characterColor>[a-zA-Z0-9_]+), image=(?<sideImage>[a-zA-Z0-9_]+)\)");
        var characterExpressionA = new Regex(@"Character\((?<shortName>[a-zA-Z0-9_]+), (?<fullName>[a-zA-Z0-9_]+), color=(?<characterColor>[a-zA-Z0-9_]+)\)");
        var characterExpressionB = new Regex(@"Character\((?<shortName>[a-zA-Z0-9_]+), (?<fullName>[a-zA-Z0-9_]+)\)");
        var characterExpressionC = new Regex(@"Character\((?<shortName>[a-zA-Z0-9_]+), (?<fullName>[a-zA-Z0-9_]+), image=(?<sideImage>[a-zA-Z0-9_]+)\)");

        if(characterExpression.IsMatch(StringToParse))
        {
            var matches = characterExpression.Match(StringToParse);
            newCharShortName = matches.Groups["shortName"].ToString();
            newCharFullName = matches.Groups["fullName"].ToString();
            newCharColor = Color.clear; ColorUtility.TryParseHtmlString(matches.Groups["characterColor"].ToString(), out newCharColor);
            newCharSideImage = matches.Groups["sideImage"].ToString();
        }
        else if(characterExpressionA.IsMatch(StringToParse))
        {
            var matches = characterExpressionA.Match(StringToParse);
            newCharShortName = matches.Groups["shortName"].ToString();
            newCharFullName = matches.Groups["fullName"].ToString();
            newCharColor = Color.clear; ColorUtility.TryParseHtmlString(matches.Groups["characterColor"].ToString(), out newCharColor);
        }
        else if(characterExpressionB.IsMatch(StringToParse))
        {
            var matches = characterExpressionB.Match(StringToParse);
            newCharShortName = matches.Groups["shortName"].ToString();
            newCharFullName = matches.Groups["fullName"].ToString();
        }
        else if(characterExpressionC.IsMatch(StringToParse))
        {
            var matches = characterExpressionC.Match(StringToParse);
            newCharShortName = matches.Groups["shortName"].ToString();
            newCharFullName = matches.Groups["fullName"].ToString();
            newCharSideImage = matches.Groups["sideImage"].ToString();
        }

        CharacterList.Add(new Character(newCharShortName, newCharFullName, newCharColor, newCharSideImage));
    }

    #endregion

    public static void jumpTo(string StringToParse)
    {
        var tempStringSplit = StringToParse.Split(' ');
        foreach(Label l in labels)
        {
            if(l.LabelName == tempStringSplit[1])
            {
                CommandLine = l.LabelIndex;
                PausedHere = false;
            }
        }

    }

    #region LoadingScript

    public static void readScript(string file_path)
    {
        TextAsset commandFile = Resources.Load(file_path) as TextAsset;
        var commandArray = commandFile.text.Split( '\n');
        foreach(var line in commandArray)
        {
            Commands.Add(line);
        }
        for (int x = 0; x<Commands.Count; x++)
        {
            if (Commands[x].StartsWith("label"))
            {
                var labelSlit = Commands[x].Split(' ');
                labels.Add(new Label(labelSlit[1], x));
            }
        }
    }

    #endregion

}
