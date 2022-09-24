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

        private readonly Dictionary<string, LeaderBoardItemView> leaders = new();

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
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

        internal void UpdatePlayer(string id, UnitInfo playerInfo, bool localPlayer)
        {
            if (leaders.TryGetValue(id, out var leaderView) && !leaderView.gameObject.IsDestroyed())
            {
                leaderView.SetInfo(playerInfo, localPlayer);

                var sorted = leaders.Values.OrderByDescending(x => x.Info.Score).ToArray();

                for (int idx = 0; idx < sorted.Count(); idx++)
                    sorted[idx].SetListPosition(idx);
            }
        }
    }
}