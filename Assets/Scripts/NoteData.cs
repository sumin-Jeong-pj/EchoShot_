using System;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType //노트 종류 0: Tap, 1: HoldStart, 2: HoldEnd, 3: Heal, 4: Bomb
{
    Tap,
    HoldStart,
    HoldEnd,
    Heal,
    Bomb
}

[Serializable]
public class NoteJsonData //노트 데이터 구조
{
    public int bar;           // 마디
    public float beat;        // 박자
    public NoteType type;     // 노트 종류
    public float angleDeg;    // -1: 랜덤
    public float holdBeats;   // HoldStart일 때만 의미 있음 (0이면 무시)
}

[Serializable]
public class ChartJsonData //채보 데이터 구조
{
    public string songId;
    public string title;
    public string audioClipName; 

    public float bpm;
    public int beatsPerBar; //4/4박자:4, 3/4박자:3
    public float offsetSeconds; //곡 시작 전 딜레이 시간
    public float noteSpeed; //노트 속도

    public List<NoteJsonData> notes;
}
