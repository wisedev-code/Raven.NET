using System.Collections.Concurrent;
using AutoFixture;
using Moq;
using Raven.NET.Core.Extensions;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Storage;
using Raven.NET.Core.Storage.Interfaces;
using Raven.NET.Core.Subjects;
using Shouldly;
using Xunit;

namespace Raven.NET.Core.Tests.SubjectTests;

[Collection("Tests")]
public class RavenSubjectTests
{
    private readonly Mock<IRavenWatcher> _ravenWatcher;
    private readonly Mock<IRavenTypeWatcher> _ravenTypeWatcher;
    private readonly IRavenStorage _ravenStorage;
    
    public RavenSubjectTests()
    {
        _ravenWatcher = new Mock<IRavenWatcher>();
        _ravenTypeWatcher = new Mock<IRavenTypeWatcher>();
        _ravenStorage = RavenStorage.Instance;
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
        _ravenStorage.SubjectStorage.ShouldNotBeEmpty();
        _ravenStorage.SubjectStorage.ShouldContainKey(result.UniqueId);
        
        _ravenStorage.SubjectStorage.Clear();
    }
    
    [Fact]
    void CreateRavenSubject_Should_AddSubjectToTypeCache_When_TypeWatcherExists()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        _ravenStorage.RavenTypeWatcherStorage.TryAdd(typeof(TestSubjectEntity), _ravenTypeWatcher.Object);
        _ravenTypeWatcher.Setup(x => x.AttachSubject(It.IsAny<RavenSubject>())).Callback(() =>
        {
            _ravenStorage.SubjectTypeStorage.TryAdd(typeof(TestSubjectEntity), new ConcurrentDictionary<string, string>());
            _ravenStorage.SubjectTypeStorage[typeof(TestSubjectEntity)].TryAdd(mockedSubject.UniqueId.ToString(), mockedSubject.CreateCacheValue());
            mockedSubject.Observers.Add(_ravenTypeWatcher.Object);
        });
        
        //Act
        var result = new TestSubjectEntity();
        
        //We need some time for watcher to complete its task
        Thread.Sleep(2000);
        
        //Assert
        result.UniqueId.ShouldNotBe(Guid.Empty);
        _ravenStorage.SubjectTypeStorage.ShouldNotBeEmpty();
        _ravenStorage.SubjectTypeStorage.ShouldContainKey(typeof(TestSubjectEntity));
        
        _ravenStorage.SubjectTypeStorage.Clear();
        _ravenStorage.RavenTypeWatcherStorage.Clear();
    }

    [Fact]
    void DetachRavenSubject_Should_RemoveSubjectFromCache()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        _ravenStorage.SubjectStorage.TryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Detach(_ravenWatcher.Object);
        mockedSubject.Observers.ShouldBeEmpty();
        
        _ravenStorage.SubjectStorage.Clear();
    }
    
    [Fact]
    void Notify_Should_CallUpdateOnWatchers()
    {
        //Arrange
        _ravenStorage.SubjectStorage.Clear();
        _ravenStorage.RavenTypeWatcherStorage.Clear();
        var fixture = new Fixture();
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        _ravenStorage.SubjectStorage.TryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Value = "Value";
        mockedSubject.Changed = true;
        mockedSubject.TryNotify();
        
        mockedSubject.Observers.ShouldContain(_ravenWatcher.Object);
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
    }
}