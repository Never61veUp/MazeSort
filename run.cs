// See https://aka.ms/new-console-template for more information
using MazeSort;

var lines = new List<string>();
string? line;

while ((line = Console.ReadLine()) != string.Empty)
{
    lines.Add(line);
}

var state = GetState(lines);

var result = new Solve().Dijkstra(state);
Console.WriteLine(result);
return;

State GetState(List<string> lines)
{
    var hallway = lines[1].Replace("#", "");
        
    var rooms = new string[4];
        
    for(var i = 3; i <= 9; i+=2)
    {
        var roomIndex = (i - 3) / 2;
        var roomChars = new char[lines.Count - 3];
        for (var j = 2; j < lines.Count - 1; j++)
        {
            roomChars[j-2] = lines[j][i];
        }
        rooms[roomIndex] = new string(roomChars);
    }
    
    return new State(hallway, rooms);
}