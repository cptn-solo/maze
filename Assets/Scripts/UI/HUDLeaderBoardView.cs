using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HUDLeaderBoardView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup layoutGroup;
        [SerializeField] private GameObject itemPrefab;
        
        private RectTransform rectTransform;
        private RectTransform listRectTransform;
        private CanvasScaler canvasScaler;
        private bool prevIsPortrait;
        private readonly Dictionary<string, LeaderBoardItemView> leaders = new();

        private const string playerId = "Player";
        private const string enemyId = "NPCs";

        private UnitInfo playerScoreInfo =
            new(playerId, Color.green, 0, Color.green, 0, 0, Color.green);
        private UnitInfo enemyScoreInfo =
            new(enemyId, Color.yellow, 0, Color.red, 0, 0, Color.yellow);

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            listRectTransform = layoutGroup.GetComponent<RectTransform>();
            canvasScaler = GetComponent<CanvasScaler>();

            AddPlayer(playerId);
            UpdatePlayer(playerId, playerScoreInfo, true);

            AddPlayer(enemyId);
            UpdatePlayer(enemyId, enemyScoreInfo, false);
        }

        internal void AddPlayer(string id)
        {
            var playerView = Instantiate(itemPrefab).GetComponent<LeaderBoardItemView>();
            var listRectTransform = layoutGroup.GetComponent<RectTransform>();
            playerView.Attach(listRectTransform, id, rectTransform.localScale.x);
            playerView.SetInfo(new UnitInfo(), false);
            leaders.Add(id, playerView);
        }
        internal void RemovePlayer(string id)
        {
            if (leaders.TryGetValue(id, out var leaderView) && !leaderView.gameObject.IsDestroyed())
            {
                leaderView.Detach();
                Destroy(leaderView.gameObject);
            }
        }

        internal void UpdatePlayer(int score)
        {
            playerScoreInfo.Score = score;
            UpdatePlayer(playerId, playerScoreInfo, true);
        }

        internal void UpdateEnemy(int score)
        {
            enemyScoreInfo.Score = score;
            UpdatePlayer(enemyId, enemyScoreInfo, false);
        }

        private void UpdatePlayer(string id, UnitInfo playerInfo, bool localPlayer)
        {
            if (leaders.TryGetValue(id, out var leaderView) && !leaderView.gameObject.IsDestroyed())
            {
                leaderView.SetInfo(playerInfo, localPlayer);

                var sorted = leaders.Values.OrderByDescending(x => x.Info.Score).ToArray();

                for (int idx = 0; idx < sorted.Count(); idx++)
                    sorted[idx].SetListPosition(idx);
            }
        }

        private void OnEnable()
        {
            //StartCoroutine(CheckScreenOrientation());
        }

        private void OnDisable()
        {
            //StopCoroutine(CheckScreenOrientation());
        }

        private IEnumerator CheckScreenOrientation()
        {
            ApplyOrientation(CheckIfPortrait());

            while (true)
            {
                yield return new WaitForSeconds(1.0f);
            
                bool isPortrait = CheckIfPortrait();

                if (prevIsPortrait != isPortrait)
                    ApplyOrientation(isPortrait);

                prevIsPortrait = isPortrait;

            }

            static bool CheckIfPortrait()
            {
                return Screen.orientation switch
                {
                    ScreenOrientation.Portrait => true,
                    ScreenOrientation.PortraitUpsideDown => true,
                    _ => false,
                };
            }

            void ApplyOrientation(bool isPortrait)
            {
                var pos = listRectTransform.position;
                pos.y = isPortrait ? pos.y * 2 : pos.y / 2;
                pos.y *= rectTransform.localScale.y;
                listRectTransform.position = pos;
            }
        }

    }
}