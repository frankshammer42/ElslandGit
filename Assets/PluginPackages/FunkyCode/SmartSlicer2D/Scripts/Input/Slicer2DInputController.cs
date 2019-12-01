using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Slicer2DInputController {
	public Slicer2DInput[] input = new Slicer2DInput[10];

	public bool multiTouch = true;

	private static bool useTouch = false;

	public void AddCompletedEvents(Slicer2DInput.InputCompleted controllerEvent, int id = 0) {
		input[id].controllerEvents += controllerEvent;
	}

	public bool GetVisualsEnabled(int id = 0) {
		return(input[id].visualsEnabled);
	}

	public void SetVisualsState(bool state, int id = 0) {
		input[id].visualsEnabled = state;
	}

	public bool GetSlicingEnabled(int id = 0) {
		return(input[id].slicingEnabled);
	}

	public void SetSlicingState(bool state, int id = 0) {
		input[id].slicingEnabled = state;
	}
	
	public Slicer2DInputController() {
		multiTouch = true;

		for(int id = 0; id < 10; id++) {
			input[id] = new Slicer2DInput();
		}
	}

	///// Get Functions /////
	public Vector2D GetInputPosition(int id = 0) {
		return(input[id].position);
	}

	public bool GetInputClicked(int id = 0) {
		return(input[id].clicked);
	}

	public bool GetInputPressed(int id = 0) {
		return(input[id].pressed);
	}

	public bool GetInputHolding(int id = 0) {
		return(input[id].holding);
	}

	public bool GetInputReleased(int id = 0) {
		return(input[id].released);
	}

	public void ClearActions(int id = 0) {
		input[id].eventsBank.Clear();
	}

	public bool Playing(int id = 0) {
		return(input[id].playing);
	}

	///// Event Actions /////
	public void SetMouse(Vector2 position, float time, int id = 0) {
		Slicer2DInputEvent e = new Slicer2DInputEvent();
		e.eventType = Slicer2DInputEvent.EventType.SetPosition;
		e.position = position;
		e.time = time;

		input[id].eventsBank.Add(e);
	}

	public void MoveMouse(Vector2 position, float time, int id = 0) {
		Slicer2DInputEvent e = new Slicer2DInputEvent();
		e.eventType = Slicer2DInputEvent.EventType.Move;
		e.position = position;
		e.time = time;

		input[id].eventsBank.Add(e);
	}

	public void PressMouse(float time, int id = 0) {
		Slicer2DInputEvent e = new Slicer2DInputEvent();
		e.eventType = Slicer2DInputEvent.EventType.Press;
		e.time = time;

		input[id].eventsBank.Add(e);
	}

	public void ReleaseMouse(float time, int id = 0) {
		Slicer2DInputEvent e = new Slicer2DInputEvent();
		e.eventType = Slicer2DInputEvent.EventType.Release;
		e.time = time;

		input[id].eventsBank.Add(e);
	}

	public void OnGUI() {
		//GUI.Label(new Rect(0, Screen.height - 30, 100, 100), "Touch Count: " + Input.touchCount);
		//for(int i = 0; i < Input.touchCount; i++) {
		//	Touch touch = Input.GetTouch(i);
		//	GUI.Label(new Rect(0, Screen.height - 60 - i * 30, 300, 100), "Pos: " + touch.position + " " + touch.phase);
		//}

		//for(int i = 0; i < 5; i++) {
		//	GUI.Label(new Rect(0, Screen.height - 60 - i * 30, 300, 100), "Pos: " + GetInputPosition(i).ToVector2());
		//}
	}

	///// Default Input Update /////
	public void Update() {
		if (multiTouch) {
			if (Input.touchCount > 0) {
				useTouch = true;
			}
		}

		int inputCount = 10;

		if (multiTouch == false) {
			inputCount = 1;
		}
		
		for(int id = 0; id < inputCount; id++) {
			input[id].clicked = false;
			input[id].holding = input[id].pressed;

			if (input[id].rawInput) {
				int touchCount = Input.touchCount;
				
				if (id < touchCount) {
					
					bool down = input[id].pressed;
					Touch touch = Input.GetTouch(id);
					input[id].clicked = (down == false);
					input[id].pressed = (touch.phase == TouchPhase.Moved) || (touch.phase == TouchPhase.Stationary);

					if (input[id].pressed) {
						input[id].position = GetTouchPosition(touch.position);
					}

					if (down && input[id].pressed == false) {
						input[id].released = true;
					} else {
						input[id].released = false;
					}
					
				} else {
					if (useTouch == false) {
						switch(id) {
							case 0:
								bool down = input[id].pressed;
								bool getMouseButton = Input.GetMouseButton(0);

								if (getMouseButton == true) {
									if (down == false) {
										input[id].clicked = true;
									}
								}
								
								//input[id].clicked = Input.GetMouseButtonDown(0);

								input[id].position = GetMousePosition();

							
								input[id].pressed = getMouseButton;
								
								
								if (down && input[id].pressed == false) {
									input[id].released = true;
								} else {
									input[id].released = false;
								}
								
								break;
						}
					} else {
						input[id].released = false;
						input[id].pressed = false;
					}
				}
				
			} else {
				// For all 10 inputs
				Update_AI(id);
			}
		}
	}

	public static Vector2D GetMousePosition() {
		return(new Vector2D (Camera.main.ScreenToWorldPoint (Input.mousePosition)));
	}

	public static Vector2D GetTouchPosition(Vector2 touch) {
		return(new Vector2D (Camera.main.ScreenToWorldPoint (touch)));
	}

	public void Update_AI(int id = 0) {
		Slicer2DInput inp = input[id];
		if (inp.playing == false) {
			return;
		}

		inp.released = false;

		if (inp.currentEvent == null) {
			if (inp.eventsPlaying.Count > 0) {
				Slicer2DInputEvent e = inp.eventsPlaying.First();

				switch(e.eventType) {
					case Slicer2DInputEvent.EventType.SetPosition:
						inp.position = new Vector2D(e.position);
						break;

					case Slicer2DInputEvent.EventType.Move:
						break;

					case Slicer2DInputEvent.EventType.Press:
						inp.pressed = true;
						inp.clicked = true;
						break;

					case Slicer2DInputEvent.EventType.Release:
						inp.pressed = false;
						inp.released = true;
						break;
				}

				inp.timer = TimerHelper.Create();

				inp.currentEvent = e;

				inp.eventsPlaying.Remove(e);
			}
		} else {
			switch(inp.currentEvent.eventType) {
				case Slicer2DInputEvent.EventType.Move:
					inp.position.x = Mathf.Lerp((float)inp.position.x, inp.currentEvent.position.x, inp.timer.Get() / inp.currentEvent.time);
					inp.position.y = Mathf.Lerp((float)inp.position.y, inp.currentEvent.position.y, inp.timer.Get() / inp.currentEvent.time);
					break;
				}

			if (inp.timer.Get() > inp.currentEvent.time) {
				inp.currentEvent = null;

				if (inp.eventsPlaying.Count < 1) {
					inp.EventsCompleted();
				}
			}
		}

		if (inp.eventsPlaying.Count < 1) {
			if (inp.currentEvent == null) {
				if (inp.loop) {
					Play();
				}

				inp.playing = false;
			}
		}
	}

	public void Play(int id = 0) {
		Slicer2DInput inp = input[id];
		inp.pressed = false;
		inp.clicked = false;
		inp.position = Vector2D.Zero();

		inp.playing = true;

		inp.eventsPlaying = new List<Slicer2DInputEvent>(inp.eventsBank);
	}

	public void Stop(int id = 0) {
		input[id].playing = false;
	}

	public void Resume(int id = 0) {
		input[id].playing = true;
	}

	public void SetLoop(bool l, int id = 0) {
		input[id].loop = l;
	}

	public void SetRawInput(bool inp, int id = 0) {
		input[id].rawInput = inp;
	}
}