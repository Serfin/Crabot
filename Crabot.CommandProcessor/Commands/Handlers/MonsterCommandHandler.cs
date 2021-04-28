using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Crabot.Commands.Dispatcher;
using Crabot.MessageExtensions;
using Crabot.Rest.Models;
using Crabot.Rest.RestClient;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Html;
using ScrapySharp.Network;

namespace Crabot.Commands.Commands.Handlers
{
    [Command("monster", 0)]
    [CommandUsage("?monster")]
    public class MonsterCommandHandler : ICommandHandler
    {
        private readonly IDiscordRestClient _discordRestClient;

        public MonsterCommandHandler(IDiscordRestClient discordRestClient)
        {
            _discordRestClient = discordRestClient;
        }

        public async Task HandleAsync(Command command)
        {
            try
            {
                var offerThreads = await GetWebsiteContentAsync();

                var embedMessage = new EbmedMessageBuilder()
                    .AddAuthor()
                    .AddMessageFields(FormatFields(offerThreads))
                    .Build();

                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    new Message { Embed = embedMessage });
            }
            catch (Exception ex)
            {
                await _discordRestClient.PostMessage(command.CalledFromChannel,
                    string.Format("Wystąpił bład - {0}", ex.Message));
            }
        }

        private async Task<List<OfferThread>> GetWebsiteContentAsync()
        {
            var browser = new ScrapingBrowser();
            var monsterQueryPage = await browser.NavigateToPageAsync(
                new Uri("https://www.pepper.pl/search?q=Monster%2BEnergy"));

            var selectedNodes = monsterQueryPage.Find("article", By.Class("thread")).ToList();

            var offerThreads = new List<OfferThread>();
            foreach (var selectedNode in selectedNodes)
            {

                if (selectedNode.InnerHtml.Contains("Zakończono")) // Improve condition
                {
                    continue;
                }

                offerThreads.Add(FillModelData(selectedNode));
            }

            return offerThreads;
        }

        private OfferThread FillModelData(HtmlNode selectedNode)
        {
            var result = new OfferThread();

            result.Temperature = selectedNode.CssSelect("span.vote-temp").FirstOrDefault()?.InnerText.Trim();
            result.Title = selectedNode.CssSelect("strong.thread-title").FirstOrDefault()?.InnerText.Trim();
            result.NewPrice = selectedNode.CssSelect("span.thread-price").FirstOrDefault()?.InnerText.Trim();
            result.Description = selectedNode.CssSelect("div.cept-description-container").FirstOrDefault()?.InnerText.Trim();
            result.Merchant = selectedNode.CssSelect("span.cept-merchant-name").FirstOrDefault()?.InnerText.Trim();
            result.Link = selectedNode.CssSelect("a.thread-link").FirstOrDefault()?.GetAttributeValue("href").Trim();

            return result;
        }
    
        private IEnumerable<EmbedField> FormatFields(List<OfferThread> offerThreads)
        {
            foreach (var offerThread in offerThreads)
            {
                yield return new EmbedField(
                    string.Format("{0} - [{1}] | {2}", offerThread.NewPrice, offerThread.Temperature, offerThread.Title),
                    string.Format("{0} \n\n {1} \n {2}", offerThread.Description, offerThread.Merchant, offerThread.Link), false);
            }
        }

        public Task<ValidationResult> ValidateCommandAsync(Command command)
        {
            return Task.FromResult(new ValidationResult(true));
        }
    }

    public class OfferThread
    {
        public string Temperature { get; set; }
        public string Title { get; set; }
        public string NewPrice { get; set; }
        public string OldPrice { get; set; }
        public string DiscountPercentage { get; set; }
        public string Description { get; set; }
        public string Merchant { get; set; }
        public string Link { get; set; }
    }
}
