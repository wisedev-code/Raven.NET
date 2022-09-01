using System.Collections.Concurrent;
using AutoFixture;
using Moq;
using Raven.NET.Core.Extensions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Static;
using Raven.NET.Core.Subjects;
using Shouldly;
using Xunit;

namespace Raven.NET.Core.Tests.SubjectTests;

public class RavenSubjectTests
{
    private readonly Mock<IRavenWatcher> _ravenWatcher;
    private readonly Mock<IRavenTypeWatcher> _ravenTypeWatcher;
    private Fixture _fixture;
    
    public RavenSubjectTests()
    {
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
        _fixture = new Fixture();
    }
    
    [Fact]
    void AttachRavenSubject_Should_AddSubjectToCache_When_NoTypeWatcherExists()
    {
        //Arrange
        var result = new TestSubjectEntity();
        
        //Act
        result.Attach(_ravenWatcher.Object);
        
        //Assert
        result.UniqueId.ShouldNotBe(Guid.Empty);
        result.Observers.ShouldContain(_ravenWatcher.Object);
        RavenCache.SubjectCache.ShouldNotBeEmpty();
        RavenCache.SubjectCache.ShouldContainKey(result.UniqueId);
    }
    
    [Fact]
    void CreateRavenSubject_Should_AddSubjectToTypeCache_When_TypeWatcherExists()
    {
        //Arrange
        var mockedSubject = _fixture.Create<TestSubjectEntity>();
        RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), _ravenTypeWatcher.Object);
        _ravenTypeWatcher.Setup(x => x.AttachSubject(It.IsAny<RavenSubject>())).Callback(() =>
        {
            RavenCache.SubjectTypeCache.TryAdd(typeof(TestSubjectEntity), new ConcurrentDictionary<string, string>());
            RavenCache.SubjectTypeCache[typeof(TestSubjectEntity)].TryAdd(mockedSubject.UniqueId.ToString(), mockedSubject.CreateCacheValue());
            mockedSubject.Observers.Add(_ravenTypeWatcher.Object);
        });
        
        //Act
        var result = new TestSubjectEntity();
        
        //Assert
        result.UniqueId.ShouldNotBe(Guid.Empty);
        mockedSubject.Observers.ShouldContain(_ravenTypeWatcher.Object);
        RavenCache.SubjectTypeCache.ShouldNotBeEmpty();
        RavenCache.SubjectTypeCache.ShouldContainKey(typeof(TestSubjectEntity));
        RavenCache.SubjectTypeCache[typeof(TestSubjectEntity)].ShouldContainKey(mockedSubject.UniqueId.ToString());
    }
}