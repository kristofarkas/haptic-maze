/*using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Ultrahaptics;
using Leap;


public struct ButtonWidget
{C:\Users\Sofia\Dropbox\PhD\Hackathon\haptic-maze\haptic-maze\Program.cs
    public float radius;
    public float angle;
    public ButtonWidget(float r = 0.025f, float a = 0.0f){
        radius = r;
        angle = a;
    }
}

public static class MathF
{
    public static Func<double, float> Cos = angleR => (float)Math.Cos(angleR);
    public static Func<double, float> Sin = angleR => (float)Math.Sin(angleR);
}

// This example creates a static focal point at a frequency of 200Hz, 20cm above the device.
public class ButtonExample
{

    const float PI = (float)Math.PI;

    public static void NotMain(string[] args)
    {
        // Create an emitter, which connects to the first connected device
        AmplitudeModulationEmitter emitter = new AmplitudeModulationEmitter();

        // Create an aligment object which relates the tracking and device spaces
        Alignment alignment = emitter.getDeviceInfo().getDefaultAlignment();

        // Create a Leap Contoller
        Controller controller = new Controller();

        ButtonWidget button = new ButtonWidget();

        // Set the position of the new control point
        Vector3 position = new Vector3(0.0f, 0.0f, 0.2f);
        // Set how intense the feeling at the new control point will be
        float intensity = 1.0f;
        // Set the frequency of the control point, which can change the feeling of the sensation
        float frequency = 200.0f;

        // Define the control point
        AmplitudeModulationControlPoint point = new AmplitudeModulationControlPoint (position, intensity, frequency);
        var points = new List<AmplitudeModulationControlPoint> { point };

        // Wait for leap
        if(!controller.IsConnected)
        {
            Console.WriteLine("Waiting for Leap");
            while (!controller.IsConnected)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine(".");
            }
            Console.WriteLine("\n");
        }

        controller.EnableGesture (Gesture.GestureType.TYPE_KEY_TAP);

        if(controller.Config.SetFloat("Gesture.Swipe.MinDistance", 30) &&
            controller.Config.SetFloat("Gesture.Swipe.MinDownVelocity", 30) &&
            controller.Config.SetFloat("Gesture.Swipe.MinSeconds", 0.01f))
        {
            controller.Config.Save();
        }

        bool button_on = true;
        new Stopwatch();

        for(;;)
        {
            Frame frame = controller.Frame();
            HandList hands = frame.Hands;

            if(!hands.IsEmpty && button_on)
            {
                Hand hand = hands[0];

                for(int i = 0; i < frame.Gestures().Count; i++)
                {
                    Gesture gesture = frame.Gestures()[i];

                    if(gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)
                    {
                        button_on = false;

                        emitter.stop();
                        break;
                    }
                }
                position = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
                Vector3 normal = new Vector3(-hand.PalmNormal.x, -hand.PalmNormal.y, -hand.PalmNormal.z);
                Vector3 direction = new Vector3(hand.Direction.x, hand.Direction.y, hand.Direction.z);

                Vector3 device_position = alignment.fromTrackingPositionToDevicePosition(position);
                Vector3 device_normal = alignment.fromTrackingDirectionToDeviceDirection(normal).normalize();
                Vector3 device_direction = alignment.fromTrackingDirectionToDeviceDirection(direction).normalize();
                Vector3 device_palm_x = device_direction.cross(device_normal).normalize();

                device_position += (device_direction * MathF.Cos(button.angle) + device_palm_x * MathF.Sin(button.angle)) * button.radius;

                points[0].setPosition(device_position);
                // Instruct the device to stop any existing actions and start producing this control point
                emitter.update(points);
                // The emitter will continue producing this point until instructed to stop

                button.angle += 0.05f;
                button.angle = button.angle % (2.0f * PI);
            }
            else if(!hands.IsEmpty && !button_on)
            {
                emitter.stop();

                for(int i = 0; i < frame.Gestures().Count; i++)
                {
                    Gesture gesture = frame.Gestures()[i];

                    if(gesture.Type == Gesture.GestureType.TYPE_KEY_TAP)
                    {
                        button_on = true;

                        emitter.stop();
                        break;
                    }
                }
            }
            else
            {
                emitter.stop();
            }
            System.Threading.Thread.Sleep(10);
        }

        // // Dispose/destroy the emitter
        // emitter.Dispose();
        // emitter = null;

        // controller.Dispose ();
    }
}

*/

using System;
using System.Collections.Generic;

public class MazeGame
{   

    int[] current_cell;
    int[] current_position;
    int[,,] maze;

    public MazeGame()
    {
        mazeno = new int 1;
        is_winner = new bool 0;
        start_tile = new int[]{ 0,0};
        end_tile = new int[]{ 0,0};

        if (mazeno == 1)
        {
            maze = Maze1();
            start_cell = new int[]{1,0};
            end_cell = new int[]{1,1};
        }
        if (mazeno ==2)
        {
            maze = Maze2();
            start_cell = new int[] { 2, 0 };
            end_cell = new int[] { 1, 1 };
        }
        current_position = start_cell;

        current_cell = new int[] { maze[current_position[0], current_position[1], 0], maze[current_position[0], current_position[1], 1], maze[current_position[0], current_position[1], 2], maze[current_position[0], current_position[1], 3] };
        Console.WriteLine(string.Join(",", current_position));
        Console.WriteLine(string.Join(",", current_cell));
        PrintMazeCell();
    }

    public int[] MoveTo(char towards){

        // Input maze keys

        //Console.WriteLine(cki.Key.ToString());
        if (towards == 'e')
        {
            if (current_cell[1] == 0){
                current_position = new int[] { current_position[0] + 1, current_position[1] };
                Console.WriteLine(string.Join(",", current_position));
                current_cell = new int[] { maze[current_position[0], current_position[1], 0], maze[current_position[0], current_position[1], 1], maze[current_position[0], current_position[1], 2], maze[current_position[0], current_position[1], 3] };
                Console.Clear();
                PrintMazeCell();

                if (current_cell == end_cell)
                {
                    Console.WriteLine("You won!!!");
                }

            } else {
                System.Diagnostics.Process.Start("say", "Wall!");
                Console.Clear();
                PrintMazeCell();
            }
        }

        if (towards == 'w')
        {
            if (current_cell[3] == 0){
                current_position = new int[] { current_position[0] - 1, current_position[1] };
                Console.WriteLine(string.Join(",", current_position));
                current_cell = new int[] { maze[current_position[0], current_position[1], 0], maze[current_position[0], current_position[1], 1], maze[current_position[0], current_position[1], 2], maze[current_position[0], current_position[1], 3] };
                Console.Clear();
                PrintMazeCell();

                if (current_cell == end_cell)
                {
                    Console.WriteLine("You won!!!");
                }
            } else {
                System.Diagnostics.Process.Start("say", "Wall!");
                Console.Clear();
                PrintMazeCell();
            }
        }

        if (towards == 'n')
        {
            if (current_cell[0] == 0){
                current_position = new int[] { current_position[0], current_position[1] + 1 };
                Console.WriteLine(string.Join(",", current_position));
                current_cell = new int[] { maze[current_position[0], current_position[1], 0], maze[current_position[0], current_position[1], 1], maze[current_position[0], current_position[1], 2], maze[current_position[0], current_position[1], 3] };
                Console.Clear();
                PrintMazeCell();

                if (current_cell == end_cell)
                {
                    Console.WriteLine("You won!!!");
                }
            } else {
                System.Diagnostics.Process.Start("say", "Wall!");
                Console.Clear();
                PrintMazeCell();

            }
        } 

        if (towards == 's')
        {
            if (current_cell[2] == 0){
                current_position = new int[] { current_position[0], current_position[1] - 1 };
                Console.WriteLine(string.Join(",", current_position));
                current_cell = new int[] { maze[current_position[0], current_position[1], 0], maze[current_position[0], current_position[1], 1], maze[current_position[0], current_position[1], 2], maze[current_position[0], current_position[1], 3] };
                Console.Clear();
                PrintMazeCell();

                if (current_cell == end_cell)
                {
                    Console.WriteLine("You won!!!");
                }

            } else {
                System.Diagnostics.Process.Start("say", "Wall!");
                Console.Clear();
                PrintMazeCell();
            }
        }

        return this.current_cell;
    }


    public static int[,,] Maze2()
    {
        var maze = new int[5, 5, 4];
        // xy = 00
        maze[0, 0, 0] = 0;
        maze[0, 0, 1] = 0;
        maze[0, 0, 2] = 1;
        maze[0, 0, 3] = 1;
        //xy = 01
        maze[0, 1, 0] = 0;
        maze[0, 1, 1] = 1;
        maze[0, 1, 2] = 0;
        maze[0, 1, 3] = 1;
        //xy = 02
        maze[0, 2, 0] = 1;
        maze[0, 2, 1] = 0;
        maze[0, 2, 2] = 0;
        maze[0, 2, 3] = 1;
        //xy = 10
        maze[1, 0, 0] = 0;
        maze[1, 0, 1] = 1;
        maze[1, 0, 2] = 1;
        maze[1, 0, 3] = 0;
        //xy = 11
        maze[1, 1, 0] = 1;
        maze[1, 1, 1] = 1;
        maze[1, 1, 2] = 0;
        maze[1, 1, 3] = 1;
        //xy = 12
        maze[1, 2, 0] = 1;
        maze[1, 2, 1] = 0;
        maze[1, 2, 2] = 1;
        maze[1, 2, 3] = 0;
        //xy = 20 
        maze[2, 0, 0] = 0;
        maze[2, 0, 1] = 1;
        maze[2, 0, 2] = 1;
        maze[2, 0, 3] = 1;
        //xy = 21
        maze[2, 1, 0] = 0;
        maze[2, 1, 1] = 1;
        maze[2, 1, 2] = 0;
        maze[2, 1, 3] = 1;
        //xy = 22
        maze[2, 2, 0] = 1;
        maze[2, 2, 1] = 1;
        maze[2, 2, 2] = 0;
        maze[2, 2, 3] = 0;
        return maze;

    }


    public static int[,,]Maze1()
    {
        var maze = new int[2, 2, 4];
        // Walls for xy = 01
        maze[0, 1, 0] = 1;
        maze[0, 1, 1] = 0;
        maze[0, 1, 2] = 0;
        maze[0, 1, 3] = 1;

        //Walls for xy = 00
        maze[0, 0, 0] = 0;
        maze[0, 0, 1] = 0;
        maze[0, 0, 2] = 1;
        maze[0, 0, 3] = 1;

        //walls for xy = 10
        maze[1, 0, 0] = 1;
        maze[1, 0, 1] = 1;
        maze[1, 0, 2] = 1;
        maze[1, 0, 3] = 0;

        // Walls for xy = 11
        maze[1, 1, 0] = 1;
        maze[1, 1, 1] = 1;
        maze[1, 1, 2] = 1;
        maze[1, 1, 3] = 0;

        return maze;
    }

    public void PrintMazeCell()
    {
        if (current_cell[0]==0)
        {
            Console.WriteLine("  ");
        }
        if (current_cell[0] == 1)
        {
            Console.WriteLine("   ------------");
        }
        if (current_cell[1] == 1 && current_cell[3] == 1)
        {
            Console.WriteLine("   |          |\n   |          |\n   |          |\n   |          |\n   |          |");
        }
        if (current_cell[1] == 1 && current_cell[3] == 0)
        {
            Console.WriteLine("              |\n              |\n              |\n              |\n              |");
        }
        if (current_cell[1] == 0 && current_cell[3] == 1)
        {
            Console.WriteLine("   |           \n   |          \n   |           \n   |           \n   |           ");
        }
        if (current_cell[1] ==0 && current_cell[3]==0)
        {
            Console.WriteLine("            \n           \n            \n            \n            ");
        }
        if (current_cell[2] == 1)
        {
            Console.WriteLine("   ------------");
        }
    }
}




