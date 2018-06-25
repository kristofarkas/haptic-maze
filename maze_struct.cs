using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Ultrahaptics;

// Initialise the maze stuff
JObject o1 = JObject.Parse(File.ReadAllText(@"c:\path_to_file\maze.json"));

// read JSON directly from a file
using (StreamReader file = File.OpenText(@"c:\path_to_file\maze.json"))
using (JsonTextReader reader = new JsonTextReader(file))
{
    JObject o2 = (JObject)JToken.ReadFrom(reader);
}

datatype[] current_position;

datatype[] maze;

string filename;

// Extract maze object from file
