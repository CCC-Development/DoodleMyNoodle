﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBallInputs : GameMonoBehaviour
{
    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        // fbessette: oui, ce code est dégeux mais chu trop paraisseux pour le faire plus compacte

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.Space });
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.RightArrow, state = SimInputKeycode.State.Pressed });
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.RightArrow, state = SimInputKeycode.State.Released });
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.LeftArrow, state = SimInputKeycode.State.Pressed });
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.LeftArrow, state = SimInputKeycode.State.Released });
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.UpArrow, state = SimInputKeycode.State.Pressed });
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.UpArrow, state = SimInputKeycode.State.Released });
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.DownArrow, state = SimInputKeycode.State.Pressed });
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SimulationController.instance.SubmitInput(new SimInputKeycode() { keyCode = KeyCode.DownArrow, state = SimInputKeycode.State.Released });
        }
    }

    public override void OnGameFixedUpdate()
    {
        base.OnGameFixedUpdate();
    }
}