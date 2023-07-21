﻿namespace Cleanception.Application.Common.Models;

public class PaginatedResult<T> : Result
{
    internal PaginatedResult(bool succeeded, List<T> data, List<string> messages, int count = 0, int page = 1, int pageSize = 10)
    {
        Data = data;
        CurrentPage = page;
        Succeeded = succeeded;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Messages = messages;
    }

    public static PaginatedResult<T> Failure(List<string> messages)
    {
        return new(false, default!, messages);
    }

    public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
    {
        return new(true, data, default!, count, page, pageSize);
    }

    public List<T> Data { get; set; }

    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;

    public bool HasNextPage => CurrentPage < TotalPages;
}