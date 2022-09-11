using UnityEngine;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private GameObject[] buildingPrefabs;
        [SerializeField] private GameObject[] collectablePrefabs;

        [SerializeField] private Camera sceneCamera;

        [SerializeField] private float cameraAngle = -35.0f;
        [SerializeField] private float cameraDistance = 2.5f;
        [SerializeField] private float camSpeed = 4.0f;

        private Building building;
        private Player player;

        // Start is called before the first frame update
        void Start()
        {
            building = Instantiate(buildingPrefabs[0]).GetComponent<Building>();
            player = Instantiate(playerPrefab).GetComponent<Player>();

            PositionPlayer(player, building);
            PositionCamera(player, building);
        }

        private void PositionCamera(Player player, Building building)
        {
            var buildingFloorY = Vector3.zero + Vector3.up * player.transform.position.y;
            var buildingToPlayer = Vector3.Distance(player.transform.position, buildingFloorY);

            var yOffset = Mathf.Tan(cameraAngle * Mathf.Deg2Rad) * buildingToPlayer;

            var cameraRay = new Ray(
                buildingFloorY,
                (player.transform.position + player.transform.up * yOffset - buildingFloorY).normalized);
            
            var camCurrent = sceneCamera.transform.position;

            var camPosition = player.transform.position + cameraRay.direction * cameraDistance;
            var camDirection = player.transform.position - camPosition;

            var camStep = (camPosition - camCurrent) * camSpeed * Time.deltaTime;

            sceneCamera.transform.SetPositionAndRotation(
                camCurrent + camStep, Quaternion.LookRotation(camDirection));
        }

        private void PositionPlayer(Player player, Building building)
        {
            var spIndex = UnityEngine.Random.Range(0, building.PlayerSpawnPoints.Length);
            var sp = building.PlayerSpawnPoints[spIndex].transform;

            player.transform.SetPositionAndRotation(sp.position, sp.rotation);
            //player.transform.parent = building.transform;

            player.Building = building;

            player.OnRespawned();

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {
            PositionCamera(player, building);

            if (player.transform.position.y < -10.0f)
                PositionPlayer(player, building);
        }
    }
}