﻿using CSharpFunctionalExtensions;
using SachkovTech.Domain.Shared;

namespace SachkovTech.Domain.IssueReview.ValueObjects;

public record Message
{
    private Message(string value)
    {
        Value = value;
    }
    
    public string Value { get; }
    
    public static Result<Message, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Errors.General.ValueIsInvalid(nameof(Message));
        }

        if (value.Length > Constants.Default.MAX_HIGH_TEXT_LENGTH)
        {
            return Errors.General.ValueIsInvalid(nameof(Message));
        }
        
        return new Message(value);
    }
}