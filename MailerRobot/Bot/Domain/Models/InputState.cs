namespace MailerRobot.Bot.Domain.Models;

public enum InputState
{
    Idle,
    WaitingForUrl,
    WaitingForIncludeKeywords,
    WaitingForExcludeKeywords,
    WaitingForInitialPull,
    WaitingForTitle,
    WaitingForSubscriptionToDelete,
    WaitingForTitleToDisable,
    WaitingForTitleToEnable,
    WaitingForLinkOnSite,
    WaitingForEmail
}