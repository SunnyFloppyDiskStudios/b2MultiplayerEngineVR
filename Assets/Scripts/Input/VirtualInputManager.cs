using System;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ButtonData
{
    public bool jump = false;
    public bool fire = false;
    public Vector2 joystick = Vector2.zero;
}

public class ManagedTouch
{
    public int index = 0;
    public int fingerId = 0;

    public Touch touch;
    public Vector2 start;
}

public class TouchData
{
    public List<ManagedTouch> touchList;

    public TouchData()
    {
        touchList = new List<ManagedTouch>();
    }
}

public class VirtualInputManager : InputManager
{
    public VirtualInputSample sample;

    public ButtonData buttonData;

    public override void Initialise()
    {
        sample = new VirtualInputSample();
        sample.Initialise();

        buttonData = new ButtonData();

        cameraController.inputType = EInput.VIRTUAL;
        cameraController.menuManager.inputType = EInput.VIRTUAL;
        
        cameraController.menuManager.buttonData = buttonData;
    }

    public override InputSample GetInputSample()
    {
        return sample;
    }

    private void PollPhysicalJoystick() {
        
    }

    private void PollPhysicalButtons() {
        
    }

    public override void PerFrameUpdate()
    {
        PollPhysicalJoystick(); //s mobile menu manager init

        sample.joystickX.value = -buttonData.joystick.x;
        sample.joystickY.value = buttonData.joystick.y;

        sample.joystickX.Quantize();
        sample.joystickY.Quantize();

        PollPhysicalButtons(); //s mobile menu manager init

        //sort button data
        if (buttonData.jump)
        {
            sample.jump.state = EButtonState.ON_PRESS;
            sample.jump.changeDetected = true;

            buttonData.jump = false;
        }

        if (buttonData.fire)
        {
            sample.fire.state = EButtonState.ON_PRESS;
            sample.fire.changeDetected = true;

            buttonData.fire = false;
        }

        //inputs must be sampled every frame
        sample.jump.Poll(true, fuzz);
        sample.fire.Poll(true, fuzz);

        cameraController.Poll();
        sample.yaw = cameraController.yaw;

        sample.pitch = 0.0f;

        if (!cameraController.isThirdPerson)
        {
            sample.pitch = cameraController.pitch;
        }
    }

    public override void Tick()
    {
        sample.jump.Reset();
        sample.fire.Reset();

        sample.timestamp++;

        if (sample.timestamp >= Settings.MAX_INPUT_INDEX)
        {
            sample.timestamp -= Settings.MAX_INPUT_INDEX;
        }
    }
}
