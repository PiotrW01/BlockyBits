using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockyBitsClient.src.Managers
{
    public class Octree
    {
        public OctreeNode Root { get; private set; }

        public Octree(Vector3 min, Vector3 max)
        {
            Root = new OctreeNode(new BoundingBox(min, max));
        }

        public void Insert(Object obj, Vector3 position)
        {
            Root.Insert(obj, position);
        }

        public List<Object> Search(Vector3 position, float radius)
        {
            List<Object> results = new List<Object>();
            Root.SearchRecursive(Root, position, radius, results);
            return results;
        }

        public bool Remove(Object target)
        {
            return Root.RemoveRecursive(Root, target);
        }
    }
}
