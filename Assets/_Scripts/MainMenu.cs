﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SongChooser;
    public LoadSongInfos SongInfos;
    public GameObject PanelAreYouSure;
    public GameObject LevelChooser;
    public GameObject LevelButtonTemplate;
    public GameObject Title;
    public GameObject NoSongsFound;
    public AudioSource SongPreview;

    private SongSettings Songsettings;
    private SceneHandling SceneHandling;

    private void Awake()
    {
        Songsettings = GameObject.FindGameObjectWithTag("SongSettings").GetComponent<SongSettings>();
        SceneHandling = GameObject.FindGameObjectWithTag("SceneHandling").GetComponent<SceneHandling>();
    }

    public void ShowSongs()
    {
        if (SongInfos.AllSongs.Count == 0)
        {
            Title.gameObject.SetActive(false);
            NoSongsFound.gameObject.SetActive(true);
            return;
        }

        Songsettings.CurrentSong = SongInfos.AllSongs[SongInfos.CurrentSong];

        Title.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(true);
        var song = SongInfos.GetCurrentSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    AudioClip previewAudioClip = null;
    bool PlayNewPreview = false;

    public IEnumerator PreviewSong(string audioFilePath)
    {
        SongPreview.Stop();
        previewAudioClip = null;
        PlayNewPreview = true;
        var www = new WWW("file:///" + audioFilePath);
        while (!www.isDone)
            yield return www;

        previewAudioClip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
    }

    private void FixedUpdate()
    {
        if (previewAudioClip != null && PlayNewPreview)
        {
            PlayNewPreview = false;
            SongPreview.Stop();
            SongPreview.clip = previewAudioClip;
            SongPreview.time = 40f;
            SongPreview.Play();
        }
    }

    public void NextSong()
    {
        var song = SongInfos.NextSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void PreviousSong()
    {
        var song = SongInfos.PreviousSong();

        SongInfos.SongName.text = song.Name;
        SongInfos.Artist.text = song.AuthorName;
        SongInfos.BPM.text = song.BPM;
        SongInfos.Levels.text = song.Difficulties.Count.ToString();

        byte[] byteArray = File.ReadAllBytes(song.CoverImagePath);
        Texture2D sampleTexture = new Texture2D(2, 2);
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        if (isLoaded)
        {
            SongInfos.Cover.texture = sampleTexture;
        }

        StartCoroutine(PreviewSong(Songsettings.CurrentSong.AudioFilePath));
    }

    public void LoadSong()
    {
        SongPreview.Stop();
        var song = SongInfos.GetCurrentSong();
        if(song.Difficulties.Count > 1)
        {
            foreach (var gameObj in LevelChooser.GetComponentsInChildren<Button>(true))
            {
                if (gameObj.gameObject.name == "ButtonTemplate")
                    continue;

                Destroy(gameObj.gameObject);
            }

            SongChooser.gameObject.SetActive(false);
            PanelAreYouSure.gameObject.SetActive(false);
            LevelChooser.gameObject.SetActive(true);

            var buttonsCreated = new List<GameObject>();

            foreach (var difficulty in song.Difficulties)
            {
                var button = GameObject.Instantiate(LevelButtonTemplate, LevelChooser.transform);

                button.GetComponentInChildren<Text>().text = difficulty;
                button.GetComponentInChildren<Button>().onClick.AddListener(() => StartSceneWithDifficulty(difficulty));
                button.SetActive(true);
                buttonsCreated.Add(button);
            }

            switch (buttonsCreated.Count)
            {
                case 2:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-287, buttonsCreated[0].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(287, buttonsCreated[1].GetComponent<RectTransform>().localPosition.y);
                    break;
                case 3:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-287, buttonsCreated[0].GetComponent<RectTransform>().position.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(0, buttonsCreated[1].GetComponent<RectTransform>().position.y);
                    buttonsCreated[2].GetComponent<RectTransform>().localPosition = new Vector3(287, buttonsCreated[2].GetComponent<RectTransform>().position.y);
                    break;
                case 4:
                    buttonsCreated[0].GetComponent<RectTransform>().localPosition = new Vector3(-430, buttonsCreated[0].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[1].GetComponent<RectTransform>().localPosition = new Vector3(-144, buttonsCreated[1].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[2].GetComponent<RectTransform>().localPosition = new Vector3(144, buttonsCreated[2].GetComponent<RectTransform>().localPosition.y);
                    buttonsCreated[3].GetComponent<RectTransform>().localPosition = new Vector3(430, buttonsCreated[3].GetComponent<RectTransform>().localPosition.y);
                    break;
                default:
                    break;
            }
        }
        else
        {
            StartSceneWithDifficulty(song.Difficulties[0]);
        }
    }

    private void StartSceneWithDifficulty(string difficulty)
    {
        SongInfos.GetCurrentSong().SelectedDifficulty = difficulty;
        StartCoroutine(LoadSongScene());
    }

    private IEnumerator LoadSongScene()
    {
        yield return SceneHandling.LoadScene("OpenSaber", LoadSceneMode.Additive);
        yield return SceneHandling.UnloadScene("Menu");
    }

    public void AreYouSure()
    {
        NoSongsFound.gameObject.SetActive(false);
        Title.gameObject.SetActive(false);
        SongChooser.gameObject.SetActive(false);
        LevelChooser.gameObject.SetActive(false);
        PanelAreYouSure.gameObject.SetActive(true);
    }

    public void No()
    {
        PanelAreYouSure.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
    }

    public void Yes()
    {
        Application.Quit();
    }
}
