using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Ultrahaptics;
using Leap;

// This example creates a static focal point at a frequency of 200Hz, 20cm above the device.

public class Wall
{
    float x1;
    float x2;
    float y1;
    float y2;
    float f;
    float i;
    public char side;

    // public Wall(float x1, float y1, float x2, float y2, float intensity, float frequency){
    //     this.x1 = x1;
    //     this.x2 = x2;
    //     this.y1 = y1;
    //     this.y2 = y2;
    //     i = intensity;
    //     f = frequency;
    // }

    public Wall(char side, float length=0.09f, float intensity=1.0f, float frequency=200.0f){
        
        this.side = side;
        
        switch(side)
        {
            case 'n':
                x1 = -length/2;
                y1 = length/2;
                x2 = length/2;
                y2 = length/2;
                break;
            case 'e':
                x1 = length/2;
                y1 = -length/2;
                x2 = length/2;
                y2 = length/2;
                break;
            case 's':
                x1 = length/2;
                y1 = -length/2;
                x2 = -length/2;
                y2 = -length/2;
                break;
            case 'w':
                x1 = -length/2;
                y1 = length/2;
                x2 = -length/2;
                y2 = -length/2;
                break;
            default:
                Console.WriteLine("WRONG SIDE!");
                break;
        }
        i = intensity;
        f = frequency;
    }

    public void PrintPoints(){
        foreach (var p in GetPoints(0)){
            Console.Write(p.getPosition());
        }
        Console.WriteLine();
    }

    public List<AmplitudeModulationControlPoint> GetPoints(float z){
        AmplitudeModulationControlPoint point1 = new AmplitudeModulationControlPoint(x1, y1, z, i, f);
        AmplitudeModulationControlPoint point2 = new AmplitudeModulationControlPoint(2*x1/3 + 1*x2/3, 2*y1/3 + 1*y2/3, z, i, f);
        AmplitudeModulationControlPoint point3 = new AmplitudeModulationControlPoint(1*x1/3 + 2*x2/3, 1*y1/3 + 2*y2/3, z, i, f);
        AmplitudeModulationControlPoint point4 = new AmplitudeModulationControlPoint(x2, y2, z, i, f);

        var points = new List<AmplitudeModulationControlPoint> { point1, point2, point3, point4 };

        return points;
    }

    public static List<Wall> GenWalls(int[] cell){
        Console.WriteLine("Creating cell with walls:");
        var ws = new List<Wall>();
        for (int i = 0; i < cell.Length; i++){
            if (cell[i] == 1) {
                ws.Add(new Wall(cell_to_side(i)));
                Console.WriteLine(cell_to_side(i));
            }
        }
        Console.WriteLine(ws.Count);
        return ws;
    }

    static char cell_to_side(int i) {
        switch (i)
        {
            case 0: return 'n'; 
            case 1: return 'e'; 
            case 2: return 's'; 
            case 3: return 'w';
            default: Console.WriteLine("BROKEN!"); return 'n';
        }
    }
}

public class GameLoop
{
    public static void Main(string[] args)
    {
        // Create an emitter, which connects to the first connected device
        AmplitudeModulationEmitter emitter = new AmplitudeModulationEmitter();

        Alignment alignment = emitter.getDeviceInfo().getDefaultAlignment();

        Controller controller = new Controller();

        MazeGame game = new MazeGame();

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


        Console.WriteLine("Leap controller connected.");

        // Create walls

        Wall w1 = new Wall('w');
        Wall w2 = new Wall('e');
        Wall w3 = new Wall('n');

        var walls = new List<Wall> {w1, w2, w3};

        var recently_moved = false;

        for (;;)
        {
            for (int i = 0; i < walls.Count; i++){
                            
                Frame frame = controller.Frame();
                HandList hands = frame.Hands;

                float z = 0.2f;

                if (!hands.IsEmpty)
                {
                    Hand hand = hands[0];

                    Vector3 pos = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
                    Vector3 palm_pos = alignment.fromTrackingPositionToDevicePosition(pos);

                    float max_center = 0.05f;
                    float far_center = 0.09f;

                    if (palm_pos.x > far_center && !recently_moved && Math.Abs(palm_pos.y) < max_center) {
                        Console.WriteLine("Moving right!");
                        var cell = game.MoveTo('e');
                        walls = Wall.GenWalls(cell);
                        recently_moved = true;
                        break;
                    }

                    if (palm_pos.x < -far_center && !recently_moved && Math.Abs(palm_pos.y) < max_center) {
                        Console.WriteLine("Moving left!");
                        var cell = game.MoveTo('w');
                        walls = Wall.GenWalls(cell);
                        recently_moved = true;
                        break;

                    }

                    if (palm_pos.y < -far_center && !recently_moved && Math.Abs(palm_pos.x) < max_center) {
                        Console.WriteLine("Moving down!");
                        var cell = game.MoveTo('s');
                        walls = Wall.GenWalls(cell);
                        recently_moved = true;
                        break;
                    }

                    if (palm_pos.y > far_center && !recently_moved && Math.Abs(palm_pos.x) < max_center) {
                        Console.WriteLine("Moving up!");
                        var cell = game.MoveTo('n');
                        walls = Wall.GenWalls(cell);
                        recently_moved = true;
                        break;
                    }

                    if (Math.Abs(palm_pos.x) < max_center && Math.Abs(palm_pos.y) < max_center && recently_moved) {
                        Console.WriteLine("Reset movement");
                        recently_moved = false;
                    }

                    z = palm_pos.z;
                }

                // Instruct the device to stop any existing actions and start producing this control point
                bool isOK = emitter.update(walls[i].GetPoints(z));

                System.Threading.Thread.Sleep(10);
            }
        }

        // The emitter will continue producing this point until instructed to stop

        // // Wait until the program is ready to stop
        // Console.ReadKey();

        // // Stop the emitter
        // emitter.stop();

        // // Dispose/destroy the emitter
        // emitter.Dispose();
        // emitter = null;
    }

}

