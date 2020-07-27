using UnityEngine;

namespace RPG.Core.TerraWorld
{
    public class Raycasts
    {
        public static RaycastHit closestHit;
        private static RaycastHit[] hits = new RaycastHit[10];

        public static bool RaycastNonAllocSorted(Ray ray, bool bypassWater, bool underWater, int layerMask = ~0, float maxDistance = Mathf.Infinity, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
        {
            float closestDistance = Mathf.Infinity;
            int closestIndex = -1;
            int hitCount = Physics.RaycastNonAlloc(ray, hits, maxDistance, ~0, queryTriggerInteraction);

            for (int i = 0; i < hitCount; i++)
            {
                if (hits[i].collider == null) continue;
                int hitLayer = hits[i].transform.gameObject.layer;
                if (LayerMask.LayerToName(hitLayer) == "Water" && underWater) continue;

                if (hits[i].distance < closestDistance)
                {
                    closestDistance = hits[i].distance;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                int hitLayer = hits[closestIndex].transform.gameObject.layer;
                if (LayerMask.LayerToName(hitLayer) == "Water" && bypassWater) return false;

                if (layerMask == (layerMask | (1 << hitLayer)))
                {
                    closestHit = hits[closestIndex];
                    return true;
                }
            }

            return false;
        }
    }
}

