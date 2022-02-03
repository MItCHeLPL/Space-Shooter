using System.Collections;
using System.Collections.Generic;


public class Score
{
    public string name { get; set; }
    public int score { get; set; }

    public Score()
	{
        name = "";
        score = 0;
	}

    public Score(string name)
    {
        this.name = name;
        score = 0;
    }

    public Score(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
