using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum DifficultyState : int
{
    FUN =1,
    TRICKY =2,
    TAXING =3,
    MAYHEM =4,
    ARCHMASTER =5
}

public enum GameState
{
    STOP,
    PLAY,    
    HISCORE
}

public class hiscore : IComparable<hiscore>
{
    public DifficultyState diff;
    public float score;
    public string name;

    public hiscore()
    {

    }

    public hiscore(DifficultyState d, float s, string n)
    {
        diff = d;
        score = s;
        name = n;
    }

    public string serialize()
    {
        return ((int)diff).ToString() + ";" + score.ToString() + ";" + name;
    }

    public static hiscore deserialize(string s)
    {
        string[] parts = s.Split(";".ToCharArray());
        hiscore hs = new hiscore();
        hs.diff = (DifficultyState)Int32.Parse(parts[0]);
        hs.score = float.Parse(parts[1]);
        hs.name = parts[2];
        return hs;
    }

    public int CompareTo(hiscore other)
    {
        return other.score.CompareTo(this.score);
    }
}

public class Game : MonoBehaviour {

    public static string[] really_cool_words = { "Bodacious!", "Wicked!", "X-treme!!", "Excellent!", "Groovy!", "Right-on!", "Cool", "Radical!" };    

    public float updatescorecountdown = 0.1f;
    public float testgameovercountdown = 1f;
    public bool hangtime = false;

    public GUIStyle ScoreTableStyle;
    public GUIStyle ScoreStyle;
    public GUIStyle CoolWordsStyle;

    public float Score = 0;
    public float TotalTime = 0;
    public float TotalSnow = 0;

    public Dictionary<DifficultyState, List<hiscore>> scoretables;

    public GameState State = GameState.STOP;
    public DifficultyState Difficulty = DifficultyState.FUN;

    public Texture2D TitleTexture;

    public int NumGates;
    public float GateMargin = 30;

    public Transform GatePrefab;
    public Slope CurrentSlope;
    public PlayerBehaviour ThePlayer;

    public float SlopeWidth = 100;
    public float SlopeLength = 3000;

    public List<Gate> CurrentGates;

    public int GatesHit;
    public int GatesMissed;

    private static Game instance;
    public static Game Instance
    {
        get
        {
            return instance;
        }
    }

    public static System.Random randy = new System.Random();

    public static string GetCoolWord()
    {
        int i = randy.Next(0, really_cool_words.Length);
        return really_cool_words[i];
    }

    public Game()
    {
        instance = this;        
    }

	// Use this for initialization
	void Start ()
    {
        ResetGame();
        scoretables = new Dictionary<DifficultyState, List<hiscore>>();
        scoretables.Add(DifficultyState.FUN, new List<hiscore>());
        scoretables.Add(DifficultyState.TRICKY, new List<hiscore>());
        scoretables.Add(DifficultyState.TAXING, new List<hiscore>());
        scoretables.Add(DifficultyState.MAYHEM, new List<hiscore>());
        scoretables.Add(DifficultyState.ARCHMASTER, new List<hiscore>());

        if (PlayerPrefs.HasKey("ScoresSaved"))
        {

        }
        else
        {
            //make up new scores
            List<hiscore> funlist = scoretables[DifficultyState.FUN];
            funlist.Add(new hiscore(DifficultyState.FUN, 100, "Rudolf"));
            funlist.Add(new hiscore(DifficultyState.FUN, 200, "Hedgehog"));
            funlist.Add(new hiscore(DifficultyState.FUN, 300, "Polarbear"));
            funlist.Add(new hiscore(DifficultyState.FUN, 400, "Sabres"));
            funlist.Add(new hiscore(DifficultyState.FUN, 500, "Buffles"));

            List<hiscore> trickylist = scoretables[DifficultyState.TRICKY];
            trickylist.Add(new hiscore(DifficultyState.TRICKY, 100, "Pepe"));
            trickylist.Add(new hiscore(DifficultyState.TRICKY, 200, "Santa"));
            trickylist.Add(new hiscore(DifficultyState.TRICKY, 300, "Polarbear"));
            trickylist.Add(new hiscore(DifficultyState.TRICKY, 400, "Sabres"));
            trickylist.Add(new hiscore(DifficultyState.TRICKY, 500, "Buffles"));

            List<hiscore> taxlist = scoretables[DifficultyState.TAXING];
            taxlist.Add(new hiscore(DifficultyState.TAXING, 60, "Rudolf"));
            taxlist.Add(new hiscore(DifficultyState.TAXING, 100, "Hedgehog"));
            taxlist.Add(new hiscore(DifficultyState.TAXING, 200, "Polarbear"));
            taxlist.Add(new hiscore(DifficultyState.TAXING, 300, "Sabres"));
            taxlist.Add(new hiscore(DifficultyState.TAXING, 400, "Buffles"));

            List<hiscore> maylist = scoretables[DifficultyState.MAYHEM];
            maylist.Add(new hiscore(DifficultyState.MAYHEM, 50, "Rudolf"));
            maylist.Add(new hiscore(DifficultyState.MAYHEM, 120, "Buffles"));
            maylist.Add(new hiscore(DifficultyState.MAYHEM, 200, "Polarbear"));
            maylist.Add(new hiscore(DifficultyState.MAYHEM, 420, "Sabres"));
            maylist.Add(new hiscore(DifficultyState.MAYHEM, 450, "Pepe"));

            List<hiscore> archlist = scoretables[DifficultyState.ARCHMASTER];
            archlist.Add(new hiscore(DifficultyState.ARCHMASTER, 50, "Pepe"));
            archlist.Add(new hiscore(DifficultyState.ARCHMASTER, 70, "Buffles"));
            archlist.Add(new hiscore(DifficultyState.ARCHMASTER, 140, "Sabres"));
            archlist.Add(new hiscore(DifficultyState.ARCHMASTER, 200, "Polarbear"));
            archlist.Add(new hiscore(DifficultyState.ARCHMASTER, 1000, "Owlik"));

            scoretables[DifficultyState.FUN] = funlist;
            scoretables[DifficultyState.TRICKY] = trickylist;
            scoretables[DifficultyState.TAXING] = taxlist;
            scoretables[DifficultyState.MAYHEM] = maylist;
            scoretables[DifficultyState.ARCHMASTER] = archlist;

            Debug.Log("ADDED DUMMY SCORES");
        }
    }
	
    public void ResetGame()
    {
        Score = 0;
        GatesHit = 0;
        GatesMissed = 0;
        TotalTime = 0;
        ThePlayer.AccumulatedSnow = 0;


        ThePlayer.transform.position = new Vector3(0, 0, 10);
        Camera.main.transform.position = new Vector3(-1, 0, 14);
        Camera.main.transform.LookAt(ThePlayer.transform);
        ThePlayer.GetComponent<Rigidbody>().useGravity = false;

        if(CurrentGates.Count>0)
        {
            foreach(Gate g in CurrentGates)
            {
                GameObject.Destroy(g.gameObject);
            }
        }

        //make gates
        float increment = (SlopeLength * 0.8f) / (float)NumGates;
        float z = increment;
        float x;
        float y;
        CurrentGates = new List<Gate>();

        for (int i = 0; i < NumGates; i++)
        {
            y = 0;
            x = 0;

            Transform t = (Transform)Instantiate(GatePrefab);
            Gate g = t.gameObject.GetComponent<Gate>();

            g.ThePlayer = ThePlayer;

            if (i % 2 == 0)
                x = (SlopeWidth / 2) - GateMargin;
            else
                x = -(SlopeWidth / 2) + GateMargin;

            g.transform.position = new Vector3(x, y, z);

            //find the correct y for the gate
            Ray r = new Ray(g.transform.position, Vector3.down);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(r, out hit, 10000))
            {
                y = hit.point.y;
                Debug.Log("Gate " + i.ToString() + " collider hit at " + hit.point.ToString());
            }
            else
            {
                Debug.LogWarning("Gate " + i.ToString() + " collider did not hit");
            }

            g.transform.position = new Vector3(x, y, z);

            CurrentGates.Add(g);

            z += increment;
        }
    }

	// Update is called once per frame
	void Update ()
    {
	    if(State == GameState.PLAY)
        {
            TotalTime += Time.deltaTime;

            //don't do this too often because it's a cheesy way of doing it!
            testgameovercountdown -= Time.deltaTime;
            if (testgameovercountdown < 0)
            {
                if (hangtime)
                {
                    Ray r = new Ray(ThePlayer.transform.position, Vector3.down);
                    RaycastHit hit = new RaycastHit();
                    if (!Physics.Raycast(r, out hit, 10000))
                    {
                        //does the player have a high score?
                        List<hiscore> scores = scoretables[Difficulty];
                        foreach (hiscore hs in scores)
                        {
                            if (Score > hs.score)
                            {
                                State = GameState.HISCORE;
                            }
                        }

                        if (State == GameState.PLAY)
                        {
                            // :(
                            Stop();
                        }
                    }
                }
                else
                {
                    hangtime = true;
                }
                testgameovercountdown = 2f;
            }
        }
	}

    public void SetCourse()
    {
        if (Difficulty == DifficultyState.FUN)
        {            
            SetupBumpers(true, false);
            SetupRamps(false, false);
        }
        if (Difficulty == DifficultyState.TRICKY)
        {            
            SetupBumpers(true, false);
            SetupRamps(false, true);
        }
        if (Difficulty == DifficultyState.TAXING)
        {
            SetupBumpers(false, true);
            SetupRamps(true, true);
        }
        if (Difficulty == DifficultyState.MAYHEM)
        {
            SetupBumpers(false, false);
            SetupRamps(true, true);
        }
        if (Difficulty == DifficultyState.ARCHMASTER)
        {
            SetupBumpers(false, false);
            SetupRamps(true, true);
        }
    }

    private List<GameObject> allRamps = new List<GameObject>();
    public void SetupRamps(bool allon, bool first8)
    {
        if (allRamps.Count > 0)
        {
            foreach (GameObject ramp in allRamps)
            {
                ramp.GetComponent<Slope>().resetPosition();
                if (ramp.name == "ramp1") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp2") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp3") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp4") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp5") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp6") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp7") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp8") ramp.SetActive(allon || first8);
                if (ramp.name == "ramp9") ramp.SetActive(allon);
                if (ramp.name == "ramp10") ramp.SetActive(allon);
                if (ramp.name == "ramp11") ramp.SetActive(allon);
                if (ramp.name == "ramp12") ramp.SetActive(allon);
                if (ramp.name == "ramp13") ramp.SetActive(allon);
                if (ramp.name == "ramp14") ramp.SetActive(allon);
                if (ramp.name == "ramp15") ramp.SetActive(allon);
                if (ramp.name == "ramp16") ramp.SetActive(allon);
                if (ramp.name == "ramp17") ramp.SetActive(allon);
                if (ramp.name == "ramp18") ramp.SetActive(allon);
                if (ramp.name == "ramp19") ramp.SetActive(allon);
                if (ramp.name == "ramp20") ramp.SetActive(allon);
            }
        }
        else
        {
            GameObject o = GameObject.Find("ramp1"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp2"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp3"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp4"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp5"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp6"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp7"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp8"); allRamps.Add(o); o.SetActive(allon || first8);
            o = GameObject.Find("ramp9"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp10"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp11"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp12"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp13"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp14"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp15"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp16"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp17"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp18"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp19"); allRamps.Add(o); o.SetActive(allon);
            o = GameObject.Find("ramp20"); allRamps.Add(o); o.SetActive(allon);
        }
    }

    private List<GameObject> allBumps = new List<GameObject>();
    public void SetupBumpers(bool allon, bool evensoff)
    {
        if (allBumps.Count > 0)
        {
            foreach (GameObject bump in allBumps)
            {
                if (bump.name == "bump1.1") bump.SetActive(allon || evensoff);
                if (bump.name == "bump1.2") bump.SetActive(allon || evensoff);

                if (bump.name == "bump2.1") bump.SetActive(allon );
                if (bump.name == "bump2.2") bump.SetActive(allon );

                if (bump.name == "bump3.1") bump.SetActive(allon || evensoff);
                if (bump.name == "bump3.2") bump.SetActive(allon || evensoff);

                if (bump.name == "bump4.1") bump.SetActive(allon );
                if (bump.name == "bump4.2") bump.SetActive(allon );

                if (bump.name == "bump5.1") bump.SetActive(allon || evensoff);
                if (bump.name == "bump5.2") bump.SetActive(allon || evensoff);

                if (bump.name == "bump6.1") bump.SetActive(allon );
                if (bump.name == "bump6.2") bump.SetActive(allon );

                if (bump.name == "bump7.1") bump.SetActive(allon || evensoff);
                if (bump.name == "bump7.2") bump.SetActive(allon || evensoff);

                if (bump.name == "bump8.1") bump.SetActive(allon );
                if (bump.name == "bump8.2") bump.SetActive(allon );

                if (bump.name == "bump9.1") bump.SetActive(allon || evensoff);
                if (bump.name == "bump9.2") bump.SetActive(allon || evensoff);

                if (bump.name == "bump10.1") bump.SetActive(allon );
                if (bump.name == "bump10.2") bump.SetActive(allon );
            }
        }
        else
        {
            GameObject o = GameObject.Find("bump1.1"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump1.2"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump2.1"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump2.2"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump3.1"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump3.2"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump4.1"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump4.2"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump5.1"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump5.2"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump6.1"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump6.2"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump7.1"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump7.2"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump8.1"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump8.2"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump9.1"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump9.2"); allBumps.Add(o); o.SetActive(allon || evensoff);
            o = GameObject.Find("bump10.1"); allBumps.Add(o); o.SetActive(allon);
            o = GameObject.Find("bump10.2"); allBumps.Add(o); o.SetActive(allon);
        }
    }

    public void OnGUI()
    {
        float buttonwidth = 160;
        float buttonheight = 80;
        float buttonMargin = 64;        
        float buttonpadding = 16;
        float buttonY = Screen.height / 2 + buttonwidth + buttonpadding;

        //title
        if (State == GameState.STOP)
            GUI.DrawTexture(new Rect(0, 0, TitleTexture.width * 1.5f, TitleTexture.height * 1.5f), TitleTexture);
        else
            GUI.DrawTexture(new Rect(0, 0, TitleTexture.width, TitleTexture.height), TitleTexture);

        //menu bits
        if (State == GameState.STOP)
        {
            if (GUI.Button(new Rect(buttonMargin, buttonY, buttonwidth, buttonheight), "Play"))
            {
                State = GameState.PLAY;
                ThePlayer.GetComponent<Rigidbody>().useGravity = true;
                ThePlayer.GetComponent<Rigidbody>().WakeUp();
                Camera.main.transform.position = new Vector3(0,20,0);
                Camera.main.transform.LookAt(new Vector3(0, 20, 1));
                TotalTime = 0;
                Score = 0;
                GatesHit = 0;
                GatesMissed = 0;

                SetCourse();
            }
            buttonY += buttonheight + buttonpadding;
            if(GUI.Button(new Rect(buttonMargin, buttonY, buttonwidth,buttonheight),"Difficulty: "+ DifficultyToString(Difficulty)))
            {
                Difficulty++;
                if ((int)Difficulty == 6) Difficulty = DifficultyState.FUN;                
            }

            //sort the highscores for this difficulty and show 1-5
            List<hiscore> scores = scoretables[Difficulty];
            scores.Sort();

            float scoreWidth = 384;
            float scoreY = Screen.height / 2 + 60;
            float scoreX = Screen.width - scoreWidth;
            float scoreHeight = 40;

            GUI.Label(new Rect(scoreX, scoreY, scoreWidth, scoreHeight), "X-tremists", ScoreStyle);
            scoreY += scoreHeight + 65;
            float scoretop = scoreY;
            for(int i = 0; i < 5; i++)
            {
                if (scores != null && scores[i] != null)
                {
                    GUI.Label(new Rect(scoreX, scoreY, scoreWidth, scoreHeight), scores[i].name, ScoreStyle);
                    scoreY += scoreHeight;
                }
            }
            scoreY = scoretop;
            for (int i = 0; i < 5; i++)
            {
                if (scores != null && scores[i] != null)
                {
                    GUI.Label(new Rect(scoreX + 240, scoreY, scoreWidth, scoreHeight), ((int)scores[i].score).ToString(), ScoreStyle);
                    scoreY += scoreHeight;
                }
            }
        }

        //score
        if (State == GameState.PLAY)
        {
            //Don't update this too often because of concatenated strings!!
            updatescorecountdown -= Time.deltaTime;
            if (updatescorecountdown < 0)
            {
                string timestring = getTimestring(TotalTime);
                showtime = "Time: " + timestring;
                showhit = "Hits: " + GatesHit.ToString();
                showmiss = "Misses: " + GatesMissed.ToString();
                showscore = "Score: " + ((int)Score).ToString();
                updatescorecountdown = 0.1f;
            }

            GUI.Label(new Rect(Screen.width - 240, 20, 240, 50), showtime, ScoreStyle);
            GUI.Label(new Rect(Screen.width - 240, 65, 240, 50), showhit, ScoreStyle);
            GUI.Label(new Rect(Screen.width - 240, 110, 240, 50), showmiss, ScoreStyle);
            GUI.Label(new Rect(Screen.width - 240, 155, 240, 50), showscore, ScoreStyle);            
        }

        if(State == GameState.HISCORE)
        {
            GUI.Label(new Rect(Screen.width / 2 - 300, Screen.height / 2, 240, 50), "High Score! enter your name, X-tremist", ScoreStyle);
            GivenName = GUI.TextField(new Rect(Screen.width / 2 - 160, Screen.height / 2 + 60, 240, 50), GivenName);
            if (GUI.Button(new Rect(Screen.width / 2 - 160, Screen.height / 2 +60 + 60, 240, 100), "Yay!"))
            {
                if (GivenName.Length < 1)
                    GivenName = "NonnyMouse";
                scoretables[Difficulty].Add(new hiscore(Difficulty, Score, GivenName));
                Stop();
            }
        }
    }

    private string GivenName = "";

    private void Stop()
    {
        State = GameState.STOP;
        ThePlayer.GetComponent<Rigidbody>().useGravity = false;
        ResetGame();
    }

    public string showtime = "";
    public string showhit = "";
    public string showmiss = "";
    public string showscore = "";

    public static string getTimestring(float t)
    {
        int mins = Mathf.FloorToInt(t) / 60;
        float secs = t % 60;
        string secstring = secs.ToString();
        if (secstring.Length > (int)secs/10 + 3)
        {
            secstring = secstring.Substring(0, (int)secs/10 +3);
        }

        string timestring = "";

        if (mins == 0) timestring += "00:";
        else if (mins < 10) timestring += "0" + mins.ToString() + ":";
        else timestring += mins.ToString() + ":";

        if (secs < 10) timestring += "0" + secstring;
        else timestring += secstring;

        return timestring;
    }

    public static string DifficultyToString(DifficultyState diff)
    {
        if (diff == DifficultyState.FUN)
            return "FUN";
        if (diff == DifficultyState.TRICKY)
            return "TRICKY";
        if (diff == DifficultyState.TAXING)
            return "TAXING";
        if (diff == DifficultyState.MAYHEM)
            return "MAYHEM";
        if (diff == DifficultyState.ARCHMASTER)
            return "ARCHMASTER";

        return "QUE?";
    }
}
