using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlockyBitsClient.src.Managers
{
    public class OctreeNode
    {
        private const int MaxObjectsPerNode = 4;
        private const int MaxDepth = 5;

        public BoundingBox Bounds { get; private set; }
        public List<Object> Objects { get; private set; } = new List<Object>();
        public OctreeNode[] Children { get; private set; }
        public bool IsLeaf => Children == null;

        private int depth;

        public OctreeNode(BoundingBox bounds, int depth = 0)
        {
            this.Bounds = bounds;
            this.depth = depth;
        }

        public bool RemoveRecursive(OctreeNode node, Object target)
        {
            // Check if the object is within the node's bounds
            if (node.Bounds.Contains(target.Transform.GlobalPosition) == ContainmentType.Disjoint)
                return false;

            // Try to remove from this node
            if (node.Objects.Remove(target))
            {
                return true;
            }

            // Recursively check child nodes
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    if (RemoveRecursive(child, target))
                    {
                        // Optional: Collapse children if empty
                        TryCollapse(node);
                        return true;
                    }
                }
            }

            return false; // Object not found
        }

        // Optional: Merges children back into the parent node if they are empty
        private void TryCollapse(OctreeNode node)
        {
            if (node.Children == null) return;

            bool allChildrenEmpty = true;
            foreach (var child in node.Children)
            {
                if (child.Objects.Count > 0 || child.Children != null)
                {
                    allChildrenEmpty = false;
                    break;
                }
            }

            if (allChildrenEmpty)
            {
                node.Children = null; // Merge children back
            }
        }

        public void SearchRecursive(OctreeNode node, Vector3 position, float radius, List<Object> results)
        {
            // Check if the node's AABB intersects with the sphere
            if (!node.Bounds.Intersects(new BoundingSphere(position, radius)))
                return;

            // Check objects in this node
            foreach (var obj in node.Objects)
            {
                if (Vector3.Distance(obj.Transform.GlobalPosition, position) <= radius)
                {
                    results.Add(obj);
                }
            }

            // Recursively check child nodes
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                {
                    SearchRecursive(child, position, radius, results);
                }
            }
        }

        public void Insert(Object obj, Vector3 position)
        {
            if (Bounds.Contains(position) == ContainmentType.Disjoint)
                return;

            if (IsLeaf && Objects.Count < MaxObjectsPerNode || depth >= MaxDepth)
            {
                Objects.Add(obj);
                return;
            }

            if (IsLeaf)
                Subdivide();

            foreach (var child in Children)
                child.Insert(obj, position);
        }

        private void Subdivide()
        {
            Children = new OctreeNode[8];
            Vector3 size = (Bounds.Max - Bounds.Min) / 2;
            Vector3 min = Bounds.Min;

            for (int i = 0; i < 8; i++)
            {
                Vector3 newMin = min + new Vector3(
                    (i & 1) == 0 ? 0 : size.X,
                    (i & 2) == 0 ? 0 : size.Y,
                    (i & 4) == 0 ? 0 : size.Z
                );

                Vector3 newMax = newMin + size;
                Children[i] = new OctreeNode(new BoundingBox(newMin, newMax), depth + 1);
            }
        }

        public bool Contains(Vector3 position)
        {
            return ContainmentType.Contains == Bounds.Contains(position);
        }
    }
}
