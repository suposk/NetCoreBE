﻿namespace CommonCleanArch.Infrastructure.Infrastructure.EventBus;

public sealed record RabbitMqSettings(string Host, string Username = "guest", string Password = "guest");
