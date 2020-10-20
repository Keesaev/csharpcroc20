using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace CrocCSharpBot
{
    /// <summary>
    /// Основной модуль бота
    /// </summary>
    public class Bot
    {
        /// <summary>
        /// Клиент Telegram
        /// </summary>
        private TelegramBotClient client;

        /// <summary>
        /// Конструктор без параметров
        /// </summary>
        public Bot()
        {
            // Создание клиента для Telegram
            // Ссылка на бота: t.me/croc20_dz_bot
            client = new TelegramBotClient("1355340435:AAF80Wqsu8jZs6sZE1gtCUAh-yN6bBZBEvw");
            client.OnMessage += MessageProcessor;
        }

        /// <summary>
        /// Обработка входящего сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageProcessor(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            switch (e.Message.Type)
            {
                case Telegram.Bot.Types.Enums.MessageType.Document:     // документ
                    HandleDocumentAsync(e.Message);
                    client.SendTextMessageAsync(e.Message.Chat.Id, "Скачал твой документ");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Photo:        // изображение     
                    HandleImageAsync(e.Message);
                    client.SendTextMessageAsync(e.Message.Chat.Id, "Скачал твоё изображение");
                    break;
                case Telegram.Bot.Types.Enums.MessageType.Contact: // телефон
                    string phone = e.Message.Contact.PhoneNumber;
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Твой телефон: {phone}");
                    Console.WriteLine(phone);
                    break;

                case Telegram.Bot.Types.Enums.MessageType.Text: // текстовое сообщение
                    if (e.Message.Text.Substring(0, 1) == "/")  // команда
                    {
                        CommandProcessor(e.Message);
                    }
                    else
                    {
                        client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты сказал мне: {e.Message.Text}");
                        Console.WriteLine(e.Message.Text);
                    }
                    break;

                default:
                    client.SendTextMessageAsync(e.Message.Chat.Id, $"Ты прислал мне {e.Message.Type}, но я это пока не понимаю");
                    Console.WriteLine(e.Message.Type);
                    break;
            }
        }
        /// <summary>
        /// Обработка команды
        /// </summary>
        /// <param name="message"></param>
        private void CommandProcessor(Telegram.Bot.Types.Message message)
        {
            // Отрезаем первый символ (который должен быть '/')
            string command = message.Text.Substring(1).ToLower();

            switch (command)
            {
                case "start":
                    var button = new KeyboardButton("Поделись телефоном");
                    button.RequestContact = true;
                    var array = new KeyboardButton[] { button };
                    var reply = new ReplyKeyboardMarkup(array, true, true);
                    client.SendTextMessageAsync(message.Chat.Id, $"Привет, {message.Chat.FirstName}, скажи мне свой телефон", replyMarkup: reply);
                    break;

                default:
                    client.SendTextMessageAsync(message.Chat.Id, $"Я пока не понимаю команду {command}");
                    break;
            }
        }
        /// <summary>
        /// Обрабатываем документ
        /// </summary>
        /// <param name="message"></param>
        private async void HandleDocumentAsync(Telegram.Bot.Types.Message message)
        {
            // [!] String и string - это синонимы, но обычно пишут string

            string address = @"d:\botDownloads\" + message.Document.FileName;
            Console.WriteLine("Сохраняем документ как " + address);

            try
            {
                // Получаем JSON
                var file = await client.GetFileAsync(message.Document.FileId);
                // Открываем поток для записи на диск

                // [!] открытый поток надо закрывать, можно использовать using для удобства
                using (System.IO.FileStream saveStream = new System.IO.FileStream(address, System.IO.FileMode.Create))
                {
                    // Сохраняем файл
                    await client.DownloadFileAsync(file.FilePath, saveStream);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Обрабатываем изображение
        /// </summary>
        /// <param name="message"></param>
        private async void HandleImageAsync(Telegram.Bot.Types.Message message)
        {
            // message.Photo.Length - 1 это нужный нам файл
            string fileId = message.Photo[message.Photo.Length - 1].FileId;
            
            string address = @"d:\botDownloads\Photo_" + fileId.ToString().Substring(0, 5) + ".jpg";
            Console.WriteLine("Сохраняем изображение как " + address);

            try
            {
                // Получаем JSON
                var file = await client.GetFileAsync(message.Photo[message.Photo.Length - 1].FileId);
                // Открываем поток для записи на диск

                // [!] открытый поток надо закрывать, можно использовать using для удобства
                using (System.IO.FileStream saveStream = new System.IO.FileStream(address, System.IO.FileMode.Create))
                {
                    // Сохраняем файл
                    await client.DownloadFileAsync(file.FilePath, saveStream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Запуск бота
        /// </summary>
        public void Run()
        {
            // Запуск приема сообщений
            client.StartReceiving();
        }
    }
}
