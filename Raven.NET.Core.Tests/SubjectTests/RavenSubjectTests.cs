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
        _ravenStorage = RavenStorage.GetInstance();
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

        _ravenStorage.SubjectExists(result.UniqueId).ShouldBeTrue();
    }
    
    [Fact]
    void CreateRavenSubject_Should_AddSubjectToTypeCache_When_TypeWatcherExists()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        _ravenStorage.RavenTypeWatcherTryAdd(typeof(TestSubjectEntity), _ravenTypeWatcher.Object);
        _ravenTypeWatcher.Setup(x => x.AttachSubject(It.IsAny<RavenSubject>())).Callback(() =>
        {
            _ravenStorage.SubjectTypeTryAdd(typeof(TestSubjectEntity), new ConcurrentDictionary<string, string>());
            _ravenStorage.SubjectTypeValueTryAdd(typeof(TestSubjectEntity), mockedSubject.UniqueId.ToString(),
                mockedSubject.CreateCacheValue());
            mockedSubject.Observers.Add(_ravenTypeWatcher.Object);
        });
        
        //Act
        var result = new TestSubjectEntity();
        
        //We need some time for watcher to complete its task
        Thread.Sleep(2000);
        
        //Assert
        result.UniqueId.ShouldNotBe(Guid.Empty);
        _ravenStorage.SubjectTypeExists(typeof(TestSubjectEntity));
    }

    [Fact]
    void DetachRavenSubject_Should_RemoveSubjectFromCache()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Build<TestSubjectEntity>().Create();
        _ravenStorage.SubjectTryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Detach(_ravenWatcher.Object);
        mockedSubject.Observers.ShouldBeEmpty();
    }
    
    [Fact]
    void Notify_Should_CallUpdateOnWatchers()
    {
        //Arrange
        var fixture = new Fixture();
        var mockedSubject = fixture.Create<TestSubjectEntity>();
        _ravenStorage.SubjectTryAdd(mockedSubject.UniqueId, mockedSubject.CreateCacheValue());
        mockedSubject.Attach(_ravenWatcher.Object);
        
        //Act
        mockedSubject.Value = "Value";
        mockedSubject.Changed = true;
        mockedSubject.TryNotify();

        mockedSubject.Observers.ShouldContain(_ravenWatcher.Object);
        _ravenWatcher.Verify(x => x.Update(mockedSubject));
    }
}