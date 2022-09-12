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

[Collection("Tests")]
public class RavenSubjectTests
{
    private readonly Mock<IRavenWatcher> _ravenWatcher;
    private readonly Mock<IRavenTypeWatcher> _ravenTypeWatcher;
    
    public RavenSubjectTests()
    {
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
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
        
        RavenCache.SubjectCache.Clear();
    }
    
    [Fact]
    void CreateRavenSubject_Should_AddSubjectToTypeCache_When_TypeWatcherExists()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), _ravenTypeWatcher.Object);
        _ravenTypeWatcher.Setup(x => x.AttachSubject(It.IsAny<RavenSubject>())).Callback(() =>
        {
            RavenCache.SubjectTypeCache.TryAdd(typeof(TestSubjectEntity), new ConcurrentDictionary<string, string>());
            RavenCache.SubjectTypeCache[typeof(TestSubjectEntity)].TryAdd(mockedSubject.UniqueId.ToString(), mockedSubject.CreateCacheValue());
            mockedSubject.Observers.Add(_ravenTypeWatcher.Object);
        });
        
        //Act
        var result = new TestSubjectEntity();
        
        //We need some time for watcher to complete its task
        Thread.Sleep(2000);
        
        //Assert
        result.UniqueId.ShouldNotBe(Guid.Empty);
        RavenCache.SubjectTypeCache.ShouldNotBeEmpty();
        RavenCache.SubjectTypeCache.ShouldContainKey(typeof(TestSubjectEntity));
        
        RavenCache.SubjectTypeCache.Clear();
        RavenCache.RavenTypeWatcherCache.Clear();
    }

    [Fact]
    void DetachRavenSubject_Should_RemoveSubjectFromCache()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        RavenCache.SubjectCache.TryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Detach(_ravenWatcher.Object);
        mockedSubject.Observers.ShouldBeEmpty();
        
        RavenCache.SubjectCache.Clear();
    }
    
    [Fact]
    void Notify_Should_CallUpdateOnWatchers()
    {
        //Arrange
        RavenCache.SubjectCache.Clear();
        RavenCache.RavenTypeWatcherCache.Clear();
        var fixture = new Fixture();
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        RavenCache.SubjectCache.TryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Value = "Value";
        mockedSubject.Changed = true;
        mockedSubject.TryNotify();
        
        mockedSubject.Observers.ShouldContain(_ravenWatcher.Object);
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
    }
}