namespace CNull.Interpreter
{
    public class DependencyTree<T> where T : notnull
    {
        private readonly Dictionary<T, List<T>> _graph = [];
        private HashSet<T> _visited = [];
        private HashSet<T> _onPath = [];
        private readonly List<T> _sortedOrder = [];

        public void AddDependency(T obj, T dependency)
        {
            if (!_graph.ContainsKey(obj))
                _graph.Add(obj, []);

            if(!_graph.ContainsKey(dependency))
                _graph.Add(dependency, []);

            _graph[obj].Add(dependency);
        }

        public bool Build()
        {
            _visited = [];
            _onPath = [];

            if (_graph.Keys.Where(node => !_visited.Contains(node)).Any(node => !DepthFirstSearch(node)))
                return false;

            _sortedOrder.Reverse();
            return true;
        }

        private bool DepthFirstSearch(T node)
        {
            _visited.Add(node);
            _onPath.Add(node);

            foreach (var neighbor in _graph[node])
            {
                if (_onPath.Contains(neighbor))
                    return false;

                if (_visited.Contains(neighbor)) 
                    continue;

                if (!DepthFirstSearch(neighbor))
                    return false;
            }

            _onPath.Remove(node);
            _sortedOrder.Add(node);
            return true;
        }
    }
}
