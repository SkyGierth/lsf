using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lsf.Grading.Models;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Lsf.Grading.Services.Notifiers
{
    public class TelegramNotifier : INotifier
    {
        public record Config
        {
            public string TelegramBotAccessToken { get; set; }
            public string TelegramChatId { get; set; }
        }
        
        private readonly TelegramBotClient _botClient;
        private readonly ILogger _logger;
        private readonly string _telegramChatId;

        public TelegramNotifier(ILogger logger, Config config)
        {
            _botClient = new TelegramBotClient(config.TelegramBotAccessToken);
            _logger = logger;
            _telegramChatId = config.TelegramChatId;
        }

        public Task NotifyChange(IEnumerable<Degree> degrees)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("<b>New grades have been published</b>:");

            foreach (var degree in degrees)
            {
                foreach (var major in degree.GradingMajors)
                {
                    messageBuilder.AppendLine($"<i>[{degree.Id}] {degree.Name}: [{major.Id}] {major.Name}]</i>");

                    foreach (var grading in major.Gradings)
                    {
                        messageBuilder.Append($"{grading.Name}: ");
                        if (!float.IsNaN(grading.Grade))
                        {
                            messageBuilder.AppendFormat("{0:F1}", grading.Grade);
                            messageBuilder.Append(", ");
                        }

                        messageBuilder.AppendLine(grading.ExamState.ToString());
                    }
                }
            }

            var message = messageBuilder.ToString();

            return _botClient.SendTextMessageAsync(_telegramChatId, message, ParseMode.Html);
        }

        public Task NotifyError(string message)
        {
            return _botClient.SendTextMessageAsync(_telegramChatId, message);
        }
    }
}