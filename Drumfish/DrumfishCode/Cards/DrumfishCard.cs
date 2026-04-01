using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Drumfish.DrumfishCode.Character;
using Drumfish.DrumfishCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Drumfish.DrumfishCode.Cards;


[Pool(typeof(DrumfishCardPool))]
public abstract class DrumfishCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    protected string DefaultPortraitPath => $"PlaceholderDrumfish.png".CardImagePath();
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    public override string CustomPortraitPath => $"PlaceholderDrumfish.png".BigCardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => DefaultPortraitPath;
    public override string BetaPortraitPath => DefaultPortraitPath;

    public virtual Task OnFeedfire(PlayerChoiceContext choiceContext, Player player, decimal amount)
    {
        return Task.CompletedTask;
    }

    // Called when the player enters Heaty (上火)
    public virtual Task OnHeatyEnter(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }

    // Called when the player exits Heaty (上火)
    public virtual Task OnHeatyExit(PlayerChoiceContext choiceContext, Player player)
    {
        return Task.CompletedTask;
    }
}

    // //Image size:
    // //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    // //Full art: 606x852
    // public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    //
    // //Smaller variants of card images for efficiency:
    // //Smaller variant of fullart: 250x350
    // //Smaller variant of normalart: 250x190
    //
    // //Uses card_portraits/card_name.png as image path. These should be smaller images.
    // public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    // public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();