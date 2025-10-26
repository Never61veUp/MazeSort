namespace MazeSort;

public class Solve
{
    private readonly char[] _amphipodTypes = ['A', 'B', 'C', 'D'];
    private readonly int[] _energyCost = [1, 10, 100, 1000];
    private readonly int[] _roomEntrances = [2, 4, 6, 8];
    
    private readonly bool[] _isEntrance = new bool[11];
    
    private State _goal;

    public int Dijkstra(State target)
    {
        var rooms = new string[]
        {
            new string('A', target.Rooms[0].Length),
            new string('B', target.Rooms[1].Length),
            new string('C', target.Rooms[2].Length),
            new string('D', target.Rooms[3].Length)
        };
        
        _goal = new State("...........", rooms);
        
        var best = new Dictionary<State, int>();
        var queue = new PriorityQueue<State, int>();

        best[target] = 0;
        queue.Enqueue(target, 0);

        var counter = 0;

        while (queue.Count > 0)
        {
            counter++;

            var current = queue.Dequeue();
            var currentCost = best[current];

            if (current == _goal)
            {
                Console.WriteLine($"Reached Goal in {counter} steps.");
                return currentCost;
            }
            
            foreach (var (next, cost) in GetMoves(current))
            {
                var newCost = currentCost + cost;
                if (!best.TryGetValue(next, out var value) || newCost < value)
                {
                    value = newCost;
                    best[next] = value;
                    queue.Enqueue(next, newCost);
                }
            }
        }

        return -1;
    }

    private IEnumerable<(State next, int cost)> GetMoves(State current)
    {
        var moves = new List<(State next, int cost)>();

        var roomSizes = current.Rooms[0].Length;

        for (var roomIndex = 0; roomIndex < 4; roomIndex++)
        {
            var room = current.Rooms[roomIndex];

            var topPos = -1;
            var topChar = '.';
            for (var depth = 0; depth < roomSizes; depth++)
                if (room[depth] != '.')
                {
                    topPos = depth;
                    topChar = room[depth];
                    break;
                }

            if (topPos == -1)
                continue;

            var targetRoom = _amphipodTypes[roomIndex];

            var roomIsDone = true;
            foreach (var roomItem in room)
                if (roomItem != targetRoom)
                {
                    roomIsDone = false;
                    break;
                }

            // Console.WriteLine($"Room {roomIndex} is done: {roomIsDone}");
            if (roomIsDone)
                continue;

            var entrancePos = _roomEntrances[roomIndex];
            for (var hallPos = 0; hallPos < 11; hallPos++)
            {
                if (_roomEntrances.Contains(hallPos))
                    continue;
                if (current.Hallway[hallPos] != '.')
                    continue;

                var stepDirection = hallPos < entrancePos ? -1 : 1;
                var wayClear = true;

                for (var pos = entrancePos; pos != hallPos; pos += stepDirection)
                    if (current.Hallway[pos] != '.')
                    {
                        wayClear = false;
                        break;
                    }

                if (!wayClear)
                    continue;

                var steps = topPos + 1 + Math.Abs(hallPos - entrancePos);
                var cost = steps * _energyCost[topChar - 'A'];

                var newHallway = current.Hallway.ToCharArray();
                newHallway[hallPos] = topChar;
                var newRooms = current.Rooms.ToArray();
                var newRoomChars = newRooms[roomIndex].ToCharArray();
                newRoomChars[topPos] = '.';
                newRooms[roomIndex] = new string(newRoomChars);

                moves.Add((new State(new string(newHallway), newRooms), cost));
            }
        }

        for (var hallPos = 0; hallPos < 11; hallPos++)
        {
            var c = current.Hallway[hallPos];
            if (c is '.' or < 'A' or > 'D')
                continue;

            var targetRoomIdx = c - 'A';
            var targetRoom = current.Rooms[targetRoomIdx];
            var entrance = _roomEntrances[targetRoomIdx];

            var canEnter = true;
            var deepestFree = -1;
            for (var depth = roomSizes - 1; depth >= 0; depth--)
                if (targetRoom[depth] == '.')
                {
                    deepestFree = depth;
                }
                else if (targetRoom[depth] != c)
                {
                    canEnter = false;
                    break;
                }

            if (!canEnter || deepestFree == -1)
                continue;

            var stepDir = hallPos < entrance ? 1 : -1;
            var pathClear = true;
            
            for (var pos = hallPos + stepDir; pos != entrance; pos += stepDir)
                if (current.Hallway[pos] != '.')
                {
                    pathClear = false;
                    break;
                }

            if (!pathClear) 
                continue;

            var steps = deepestFree + 1 + Math.Abs(hallPos - entrance);
            var cost = steps * _energyCost[c - 'A'];

            var newHallway = current.Hallway.ToCharArray();
            newHallway[hallPos] = '.';
            var newRooms = current.Rooms.ToArray();
            var newRoomChars = newRooms[targetRoomIdx].ToCharArray();
            newRoomChars[deepestFree] = c;
            newRooms[targetRoomIdx] = new string(newRoomChars);

            moves.Add((new State(new string(newHallway), newRooms), cost));
        }

        return moves;
    }
}