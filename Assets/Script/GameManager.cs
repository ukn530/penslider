using UnityEngine;
using System.Collections;

public enum State{Title, Play, Over};

public class GameManager: MonoBehaviour
{
	public static State state = State.Play;
	public static int score = 0;

}