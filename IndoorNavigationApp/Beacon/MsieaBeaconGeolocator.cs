using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using IndoorNavigationApp.Models;
using IndoorNavigationApp.Service;

namespace IndoorNavigationApp.Beacon
{
    public sealed class MsieaBeaconGeolocator : IBeaconGeolocator
    {
        private readonly IMapServiceClient _mapServiceClient;

        public MsieaBeaconGeolocator(IMapServiceClient mapServiceClient)
        {
            _mapServiceClient = mapServiceClient;
        }

        public async Task<BeaconGeolocation> FindGeolocationByBeaconData(BeaconData beaconData, Map map = null)
        {
            if (map == null)
            {
                var beaconId = await _mapServiceClient.GetBeaconIdByBeaconDataAsync(beaconData).ConfigureAwait(false);
                if (beaconId == null)
                {
                    return null;
                }

                map = await _mapServiceClient.GetMapByBeaconIdAsync(beaconId.Value).ConfigureAwait(false);
                if (map == null)
                {
                    return null;
                }
            }

            var nearestNode = map.Nodes.FirstOrDefault(x => x.BeaconData == beaconData);
            if (nearestNode == null)
            {
                return null;
            }

            return new BeaconGeolocation(
                map: map, 
                nearestNode: nearestNode,
                nearestBeacon: beaconData);
        }

        static Stack<int> Problem1(int[,] adjacentVertices, int sourceVertex, int targetVertex)
        {
            var verticesCount = adjacentVertices.GetLength(0);
            
            var parentVertices = new int[verticesCount];
            var distances = new int[verticesCount];
            for (var vertex = 0; vertex < verticesCount; vertex++)
            {
                distances[vertex] = int.MaxValue;
            }

            distances[sourceVertex] = 0;

            var unvisitedVertices = new List<int>();
            unvisitedVertices.AddRange(Enumerable.Range(0, verticesCount));

            while (unvisitedVertices.Count > 0)
            {
                var currentVertex = unvisitedVertices.OrderBy(vertex => distances[vertex]).First();
                if (currentVertex == targetVertex)
                {
                    break;
                }

                unvisitedVertices.Remove(currentVertex);

                if (distances[currentVertex] == int.MaxValue)
                {
                    break;
                }

                for (int adjacentVertex = 0; adjacentVertex < verticesCount; adjacentVertex++)
                {
                    if (adjacentVertices[currentVertex, adjacentVertex] > 0)
                    {
                        var newDistance = distances[currentVertex] + 1;
                        if (unvisitedVertices.Contains(adjacentVertex) && distances[adjacentVertex] > newDistance)
                        {
                            distances[adjacentVertex] = newDistance;
                            parentVertices[adjacentVertex] = currentVertex;
                        }
                    }
                }
            }

            if (distances[targetVertex] == int.MaxValue)
            {
                throw new Exception();
            }
            
            var path = new Stack<int>();
            var currentPathVertex = targetVertex;
            while (currentPathVertex != sourceVertex)
            {
                path.Push(currentPathVertex);
                currentPathVertex = parentVertices[currentPathVertex];
            }
            path.Push(sourceVertex);

            return path;
        }

        public IList<RouteSegment> MakeRoute(Building building, Node sourceNode, Node destinationNode)
        {
            var path = Problem1(building.AdjacencyMatrix, sourceNode.NavigationId, destinationNode.NavigationId);

            var nodes = building.Maps.SelectMany(x => x.Nodes).ToImmutableDictionary(x => x.NavigationId);

            var segments = new List<RouteSegment>();
            var previousVertex = path.Pop();
            while (path.Count > 0)
            {
                var currentVertex = path.Pop();
                segments.Add(new RouteSegment(nodes[previousVertex], nodes[currentVertex]));
                previousVertex = currentVertex;
            }

            return segments;
        }
    }    
}