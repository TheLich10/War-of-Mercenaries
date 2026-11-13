using UnityEngine;
using Pathfinding;
using System.Collections.Generic;

namespace Pathfinding
{
    [UniqueComponent(tag = "ai.destination")]
    public class AIDestinationSetter : VersionedMonoBehaviour
    {
        public string[] allTags = { "trapfire", "trapN", "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };

        public string unitName;
        private string[] searchTags;

        public Transform target;
        private IAstarAI ai;

        public enum AIState { TargetingBuilding, AttackingWall }
        public AIState currentState = AIState.TargetingBuilding;

        private float checkTimer = 1f;
        private float checkInterval = 1f;


        void Start()
        {
            ai = GetComponent<IAstarAI>();
            InitializeSearchTags();
        }

        void InitializeSearchTags()
        {
            if (unitName == "bedoiun" || unitName == "Battering_Gaul" || unitName == "persian" || unitName == "postion" || unitName == "Destroyer" || unitName == "BearWolf" || unitName == "Wom-kong")
            {
                searchTags = new string[] { "boomOP", "CenterCity", "wood", "stone", "iron", "power", "Resoures", "maker", "archer", "archerOP", "flameT", "flayTower", "Mirror", "spy", "spyClan", "boomTower", "Barracks", "clan", "exeBuild", "plan", "post", "fourMaker", "hadad", "horn", "NewWorr", "training", "Cave", "mongel" };
            }
            else if (unitName == "ChineseMongolian" || unitName == "EgyptianSavage" || unitName == "Ottoman" || unitName == "PersianSapper" || unitName == "ScalingEgyptian" || unitName == "Mightosour")
            {
                searchTags = new string[] { "boomOP", "archer", "archerOP", "flameT", "flayTower", "Mirror", "boomTower" };
            }
            else if (unitName == "KnightOnHorse" || unitName == "Sneaky")
            {
                searchTags = new string[] { "CenterCity", "wood", "stone", "iron", "power", "Resoures" };
            }
            else if (unitName == "Kamikaze")
            {
                searchTags = new string[] { "wall" };
            }
            else
            {
                searchTags = allTags;
            }
        }

        void Update()
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= checkInterval)
            {
                checkTimer = 0f;
                EvaluateTargetAndPath();
            }

            if (ai != null && target != null)
            {
                ai.destination = GetClosestPoint(target);
            }
        }

        void EvaluateTargetAndPath()
        {
            if (currentState == AIState.TargetingBuilding)
            {
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    target = FindClosestTarget(searchTags) ?? FindClosestTarget(allTags);
                }

                if (target != null && AstarPath.active != null)
                {
                    // التعديل 1: نتحقق من المسار لأقرب نقطة على محيط المبنى بدلاً من مركزه الداخلي
                    Vector3 actualDestination = GetClosestPoint(target);

                    // التعديل 2: نجبر A* على جلب العقد القابلة للمشي فقط لتجنب الأخطاء
                    NNConstraint constraint = NNConstraint.Default;
                    GraphNode myNode = AstarPath.active.GetNearest(transform.position, constraint).node;
                    GraphNode targetNode = AstarPath.active.GetNearest(actualDestination, constraint).node;

                    if (myNode != null && targetNode != null && !PathUtilities.IsPathPossible(myNode, targetNode))
                    {
                        Transform blockingWall = FindBlockingWall(target);

                        if (blockingWall != null)
                        {
                            currentState = AIState.AttackingWall;
                            target = blockingWall;
                            Debug.Log($"{gameObject.name} : الطريق مسدود! التحول للهجوم على الجدار المعترض ({target.name}).");
                        }
                        else
                        {
                            // التعديل 3: إزالة استهداف جدار عشوائي! 
                            // إذا لم يجد الشعاع جداراً، فهذا يعني أن الجدار تدمر للتو والشبكة لم تتحدث بعد.
                            // نتجاهل الأمر وننتظر حتى يتم تحديث شبكة المسارات.
                        }
                    }
                }
            }
            else if (currentState == AIState.AttackingWall)
            {
                // التأكد من أن الجدار قد تدمر أو تم إخفاؤه
                if (target == null || !target.gameObject.activeInHierarchy)
                {
                    currentState = AIState.TargetingBuilding;
                    target = FindClosestTarget(searchTags);
                }
                else
                {
                    Transform potentialTarget = FindClosestTarget(searchTags);
                    if (potentialTarget != null && AstarPath.active != null)
                    {
                        Vector3 actualDest = GetClosestPoint(potentialTarget);
                        NNConstraint constraint = NNConstraint.Default;
                        GraphNode myNode = AstarPath.active.GetNearest(transform.position, constraint).node;
                        GraphNode potentialNode = AstarPath.active.GetNearest(actualDest, constraint).node;

                        if (myNode != null && potentialNode != null && PathUtilities.IsPathPossible(myNode, potentialNode))
                        {
                            currentState = AIState.TargetingBuilding;
                            target = potentialTarget;
                            Debug.Log($"{gameObject.name} : تم فتح الطريق! العودة للهدف الأساسي.");
                        }
                    }
                }
            }
        }

        Transform FindBlockingWall(Transform goal)
        {
            if (goal == null) return null;

            // نوجه الشعاع إلى أقرب نقطة على المبنى وليس للمركز
            Vector3 targetPoint = GetClosestPoint(goal);
            Vector2 direction = (targetPoint - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, targetPoint);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distance);

            Transform closestWall = null;
            float minWallDist = Mathf.Infinity;

            // التعديل 4: البحث عن أقرب جدار يصطدم به الشعاع بدلاً من الاعتماد على العشوائية
            foreach (var hit in hits)
            {
                if (hit.collider != null && hit.collider.CompareTag("wall"))
                {
                    float dist = Vector2.Distance(transform.position, hit.point);
                    if (dist < minWallDist)
                    {
                        minWallDist = dist;
                        closestWall = hit.collider.transform;
                    }
                }
            }
            return closestWall;
        }

        Vector3 GetClosestPoint(Transform t)
        {
            Vector3 destination = t.position;
            Collider2D col2d = t.GetComponent<Collider2D>();

            if (col2d != null)
            {
                Vector2 closestPoint = col2d.ClosestPoint(transform.position);
                destination = new Vector3(closestPoint.x, closestPoint.y, t.position.z);
            }
            return destination;
        }

        Transform FindClosestTarget(string[] tagsToSearch)
        {
            Transform closest = null;
            float minDist = Mathf.Infinity;

            foreach (string tag in tagsToSearch)
            {
                GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in objs)
                {
                    float dist = Vector3.Distance(transform.position, obj.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = obj.transform;
                    }
                }
            }
            return closest;
        }
    }
}