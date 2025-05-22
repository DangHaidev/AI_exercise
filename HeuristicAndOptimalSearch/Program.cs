using System;
using System.Collections.Generic;
using System.Linq;

class GraphSearch
{
    private Dictionary<string, Dictionary<string, int>> graph;
    private Dictionary<string, int> heuristic;
    private List<string> steps;

    public GraphSearch()
    {
        graph = new Dictionary<string, Dictionary<string, int>>();
        heuristic = new Dictionary<string, int>();
        steps = new List<string>();
    }

    // Nhập đồ thị và heuristic
    public void InputGraph()
    {
        Console.WriteLine("Nhập số lượng node:");
        int nodeCount = int.Parse(Console.ReadLine());

        Console.WriteLine("Nhập số lượng cạnh:");
        int edgeCount = int.Parse(Console.ReadLine());

        Console.WriteLine("Nhập các cạnh (node1 node2 weight):");
        for (int i = 0; i < edgeCount; i++)
        {
            var input = Console.ReadLine().Split();
            string node1 = input[0], node2 = input[1];
            int weight = int.Parse(input[2]);

            if (!graph.ContainsKey(node1))
                graph[node1] = new Dictionary<string, int>();
            if (!graph.ContainsKey(node2))
                graph[node2] = new Dictionary<string, int>();

            graph[node1][node2] = weight;
        }

        Console.WriteLine("Nhập giá trị heuristic cho từng node (node h_value):");
        for (int i = 0; i < nodeCount; i++)
        {
            var input = Console.ReadLine().Split();
            heuristic[input[0]] = int.Parse(input[1]);
        }
    }

    // Best-First Search (theo mã giả)
    private (List<string>, List<string>) BestFirstSearch(string start, string goal)
    {
        steps.Add("\n=== Best-First Search ===");
        var visited = new HashSet<string>();
        var L = new List<(string node, List<string> path)> { (start, new List<string> { start }) };

        while (true)
        {
            // 2.1. Kiểm tra danh sách rỗng
            if (L.Count == 0)
            {
                steps.Add("Không tìm thấy đường đi đến đích.");
                return (steps, new List<string>());
            }

            // 2.2. Lấy node đầu tiên
            var (current, path) = L[0];
            L.RemoveAt(0);
            steps.Add($"Xét node {current} (h={heuristic[current]}), đường đi hiện tại: [{string.Join(", ", path)}]");

            // 2.3. Kiểm tra đích
            if (current == goal)
            {
                int cost = CalculatePathCost(path);
                steps.Add($"Đã tìm thấy đích {goal}. Đường đi: [{string.Join(", ", path)}], chi phí: {cost}");
                return (steps, path);
            }

            // 2.4. Thêm tất cả hàng xóm và sắp xếp
            if (!visited.Contains(current))
            {
                visited.Add(current);
                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor.Key))
                    {
                        var newPath = new List<string>(path) { neighbor.Key };
                        L.Add((neighbor.Key, newPath));
                        steps.Add($"Thêm {neighbor.Key} vào danh sách (h={heuristic[neighbor.Key]})");
                    }
                }
                // Sắp xếp L theo giá trị heuristic tăng dần
                L.Sort((a, b) => heuristic[a.node].CompareTo(heuristic[b.node]));
            }
        }
    }


    // Simple Hill Climbing
    private (List<string>, List<string>) SimpleHillClimbing(string start, string goal)
    {
        steps.Add("\n=== Simple Hill Climbing ===");
        string current = start;
        var path = new List<string> { current };
        steps.Add($"Khởi tạo tại {current} (h={heuristic[current]})");

        while (current != goal)
        {
            if (!graph.ContainsKey(current) || graph[current].Count == 0)
            {
                steps.Add("Không có hàng xóm, thuật toán dừng.");
                return (steps, new List<string>());
            }

            var neighbors = graph[current];
            bool moved = false;
            foreach (var neighbor in neighbors)
            {
                if (heuristic[neighbor.Key] < heuristic[current])
                {
                    current = neighbor.Key;
                    path.Add(current);
                    steps.Add($"Di chuyển đến {current} (h={heuristic[current]})");
                    moved = true;
                    break;
                }
            }

            if (!moved)
            {
                steps.Add($"Không tìm thấy hàng xóm tốt hơn tại {current}. Thuật toán dừng.");
                return (steps, new List<string>());
            }
        }

        int cost = CalculatePathCost(path);
        steps.Add($"Đã tìm thấy đích {goal}. Đường đi: [{string.Join(", ", path)}], chi phí: {cost}");
        return (steps, path);
    }

    // Steepest-Ascent Hill Climbing
    private (List<string>, List<string>) SteepestAscentHillClimbing(string start, string goal)
    {
        steps.Add("\n=== Steepest-Ascent Hill Climbing ===");
        string current = start;
        var path = new List<string> { current };
        steps.Add($"Khởi tạo tại {current} (h={heuristic[current]})");

        while (current != goal)
        {
            if (!graph.ContainsKey(current) || graph[current].Count == 0)
            {
                steps.Add("Không có hàng xóm, thuật toán dừng.");
                return (steps, new List<string>());
            }

            var neighbors = graph[current];
            var bestNeighbor = neighbors.OrderBy(n => heuristic[n.Key]).FirstOrDefault();
            if (heuristic[bestNeighbor.Key] < heuristic[current])
            {
                current = bestNeighbor.Key;
                path.Add(current);
                steps.Add($"Di chuyển đến {bestNeighbor.Key} (h={heuristic[bestNeighbor.Key]})");
            }
            else
            {
                steps.Add($"Không tìm thấy hàng xóm tốt hơn tại {current}. Thuật toán dừng.");
                return (steps, new List<string>());
            }
        }

        int cost = CalculatePathCost(path);
        steps.Add($"Đã tìm thấy đích {goal}. Đường đi: [{string.Join(", ", path)}], chi phí: {cost}");
        return (steps, path);
    }

    // Branch and Bound Search
    private (List<string>, List<string>) BranchAndBoundSearch(string start, string goal)
    {
        steps.Add("\n=== Branch and Bound Search ===");
        var priorityQueue = new PriorityQueue<(int cost, string node, List<string> path)>(Comparer<(int, string, List<string>)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
        priorityQueue.Enqueue((0, start, new List<string> { start }));
        var visited = new HashSet<string>();

        while (priorityQueue.Count > 0)
        {
            var (cost, current, path) = priorityQueue.Dequeue();
            steps.Add($"Xét node {current}, chi phí hiện tại: {cost}, đường đi: [{string.Join(", ", path)}]");

            if (current == goal)
            {
                steps.Add($"Đã tìm thấy đích {goal}. Đường đi: [{string.Join(", ", path)}], chi phí: {cost}");
                return (steps, path);
            }

            if (!visited.Contains(current))
            {
                visited.Add(current);
                foreach (var neighbor in graph[current])
                {
                    if (!visited.Contains(neighbor.Key))
                    {
                        var newCost = cost + neighbor.Value;
                        var newPath = new List<string>(path) { neighbor.Key };
                        priorityQueue.Enqueue((newCost, neighbor.Key, newPath));
                        steps.Add($"Thêm {neighbor.Key} vào hàng đợi, chi phí: {newCost}");
                    }
                }
            }
        }

        steps.Add("Không tìm thấy đường đi đến đích.");
        return (steps, new List<string>());
    }

    // Tính chi phí đường đi
    private int CalculatePathCost(List<string> path)
    {
        int cost = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            cost += graph[path[i]][path[i + 1]];
        }
        return cost;
    }

    // Chạy chương trình
    public void Run()
    {
        InputGraph();

        Console.WriteLine("Nhập node bắt đầu:");
        string start = Console.ReadLine();
        Console.WriteLine("Nhập node đích:");
        string goal = Console.ReadLine();

        Console.WriteLine("Chọn thuật toán (1: Best-First Search, 2: Simple Hill Climbing, 3: Steepest-Ascent Hill Climbing, 4: Branch and Bound):");
        int choice = int.Parse(Console.ReadLine());

        List<string> steps;
        List<string> path;

        switch (choice)
        {
            case 1:
                (steps, path) = BestFirstSearch(start, goal);
                break;
            case 2:
                (steps, path) = SimpleHillClimbing(start, goal);
                break;
            case 3:
                (steps, path) = SteepestAscentHillClimbing(start, goal);
                break;
            case 4:
                (steps, path) = BranchAndBoundSearch(start, goal);
                break;
            default:
                Console.WriteLine("Lựa chọn không hợp lệ.");
                return;
        }

        Console.WriteLine(string.Join("\n", steps));
    }

    // Lớp PriorityQueue tùy chỉnh
    public class PriorityQueue<T>
    {
        private readonly List<T> list;
        private readonly IComparer<T> comparer;

        public PriorityQueue(IComparer<T> comparer)
        {
            this.list = new List<T>();
            this.comparer = comparer;
        }

        public void Enqueue(T item)
        {
            list.Add(item);
            list.Sort(comparer);
        }

        public T Dequeue()
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public int Count => list.Count;
    }
}

class Program
{
    static void Main()
    {
        var search = new GraphSearch();
        search.Run();
    }
}