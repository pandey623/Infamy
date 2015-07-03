//    public class ContextMap {
//
//        public int resolution;
//        public float sphereCastRadius;
//        private Entity entity;
//        private SensorSystem sensorSystem;
//        private Timer castTimer;
//        public SteerNode[] nodes;
//        public SteerNode highest;
//        private List<CastHit> castHits;
//        private Transform transform;
//        public float maxScore;
//        public float minScore;
//
//        public ContextMap(Entity entity, int resolution) {
//            this.sphereCastRadius = 5f;
//            this.entity = entity;
//            this.resolution = resolution;
//            this.sensorSystem = entity.sensorSystem;
//            this.castTimer = new Timer();
//            this.nodes = CreateNodeMap(resolution);
//            this.castHits = new List<CastHit>(20); //todo pool these for less garbage
//            this.transform = entity.transform;
//            highest = nodes[0];
//        }
//
//        public Vector3 Update() {
//            for (int i = 0; i < nodes.Length; i++) {
//                nodes[i].score = 0f;
//            }
//            castHits.Clear();
//            GatherDangerInfo();
//            SaturateTarget();
//            SaturateDanger();
//            return DesiredDirection();
//        }
//
//        private Vector3 DesiredDirection() {
//            maxScore = float.MinValue;
//            minScore = float.MaxValue;
//            for (int i = 0; i < nodes.Length; i++) {
//                float score = nodes[i].score;
//                if (score > maxScore) {
//                    maxScore = score;
//                    highest = nodes[i];
//                } else if (score == maxScore && UnityEngine.Random.Range(0f, 1f) >= 0.5f) {
//                    Debug.Log("TIE");
//                    highest = nodes[i];
//                } else if (score < minScore) {
//                    minScore = score;
//                }
//            }
//
//            DotContainer dots = sensorSystem.RightUpForwardDotToVector(entity.controls.destination);
//
//            int index = GetIndex(dots.right, dots.up, resolution);
//            var targetNode = nodes[index];
//            if (targetNode != highest && targetNode.score == highest.score) {
//                highest = targetNode;
//            }
//
//            float percentX = highest.x / (float)(resolution - 1);
//            float percentY = highest.y / (float)(resolution - 1);
//
//            if (percentX > 0.45f && percentX < 0.55f && percentY > 0.45f && percentY < 0.55f) {
//                return entity.controls.destination;
//            }
//
//            float xValue = (2 * percentX) - 1;
//            float yValue = (2 * percentY) - 1;
//            // Debug.Log("index: " + highest.x + ", " + highest.y + " mapped[-1, 1] " + xValue + ", " + yValue);
//            return transform.position + transform.TransformDirection(new Vector3(xValue, yValue, 1f).normalized);
//        }
//
//        private void GatherDangerInfo() {
//            Vector3 topLeft = new Vector3(-1, 1, 0);
//            Vector3 topRight = new Vector3(1, 1, 0);
//            Vector3 bottomLeft = new Vector3(-1, -1, 0);
//            Vector3 bottomRight = new Vector3(1, -1, 0);
//
//            RaycastHit[] hits = Physics.SphereCastAll(transform.position + transform.forward * sphereCastRadius, sphereCastRadius, transform.forward, 30f);//entity.engineSystem.Speed * 1.5f);
//            for (int i = 0; i < hits.Length; i++) {
//                if (hits[i].transform == sensorSystem.Target) continue;
//                if (hits[i].transform == transform) continue;
//
//                var hitEntity = hits[i].transform.GetComponentInParent<Entity>();
//                Debug.DrawLine(transform.position, hits[i].point, Color.yellow);
//
//                var contactPoint = hits[i].point;
//                var toContactPoint = contactPoint - entity.transform.position;
//                int casts = (int)hitEntity.size;
//                castHits.Add(new CastHit(contactPoint, toContactPoint.normalized, toContactPoint.sqrMagnitude, Color.red, true));
//                for (int j = 1; j <= casts; j++) {
//                    float offset = (int)hitEntity.size * j;
//                    if (j % 2 == 0) {
//                        ProcessContactDirection(hitEntity, Vector3.up, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, Vector3.down, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, Vector3.left, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, Vector3.right, contactPoint, offset);
//                    } else {
//                        ProcessContactDirection(hitEntity, topLeft, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, topRight, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, bottomLeft, contactPoint, offset);
//                        ProcessContactDirection(hitEntity, bottomRight, contactPoint, offset);
//                    }
//                }
//            }
//        }
//
//        private void ProcessContactDirection(Entity hitEntity, Vector3 offsetPoint, Vector3 contactPoint, float offset) {
//            Vector3 rotatedPoint = transform.rotation * (offsetPoint * offset);
//            Vector3 toOffsetPoint = ((rotatedPoint + contactPoint) - transform.position).normalized;
//            //todo replace with raycast
//            var intersection = hitEntity.GetRayIntersectionPoint(transform.position, toOffsetPoint);
//            if (intersection != null) {
//                float squareDistance = ((Vector3)intersection - transform.position).sqrMagnitude;
//                castHits.Add(new CastHit((Vector3)intersection, toOffsetPoint, squareDistance, Color.white));
//                //    if (drawCastLines) {transform.position + toOffsetPoint * 10f
//                Debug.DrawLine(transform.position, (Vector3)intersection, Color.green);
//                //  }
//            }
//        }
//
//        private void SaturateTarget() {
//            DotContainer dots = sensorSystem.RightUpForwardDotToVector(entity.controls.destination);
//            int index = GetIndex(dots.right, dots.up, resolution);
//            Saturate(index, 1f, 0.75f);
//            //todo -- add bonus strength to all nodes in the direction of our angular velocity trend
//        }
//
//        //i need enough context to make a speed up / slow down / use afterburner decision
//        private void SaturateDanger() {
//            float speed = entity.engineSystem.Speed;
//            speed *= speed;
//            speed *= 1.5f;
//            if (speed == 0) speed = 1;
//            foreach (CastHit hit in castHits) {
//                DotContainer dots = sensorSystem.RightUpForwardDotToVector(hit.direction);
//                int index = GetIndex(dots.right, dots.up, resolution);
//                float strength = 1f;
//                float dropOff = 0.5f;
//                if (hit.primary) {
//                    strength = 1f;
//                    dropOff = 0.75f;
//                }
//                float distanceScale = 1 - (hit.squareDistance / (speed));
//                if (distanceScale > 0.9f) distanceScale = 1.25f;
//                float forwardDotScale = dots.forward;
//                if (dots.forward >= 0.95f) forwardDotScale = 2f;
//                strength *= forwardDotScale;
//                strength *= distanceScale;
//                if (strength > 0) strength = -strength;
//                Saturate(index, strength, dropOff);
//            }
//        }
//
//        private void Saturate(int index, float strength, float dropOff) {
//            SteerNode origin = nodes[index];
//            for (int i = 0; i < nodes.Length; i++) {
//                SteerNode node = nodes[i];
//                if (i == index) {
//                    origin.score += strength;
//                } else {
//                    int xDiff = Math.Abs(origin.x - node.x);
//                    int yDiff = Math.Abs(origin.y - node.y);
//
//                    float xScale = Mathf.Pow(dropOff, xDiff);
//                    if (xScale == 0) xScale = 1;
//                    node.score += strength * xScale * 0.5f;
//
//                    float yScale = Mathf.Pow(dropOff, yDiff);
//                    if (yScale == 0) yScale = 1;
//                    node.score += strength * yScale * 0.5f;
//                }
//            }
//        }
//
//        public static SteerNode[] CreateNodeMap(int resolution) {
//            if (resolution <= 2) resolution = 3;
//            if (resolution % 2 == 0) resolution += 1;
//            var nodes = new SteerNode[resolution * resolution];
//            for (int i = 0; i < resolution * resolution; i++) {
//                nodes[i] = new SteerNode(i, i % resolution, i / resolution);
//            }
//            return nodes;
//        }
//
//        public static int GetIndex(float x, float y, int resolution) {
//            int xIndex, yIndex;
//
//            if (x < 0.0001f && x > -0.0001f) x = 0;
//            if (y < 0.0001f && y > -0.0001f) y = 0;
//
//            if (x == 1) {
//                xIndex = resolution - 1;
//            } else {
//                float xPercent = (x + 1) / 2f;
//                xIndex = (int)((resolution) * xPercent);
//            }
//            if (y == 1) {
//                yIndex = resolution - 1;
//            } else {
//                float yPercent = (y + 1) / 2f;
//                yIndex = (int)((resolution) * yPercent);
//
//            }
//
//            return yIndex * resolution + xIndex;
//        }
//
//    }
//
//    public class SteerNode {
//        public float score;
//        public int x;
//        public int y;
//        public int index;
//
//        public SteerNode(int index, int x, int y) {
//            this.index = index;
//            this.x = x;
//            this.y = y;
//            score = 0f;
//        }
//    }
//
