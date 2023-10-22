using AWS.Application;
using Microsoft.AspNetCore.Mvc;

namespace AWS.WEB.Controllers
{
    public class SQSMessageController : Controller
    {
        private readonly ISQSMessageService _messageService;

        public SQSMessageController(ISQSMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<IActionResult> SQSMessageList()
        {
            var messages = await _messageService.GetMessagesFromQueueAsync();
            var messageCount = await _messageService.GetMessageCountAsync();
            ViewData["MessageCount"] = messageCount;
            return View(messages);
        }

        public IActionResult SendMessage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                // Send the message
                await _messageService.SendMessageAsync(message);
                TempData["SuccessMessage"] = "Message sent successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Message cannot be empty.";
            }

            return RedirectToAction("SQSMessageList");
        }
        [HttpGet]
        public async Task<IActionResult> DeleteMessage(string receiptHandle)
        {
            if (string.IsNullOrEmpty(receiptHandle))
            {
                return NotFound();
            }

            // Call your message deletion logic here
            await _messageService.DeleteMessageAsync(receiptHandle);

            TempData["SuccessMessage"] = "Message deleted successfully.";
            return RedirectToAction("SQSMessageList");
        }


    }
}
