using IntranetSearchBot.Models;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.TeamsFx.Conversation;
using Newtonsoft.Json;
using IntranetSearchBot.Interfaces;
using IntranetSearchBot.Services;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using AdaptiveCards;
using IntranetSearchBot.Helpers;

namespace IntranetSearchBot.Commands
{
    /// <summary>
    /// The <see cref="SearchCommandHandler"/> registers a pattern with the <see cref="ITeamsCommandHandler"/> and 
    /// responds with an Adaptive Card if the user types the <see cref="TriggerPatterns"/>.
    /// </summary>
    public class SearchCommandHandler : ITeamsCommandHandler
    {
        protected readonly IAdaptiveCardService _adaptiveCardService;
        protected readonly IFileService _fileService;
        protected readonly IGraphService _graphService;

        private readonly ILogger<SearchCommandHandler> _logger;
        private readonly string _adaptiveCardFilePath = Path.Combine(".", "Resources", "WelcomeCard.json");

        public IEnumerable<ITriggerPattern> TriggerPatterns => new List<ITriggerPattern>
        {
            // Used to trigger the command handler if the command text contains 'helloWorld'
            new RegExpTrigger("search")
        };

        public SearchCommandHandler(ILogger<SearchCommandHandler> logger, IAdaptiveCardService adaptiveCardService, IFileService fileService, IGraphService graphService)
        {
            _logger = logger;
            _adaptiveCardService = adaptiveCardService;
            _fileService = fileService;
            _graphService = graphService;
        }

        public async Task<ICommandResponse> HandleCommandAsync(ITurnContext turnContext, CommandMessage message, CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation($"Bot received message: {message.Text}");

            var members = new List<TeamsChannelAccount>();
            string continuationToken = null;

            do
            {
                // Gets a paginated list of members of one-on-one, group, or team conversation.
                var currentPage = await TeamsInfo.GetPagedMembersAsync(turnContext, 100, continuationToken, cancellationToken);
                continuationToken = currentPage.ContinuationToken;
                members.AddRange(currentPage.Members);
                // Pull in the data from the Microsoft Graph.
                _graphService.SetAccessToken(continuationToken);
            }
            while (continuationToken != null);

            // Get welcome adaptive card
            var card = _fileService.GetCard("WelcomeCard");
            var adaptiveCard = _adaptiveCardService.BindData(card, members[0]);
            var adaptiveCardAttachment = AttachmentHelper.GetAttachment(adaptiveCard);
            var activity = MessageFactory.Attachment(adaptiveCardAttachment);

            // send response
            return new ActivityCommandResponse(activity);
        }

    }
}
