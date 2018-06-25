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

    // public Wall(float x1, float y1, float x2, float y2, float intensity, float frequency){
    //     this.x1 = x1;
    //     this.x2 = x2;
    //     this.y1 = y1;
    //     this.y2 = y2;
    //     i = intensity;
    //     f = frequency;
    // }

    public Wall(char side, float length=0.08f, float intensity=1.0f, float frequency=200.0f){
        
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

}

public class AMExample
{
    public static void Main(string[] args)
    {
        // Create an emitter, which connects to the first connected device
        AmplitudeModulationEmitter emitter = new AmplitudeModulationEmitter();

        Alignment alignment = emitter.getDeviceInfo().getDefaultAlignment();

        Controller controller = new Controller();

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

        Wall w1 = new Wall('s');
        Wall w2 = new Wall('e');

        w1.PrintPoints();
        w2.PrintPoints();

        var walls = new List<Wall> {w1, w2};

        for (;;)
        {
            foreach (var w in walls)
            {
            
                Frame frame = controller.Frame();
                HandList hands = frame.Hands;

                float z = 0.2f;

                if (!hands.IsEmpty)
                {
                    Hand hand = hands[0];

                    Vector3 pos = new Vector3(hand.PalmPosition.x, hand.PalmPosition.y, hand.PalmPosition.z);
                    Vector3 palm_pos = alignment.fromTrackingPositionToDevicePosition(pos);

                    z = palm_pos.z;
                }

                // Instruct the device to stop any existing actions and start producing this control point
                bool isOK = emitter.update(w.GetPoints(z));

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
