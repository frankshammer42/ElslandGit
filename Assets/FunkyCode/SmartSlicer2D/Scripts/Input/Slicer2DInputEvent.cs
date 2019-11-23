using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DInputEvent {
	public enum EventType {None, Press, Release, Move, SetPosition};

	public EventType eventType = EventType.None;
	public Vector2 position;
	public float time;
}