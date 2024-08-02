using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using static FGSOfflineCallBreak.CallBreakRemoteConfigClass;

namespace FGSOfflineCallBreak
{
    [CreateAssetMenu(fileName = "ManagerData", menuName = "ManagerData/BotsDetails", order = 1)]
    [Serializable]
    public class ManageData : ScriptableObject
    {
        public List<BotDetails> allBotDetails;
    }

    [Serializable]
    public class UserDetails
    {
        public string userName;
        public string userId;
        public float userChips;
        public float userKeys;
        public int userAvatarIndex;
        public int level = 1;
        public float levelProgress;
        public int removeAds;
        public UserGameDetails userGameDetails;
    }

    [Serializable]
    public class UserGameDetails
    {
        public int GamePlayed;
        public int GameWon;
        public int GameLoss;
    }

    [Serializable]
    public class BotDetails
    {
        public string userName;
        public string userId;
        public long userChips;
        public long userKeys;
        public int userAvatarIndex;
    }

    public sealed class CallBreakGameManager : MonoBehaviour
    {
        [SerializeField]
        public UserDetails selfUserDetails;
        public static Sprite profilePicture;

        public static bool isInGamePlay;

        public static Action UpdateUserDetails;

        public GenerateTheBots generateTheBots;
        //public ManageData manageData;
        //public SpriteData spriteData;

        public float lobbyAmount;

        private void Awake()
        {
            if (instance == null) instance = this;

            StopAllCoroutines();

            Application.targetFrameRate = 70;
            Input.multiTouchEnabled = false;
            Time.timeScale = 1f;

            float sizeOfInt = sizeof(float);

            // Print the size
            Debug.Log("Size of integer: " + sizeOfInt + " bytes");
        }

        internal CallBreakCardController currentCard;
        internal List<CallBreakCardController> allTableCards = new List<CallBreakCardController>();


        public static Action ScoreBoardDataEvent;

        public static CallBreakGameManager instance;

        public static int currentPlayerIndex;
        [Header("CURRENT ROUND")]
        public int currentRound = 0, totalRound;
        public static int currentLobbyAmount;

        private const float WinCardScale = 1.5f;
        private float delayTime = 0.1f;

        [Space(5)]

        public List<Sprite> allProfileSprite = new List<Sprite>();
        public List<GameObject> particles = new List<GameObject>();
        public ParticleSystem particlesOfHukum;

        public int clubCardCounter;

        public CallBreakGamePlayController gamePlayController;

        internal IEnumerator NextUserTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % 4;

            Debug.Log(" Next PlayerTurn ====> " + currentPlayerIndex);

            if (allTableCards.Count == 4)
            {
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);

                List<CallBreakCardController> spadeCards = new();
                List<CallBreakCardController> otherTypeCards = new();

                for (int i = 0; i < allTableCards.Count; i++)
                {
                    if (allTableCards[i].cardDetail.cardType == CardType.Spade)
                    {
                        spadeCards.Add(allTableCards[i]);
                        Debug.Log(" Spade Add " + allTableCards[i].name);
                    }
                }

                allTableCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));
                spadeCards.Sort((a, b) => b.cardDetail.value.CompareTo(a.cardDetail.value));

                for (int i = 0; i < allTableCards.Count; i++)
                {
                    Debug.LogWarning(allTableCards[i].cardDetail.cardType + " ===== " + currentCard.cardDetail.cardType);

                    if (allTableCards[i].cardDetail.cardType != currentCard.cardDetail.cardType)
                    {
                        otherTypeCards.Add(allTableCards[i]);
                        Debug.Log(allTableCards[i].name);
                    }
                }

                for (int i = 0; i < otherTypeCards.Count; i++)
                {
                    if (allTableCards.Contains(otherTypeCards[i])) allTableCards.Remove(otherTypeCards[i]);
                }

                CallBreakCardController winCard = (spadeCards.Count > 0) ? spadeCards[0] : allTableCards[0];
                yield return new WaitForSeconds(0.2f);

                Debug.Log(" WINCARD == " + winCard.name);
                Tweener scaleAnimation = winCard.transform.DOScale(new Vector3(WinCardScale, WinCardScale, WinCardScale), 0.2f).SetEase(Ease.Linear).SetAutoKill(false);
                Transform playerData = winCard.cardThrowParent;

                //for (int i = 0; i < allTableCards.Count; i++)
                //    allTableCards[i].cardThrowParent = playerData;

                yield return new WaitForSeconds(0.3f);

                scaleAnimation.PlayBackwards();

                yield return new WaitForSeconds(0.5f);

                for (int i = 0; i < otherTypeCards.Count; i++)
                {
                    allTableCards.Add(otherTypeCards[i]);
                }

                foreach (var item in particles)
                {
                    item.SetActive(true);
                    item.transform.localPosition = Vector3.zero;
                }

                foreach (var item in allTableCards)
                {
                    item.transform.DOScale(Vector3.zero, .3f).SetEase(Ease.Linear);
                    item.transform.DOMove(gamePlayController.allPlayer[winCard.playerIndex].myCardPos.position, .3f).SetEase(Ease.Linear);
                }

                foreach (var item in particles)
                {
                    item.transform.DOMove(gamePlayController.allPlayer[winCard.playerIndex].myCardPos.position, .3f).SetEase(Ease.Linear);
                }


                yield return new WaitForSeconds(0.5f);

                foreach (var item in particles)
                {
                    item.SetActive(false);
                }

                gamePlayController.allPlayer[winCard.playerIndex].currentBidScore++;
                gamePlayController.allPlayer[winCard.playerIndex].SetMyBid();
                TurnDataReset();
                currentPlayerIndex = gamePlayController.allPlayer.IndexOf(gamePlayController.allPlayer[winCard.playerIndex]);

                WinnerTurn();

                if (gamePlayController.allPlayer.TrueForAll(i => i.myCards.Count == 0))
                {
                    ScoreBoardDataEvent();
                    //Invoke(nameof(), 1.5f);
                    RestartGame();
                }
            }
            else
            {
                //float delayTime = UnityEngine.Random.Range(0.1f, 0.4f);
                if (currentPlayerIndex == 0) delayTime = 0;
                else delayTime = 0.3f;
                yield return new WaitForSeconds(delayTime);

                if (currentPlayerIndex == 0)
                {
                    gamePlayController.allPlayer[0].HighLightCard();
                    ThrowSelfPlayerCardAuto();
                }
                else
                {
                    gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();//ThrowCard()
                }
            }
        }



        internal void WinnerTurn()
        {
            if (currentCard == null)
            {
                int highCard = gamePlayController.allPlayer[currentPlayerIndex].myCards.Count;

                if (highCard != 0) currentCard = gamePlayController.allPlayer[currentPlayerIndex].myCards[UnityEngine.Random.Range(0, highCard)].GetComponent<CallBreakCardController>();

                Debug.Log("====>" + currentCard.name);
            }
            if (currentPlayerIndex != 0)
            {
                Debug.LogError(" WINNER TURN ");
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();// ThrowCard();
            }
            else
            {
                Debug.LogError(" YOUR TURN ");
                ThrowSelfPlayerCardAuto();
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);
            }
        }

        public void RestartGame()
        {
            for (int i = 0; i < gamePlayController.allPlayer.Count; i++)
                gamePlayController.allPlayer[i].finalScore = gamePlayController.allPlayer[i].roundScore.Sum();

            arrageUserInOrderToRoundHighScore = new List<CallBreakUserController>(gamePlayController.allPlayer);
            arrageUserInOrderToFinalHighScore = new List<CallBreakUserController>(gamePlayController.allPlayer);

            arrageUserInOrderToRoundHighScore = arrageUserInOrderToRoundHighScore.OrderByDescending(player => player.roundScore[currentRound - 1]).ToList();
            arrageUserInOrderToFinalHighScore = arrageUserInOrderToFinalHighScore.OrderByDescending(player => player.finalScore).ToList();

            BidCalculationTeam01(0, 2);
            BidCalculationTeam01(1, 3);

            if (teamScoreboard.teamScoreboard01.totalPoints >= 500 || teamScoreboard.teamScoreboard02.totalPoints >= 500)
            {
                clubCardCounter = 0;
                currentRound = 1;
                int winnerTeamIndex = 0;
                if (teamScoreboard.teamScoreboard01.totalPoints >= 500)
                {
                    winnerTeamIndex = 0;
                }
                else if (teamScoreboard.teamScoreboard02.totalPoints >= 500)
                {
                    winnerTeamIndex = 1;
                }

                CallBreakUIManager.Instance.winnerLoserController.OpenWinnerAndLosserScreen(winnerTeamIndex);
            }
            else
            {
                //CallBreakUIManager.Instance.scoreBoardController.OpenScreen(arrageUserInOrderToRoundHighScore[0].staticSeatIndex);
                CallBreakUIManager.Instance.scoreBoardController.OpenSpadesScoreboard(teamScoreboard);
            }
            for (int i = 0; i < gamePlayController.allPlayer.Count; i++)
                gamePlayController.allPlayer[i].ResetPlayerData();
        }

        public List<CallBreakUserController> arrageUserInOrderToRoundHighScore;
        public List<CallBreakUserController> arrageUserInOrderToFinalHighScore;

        private void Start()
        {
            BidCalculationTeam01(0, 1);
        }

        [Header("=======")]
        public SpadesScoreboard teamScoreboard;


        public void BidCalculationTeam01(int teamFirstPlayerFirst, int teamFirstPlayerSecond)
        {
            TeamScoreboard tempTeamScoreboard = new TeamScoreboard();

            int teamOneTotalBid = gamePlayController.allPlayer[teamFirstPlayerFirst].totalBid + gamePlayController.allPlayer[teamFirstPlayerSecond].totalBid;
            int teamOneCurrentBidScore = gamePlayController.allPlayer[teamFirstPlayerFirst].currentBidScore + gamePlayController.allPlayer[teamFirstPlayerSecond].currentBidScore;


            Debug.Log($" team 01 combineBidText  {teamOneTotalBid} ");
            Debug.Log($" team 01 total trick taken   {teamOneCurrentBidScore} ");

            tempTeamScoreboard.combinedBid = teamOneTotalBid;
            tempTeamScoreboard.tricksTaken = teamOneCurrentBidScore;

            int bags = 0;
            if (teamOneCurrentBidScore > teamOneTotalBid)
                bags = Mathf.Abs(teamOneTotalBid - teamOneCurrentBidScore);

            tempTeamScoreboard.bags = bags;
            Debug.Log($"TEAM 01 extrahand {bags} ");

            if (teamFirstPlayerFirst == 0)
            {
                tempTeamScoreboard.bagsFromLastRound += teamScoreboard.teamScoreboard01.totalBags;
                tempTeamScoreboard.totalBags = tempTeamScoreboard.bagsFromLastRound + bags;
            }
            else if (teamFirstPlayerFirst == 1)
            {
                tempTeamScoreboard.bagsFromLastRound += teamScoreboard.teamScoreboard02.totalBags;
                tempTeamScoreboard.totalBags = tempTeamScoreboard.bagsFromLastRound + bags;
            }


            int successBID01 = 0, failedBID01 = 0;

            if (teamOneCurrentBidScore >= teamOneTotalBid)
            {
                successBID01 = teamOneTotalBid * 10;
                tempTeamScoreboard.successfulBid = successBID01;
                tempTeamScoreboard.bagScore = bags;
            }
            else
            {
                failedBID01 = teamOneTotalBid * -10;
                tempTeamScoreboard.failedBid = failedBID01;
                Debug.Log($"TEAM 01 Failed BID USER ONE {failedBID01} ");
            }
            if (tempTeamScoreboard.totalBags >= 10)
            {
                tempTeamScoreboard.bagPenalty = -100;
                tempTeamScoreboard.totalBags -= 10;
            }

            if (gamePlayController.allPlayer[teamFirstPlayerFirst].totalBid == 0 && gamePlayController.allPlayer[teamFirstPlayerFirst].currentBidScore > 0)
                tempTeamScoreboard.failedNilBid -= 100;

            if (gamePlayController.allPlayer[teamFirstPlayerSecond].totalBid == 0 && gamePlayController.allPlayer[teamFirstPlayerSecond].currentBidScore > 0)
                tempTeamScoreboard.failedNilBid -= 100;

            tempTeamScoreboard.pointsThisRound = tempTeamScoreboard.successfulBid + tempTeamScoreboard.bagScore + tempTeamScoreboard.failedNilBid + tempTeamScoreboard.failedBid;


            if (teamFirstPlayerFirst == 0)
            {
                tempTeamScoreboard.previousPoints += teamScoreboard.teamScoreboard01.totalPoints;
            }
            else if (teamFirstPlayerFirst == 1)
            {
                tempTeamScoreboard.previousPoints += teamScoreboard.teamScoreboard02.totalPoints;
            }

            tempTeamScoreboard.totalPoints = tempTeamScoreboard.previousPoints + tempTeamScoreboard.pointsThisRound;

            if (teamFirstPlayerFirst == 0)
            {
                teamScoreboard.teamScoreboard01 = tempTeamScoreboard;
            }
            else if (teamFirstPlayerFirst == 1)
            {
                teamScoreboard.teamScoreboard02 = tempTeamScoreboard;
            }
        }


        public void StartNewRoundAfterScoreboard()
        {
            CallBreakUIManager.Instance.gamePlayController.UpdateTheRoundText();
            StartCoroutine(CallBreakCardAnimation.instance.SetAndStartGamePlay(1f));
        }


        public void ThrowSelfPlayerCardAuto()
        {
            if (gamePlayController.allPlayer[0].myCards.Count == 0) return;

            if (gamePlayController.allPlayer[0].myCards.Count == 1)
            {
                gamePlayController.allPlayer[0].ActiveAllSelfPlayerCard(false);
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();// ThrowCard();
            }
            else
            {
                gamePlayController.allPlayer[currentPlayerIndex].turnTimer.SelfUserTimer();
            }
        }

        public void TurnDataReset()
        {
            allTableCards.ForEach(c => c.gameObject.SetActive(false));
            allTableCards.ForEach(c => Destroy(c.gameObject));
            gamePlayController.allPlayer.ForEach(c => c.isMyTurnComplete = false);
            allTableCards.Clear();
        }

        public static Action<CallBreakRemoteConfig> updateTheConfigData;

        public CallBreakRemoteConfig UpdatedConfigData()
        {

            return new CallBreakRemoteConfig();
        }


    }
}
