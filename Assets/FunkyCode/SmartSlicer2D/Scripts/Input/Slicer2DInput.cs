using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DInput {
	public bool visualsEnabled = true;
	public bool slicingEnabled = true;
	
	public bool released = false;
	public bool pressed = false;
	public bool holding = false;
	public bool clicked = false;
	public Vector2D position = Vector2D.Zero();

	public bool playing = false;
	public bool loop = false;

	public bool rawInput = true;

	public List<Slicer2DInputEvent> eventsPlaying = new List<Slicer2DInputEvent>();
	public List<Slicer2DInputEvent> eventsBank = new List<Slicer2DInputEvent>();
	public Slicer2DInputEvent currentEvent;
		
	public TimerHelper timer = null;
	
	// Event Handling
	public delegate void InputCompleted();
	public event InputCompleted controllerEvents;

	public void EventsCompleted() {
		if (controllerEvents != null) {
			controllerEvents();
		}
	}
}