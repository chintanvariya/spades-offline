using GoogleMobileAds.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FGSOfflineCallBreak
{
    public class CallBreakScoreBoardController : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI currentRoundText;

        public List<TMPro.TextMeshProUGUI> playerOneRoundsText;
        public List<TMPro.TextMeshProUGUI> playerTwoRoundsText;
        public List<TMPro.TextMeshProUGUI> playerThreeRoundsText;
        public List<TMPro.TextMeshProUGUI> playerFourRoundsText;

        public List<TMPro.TextMeshProUGUI> allPlayerName;
        public List<Image> allPlayerProfilePicture;
        public List<GameObject> allPlayerCrown;

        public GameObject closeButton;
        public GameObject continueButton;

        public List<TMPro.TextMeshProUGUI> team01Text;
        public List<TMPro.TextMeshProUGUI> team02Text;


        public void ResetScoreBoardData()
        {
            ResetDataOfText(playerOneRoundsText);
            ResetDataOfText(playerTwoRoundsText);
            ResetDataOfText(playerThreeRoundsText);
            ResetDataOfText(playerFourRoundsText);

            for (int i = 0; i < allPlayerCrown.Count; i++)
                allPlayerCrown[i].SetActive(false);

        }

        public void ResetDataOfText(List<TMPro.TextMeshProUGUI> currentList)
        {
            for (int i = 0; i < playerOneRoundsText.Count; i++)
            {
                currentList[i].color = Color.white;
                currentList[i].text = "0";
            }
        }

        public void OpenCurrentRoundScoreBoard()
        {
            closeButton.SetActive(true);
            continueButton.SetActive(false);
            gameObject.SetActive(true);
        }

        public GameObject bagPenaltyRow;
        public GameObject failedNilBidRow;

        public void OpenSpadesScoreboard(SpadesScoreboard spadesScoreboard)
        {
            for (int i = 0; i < allPlayerCrown.Count; i++)
                allPlayerCrown[i].SetActive(false);

            allPlayerName[0].text = CallBreakGameManager.instance.selfUserDetails.userName + " & " + CallBreakUIManager.Instance.gamePlayController.allPlayer[2].botDetails.userName;
            allPlayerName[1].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].botDetails.userName + " & " + CallBreakUIManager.Instance.gamePlayController.allPlayer[3].botDetails.userName;

            allPlayerProfilePicture[0].sprite = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];
            allPlayerProfilePicture[1].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].profilePicture.sprite;
            allPlayerProfilePicture[2].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].profilePicture.sprite;
            allPlayerProfilePicture[3].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].profilePicture.sprite;

            if (spadesScoreboard.teamScoreboard01.pointsThisRound > spadesScoreboard.teamScoreboard02.pointsThisRound)
                allPlayerCrown[0].SetActive(true);
            else
                allPlayerCrown[1].SetActive(true);

            if (spadesScoreboard.teamScoreboard01.failedNilBid == 0 && spadesScoreboard.teamScoreboard01.failedNilBid == 0)
                failedNilBidRow.SetActive(false);
            else
                failedNilBidRow.SetActive(true);

            if (spadesScoreboard.teamScoreboard01.bagPenalty == 0 && spadesScoreboard.teamScoreboard01.bagPenalty == 0)
                bagPenaltyRow.SetActive(false);
            else
                bagPenaltyRow.SetActive(true);

            team01Text[0].text = $"{spadesScoreboard.teamScoreboard01.combinedBid}";
            team01Text[1].text = $"{spadesScoreboard.teamScoreboard01.tricksTaken}";
            team01Text[2].text = $"{spadesScoreboard.teamScoreboard01.bags}";
            team01Text[3].text = $"{spadesScoreboard.teamScoreboard01.bagsFromLastRound}";
            team01Text[4].text = $"{spadesScoreboard.teamScoreboard01.totalBags}";

            team01Text[5].text = $"{spadesScoreboard.teamScoreboard01.successfulBid}";
            team01Text[6].text = $"{spadesScoreboard.teamScoreboard01.failedBid}";
            team01Text[7].text = $"{spadesScoreboard.teamScoreboard01.failedNilBid}";
            team01Text[8].text = $"{spadesScoreboard.teamScoreboard01.bagScore}";
            team01Text[9].text = $"{spadesScoreboard.teamScoreboard01.bagPenalty}";
            team01Text[10].text = $"{spadesScoreboard.teamScoreboard01.pointsThisRound}";
            team01Text[11].text = $"{spadesScoreboard.teamScoreboard01.previousPoints}";
            team01Text[12].text = $"{spadesScoreboard.teamScoreboard01.totalPoints}";

            team02Text[0].text = $"{spadesScoreboard.teamScoreboard02.combinedBid}";
            team02Text[1].text = $"{spadesScoreboard.teamScoreboard02.tricksTaken}";
            team02Text[2].text = $"{spadesScoreboard.teamScoreboard02.bags}";
            team02Text[3].text = $"{spadesScoreboard.teamScoreboard02.bagsFromLastRound}";
            team02Text[4].text = $"{spadesScoreboard.teamScoreboard02.totalBags}";

            team02Text[5].text = $"{spadesScoreboard.teamScoreboard02.successfulBid}";
            team02Text[6].text = $"{spadesScoreboard.teamScoreboard02.failedBid}";
            team02Text[7].text = $"{spadesScoreboard.teamScoreboard02.failedNilBid}";
            team02Text[8].text = $"{spadesScoreboard.teamScoreboard02.bagScore}";
            team02Text[9].text = $"{spadesScoreboard.teamScoreboard02.bagPenalty}";
            team02Text[10].text = $"{spadesScoreboard.teamScoreboard02.pointsThisRound}";
            team02Text[11].text = $"{spadesScoreboard.teamScoreboard02.previousPoints}";
            team02Text[12].text = $"{spadesScoreboard.teamScoreboard02.totalPoints}";

            continueButton.SetActive(true);
            gameObject.SetActive(true);
        }

        public void OpenScreen(int winnerIndex)
        {
            for (int i = 0; i < allPlayerCrown.Count; i++)
                allPlayerCrown[i].SetActive(false);

            allPlayerCrown[winnerIndex].SetActive(true);
            switch (winnerIndex)
            {
                case 0:
                    playerOneRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerOneRoundsText[5].color = Color.green;
                    break;
                case 1:
                    playerTwoRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerTwoRoundsText[5].color = Color.green;
                    break;
                case 2:
                    playerThreeRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerThreeRoundsText[5].color = Color.green;
                    break;
                case 3:
                    playerFourRoundsText[CallBreakGameManager.instance.currentRound - 1].color = Color.green;
                    playerFourRoundsText[5].color = Color.green;
                    break;
            }

            currentRoundText.text = $"Round : {CallBreakGameManager.instance.currentRound}";

            allPlayerName[0].text = CallBreakGameManager.instance.selfUserDetails.userName;
            allPlayerProfilePicture[0].sprite = CallBreakGameManager.instance.allProfileSprite[CallBreakGameManager.instance.selfUserDetails.userAvatarIndex];

            for (int i = 0; i < playerOneRoundsText.Count - 1; i++)
                playerOneRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].roundScore[i].ToString();

            for (int i = 0; i < playerTwoRoundsText.Count - 1; i++)
            {
                playerTwoRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].roundScore[i].ToString();
                allPlayerName[1].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].botDetails.userName;
                allPlayerProfilePicture[1].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].profilePicture.sprite;
            }
            for (int i = 0; i < playerThreeRoundsText.Count - 1; i++)
            {
                playerThreeRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].roundScore[i].ToString();
                allPlayerName[2].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].botDetails.userName;
                allPlayerProfilePicture[2].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].profilePicture.sprite;
            }
            for (int i = 0; i < playerFourRoundsText.Count - 1; i++)
            {
                playerFourRoundsText[i].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].roundScore[i].ToString();
                allPlayerName[3].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].botDetails.userName;
                allPlayerProfilePicture[3].sprite = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].profilePicture.sprite;
            }

            playerOneRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[0].roundScore.Sum().ToString("F1");
            playerTwoRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[1].roundScore.Sum().ToString("F1");
            playerThreeRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[2].roundScore.Sum().ToString("F1");
            playerFourRoundsText[5].text = CallBreakUIManager.Instance.gamePlayController.allPlayer[3].roundScore.Sum().ToString("F1");

            continueButton.SetActive(true);
            gameObject.SetActive(true);
        }

        public void OnButtonClicked(string buttonName)
        {
            switch (buttonName)
            {
                case "Continue":
                    //if (CallBreakConstants.callBreakRemoteConfig.adsDetails.isShowInterstitialAdsOnScoreBoard)
                    //{
                    //    CallBreakUIManager.Instance.preLoaderController.OpenPreloader();
                    //    GoogleMobileAds.Sample.InterstitialAdController.ShowInterstitialAd();
                    //}
                    //else
                    //{
                    OnAdFullScreenContentClosedHandler();
                    //}
                    break;

                default:
                    break;
            }
        }

        private void OnEnable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed += OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed += OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady += OnInterstitialAdNotReady;
        }
        private void OnDisable()
        {
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentClosed -= OnAdFullScreenContentClosedHandler;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdFullScreenContentFailed -= OnRewardedAdFullScreenContentFailed;
            GoogleMobileAds.Sample.InterstitialAdController.OnInterstitialAdNotReady -= OnInterstitialAdNotReady;
        }

        private void OnInterstitialAdNotReady()
        {
            //CallBreakUIManager.Instance.toolTipsController.OpenToolTips("AdsIsNotReady", "Ad is not ready yet !!", "");
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        private void OnRewardedAdFullScreenContentFailed(AdError error)
        {
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        private void OnAdFullScreenContentClosedHandler()
        {
            Debug.Log("OnAdFullScreenContentClosedHandler || ");
            CloseScreen();
            CallBreakUIManager.Instance.preLoaderController.ClosePreloader();
            CallBreakGameManager.instance.StartNewRoundAfterScoreboard();
        }

        public void CloseScreen()
        {
            continueButton.SetActive(false);
            closeButton.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
[System.Serializable]
public class SpadesScoreboard
{
    public TeamScoreboard teamScoreboard01;
    public TeamScoreboard teamScoreboard02;
}
[System.Serializable]
public class TeamScoreboard
{
    public int combinedBid;
    public int tricksTaken;
    public int bags;
    public int bagsFromLastRound;
    public int totalBags;
    public int successfulBid;
    public int failedBid;
    public int failedNilBid;
    public int bagScore;
    public int bagPenalty;
    public int pointsThisRound;
    public int previousPoints;
    public int totalPoints;
}


//public int combinedBidTeam01;
//public int combinedBidTeam02;

//public int tricksTakenTeam01;
//public int tricksTakenTeam02;

//public int bagsTeam01;
//public int bagsTeam02;

//public int bagsFromLastRoundTeam01;
//public int bagsFromLastRoundTeam02;

//public int totalBagsTeam01;
//public int totalBagsTeam02;

//public int successfulBidTeam01;
//public int successfulBidTeam02;

//public int failedBidTeam01;
//public int failedBidTeam02;

//public int bagScoreTeam01;
//public int bagScoreTeam02;

//public int pointsThisRoundTeam01;
//public int pointsThisRoundTeam02;

//public int previousPointsTeam01;
//public int previousPointsTeam02;

//public int totalPointsTeam01;
//public int totalPointsTeam02;