using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnet_articles_api;

/// <summary>
/// In-memory implementation of IRepository for Article entities.
/// Suitable for testing, prototyping, and development scenarios.
/// </summary>
public class InMemoryRepository : IRepository
{
    private readonly Dictionary<Guid, Article> _articles = new();
    private readonly object _lockObject = new();

    public InMemoryRepository()
    {
        InitializeWithSampleData();
    }

    /// <summary>
    /// Initializes the repository with a sample article entry.
    /// </summary>
    private void InitializeWithSampleData()
    {
        var sampleArticle = new Article
        {
            Id = Guid.Parse("6B29FC40-CA47-1067-B31D-00DD010662DA"),
            Title = "some title",
            Text = "This is hardcoded sample text for demonstration purposes. You can retrieve, update, or delete this article using the repository methods."
        };

        lock (_lockObject)
        {
            _articles[sampleArticle.Id] = sampleArticle;
        }
    }

    /// <summary>
    /// Returns a found article or null.
    /// </summary>
    public Article Get(Guid id)
    {
        lock (_lockObject)
        {
            _articles.TryGetValue(id, out var article);
            return article;
        }
    }

    /// <summary>
    /// Creates a new article and returns its identifier.
    /// Throws an exception if article is null.
    /// Throws an exception if title is null or empty.
    /// </summary>
    public Guid Create(Article article)
    {
        if (article == null)
            throw new ArgumentNullException(nameof(article));
        if (string.IsNullOrWhiteSpace(article.Title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(article.Title));

        lock (_lockObject)
        {
            if (article.Id == Guid.Empty)
                article.Id = Guid.NewGuid();

            _articles[article.Id] = article;
            return article.Id;
        }
    }

    /// <summary>
    /// Returns true if an article was deleted or false if it was not possible to find it.
    /// </summary>
    public bool Delete(Guid id)
    {
        lock (_lockObject)
        {
            return _articles.Remove(id);
        }
    }

    /// <summary>
    /// Returns true if an article was updated or false if it was not possible to find it.
    /// Throws an exception if articleToUpdate is null.
    /// Throws an exception if title is null or empty.
    /// </summary>
    public bool Update(Article articleToUpdate)
    {
        if (articleToUpdate == null)
            throw new ArgumentNullException(nameof(articleToUpdate));
        if (string.IsNullOrWhiteSpace(articleToUpdate.Title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(articleToUpdate.Title));

        lock (_lockObject)
        {
            if (!_articles.ContainsKey(articleToUpdate.Id))
                return false;

            _articles[articleToUpdate.Id] = articleToUpdate;
            return true;
        }
    }
}