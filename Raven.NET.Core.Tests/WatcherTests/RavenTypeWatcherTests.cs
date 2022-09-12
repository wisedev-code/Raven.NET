using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Raven.NET.Core.Configuration;
using Raven.NET.Core.Observers;
using Raven.NET.Core.Observers.Interfaces;
using Raven.NET.Core.Providers.Interfaces;
using Raven.NET.Core.Static;
using Raven.NET.Core.Subjects;
using Raven.NET.Core.Tests.SubjectTests;
using Shouldly;
using Xunit;

namespace Raven.NET.Core.Tests.WatcherTests;

[Collection("Tests")]
public class RavenTypeWatcherTests
{
    private Fixture _fixture;
    private readonly IRavenTypeWatcher sut;
    private readonly Mock<IRavenProvider> _ravenProvider;
    private readonly Mock<IRavenSettingsProvider> _ravenSettingsProvider;
    
    public RavenTypeWatcherTests()
    {
        _fixture = new Fixture();
        _ravenProvider = new Mock<IRavenProvider>();
        _ravenSettingsProvider = new Mock<IRavenSettingsProvider>();
        sut = new RavenTypeWatcher(_ravenProvider.Object, _ravenSettingsProvider.Object);
    }
    
    [Fact]
    void Create_Should_AddWatcherToCacheWithDefaultConfiguration_When_NoConfigFileSpecified()
    {
        //Arrange
        var watcherName = "TestWatcher";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });
        
        //Act
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);
        
        //Assert
        RavenCache.RavenTypeWatcherCache.ShouldContainKey(typeof(TestSubjectEntity));
        var settings = (sut as RavenTypeWatcher)?._ravenSettings;
        settings.LogLevel.ShouldBe(LogLevel.Warning);
        settings.AutoDestroy.ShouldBe(false);
        settings.BackgroundWorker.ShouldBe(true);
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, typeof(TestSubjectEntity)));
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
    }
    
    [Fact]
    void Create_Should_AddWatcherToCacheWithFileConfiguration_When_ConfigFileSpecified()
    {
        //Arrange
        var watcherName = "TestWatcher";
        var settingsMock = _fixture.Create<RavenSettings>();
        settingsMock.AutoDestroy = true;
        settingsMock.LogLevel = LogLevel.Trace;
        
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });

        _ravenSettingsProvider.Setup(x => x.GetRaven(watcherName)).Returns(settingsMock);
        
        //Act
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);
        
        //Assert
        RavenCache.RavenTypeWatcherCache.ShouldContainKey(typeof(TestSubjectEntity));
        var settings = (sut as RavenTypeWatcher)?._ravenSettings;
        settings.ShouldNotBeNull();
        settings.LogLevel.ShouldBe(LogLevel.Trace);
        settings.AutoDestroy.ShouldBe(true);
        settings.BackgroundWorker.ShouldBe(true);
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, typeof(TestSubjectEntity)));
    }
    
    [Fact]
    void Create_Should_AddWatcherToCacheWithProvidedConfiguration_When_OptionsParameterProvided()
    {
        //Arrange
        var watcherName = "TestWatcher";
        var settingsMock = _fixture.Create<RavenSettings>();
        settingsMock.AutoDestroy = true;
        settingsMock.LogLevel = LogLevel.Trace;
        
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });

        _ravenSettingsProvider.Setup(x => x.GetRaven(watcherName)).Returns(settingsMock);
        
        //Act
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value), subject => true, ravenSettings =>
        {
            ravenSettings.AutoDestroy = false;
            ravenSettings.LogLevel = LogLevel.Critical;
        });
        
        //Assert
        RavenCache.RavenTypeWatcherCache.ShouldContainKey(typeof(TestSubjectEntity));
        var settings = (sut as RavenTypeWatcher)?._ravenSettings;
        settings.LogLevel.ShouldBe(LogLevel.Critical);
        settings.AutoDestroy.ShouldBe(false);
        settings.BackgroundWorker.ShouldBe(true);
        _ravenSettingsProvider.Verify(x => x.GetRaven(watcherName));
        _ravenProvider.Verify(x => x.AddRaven(watcherName, sut, typeof(TestSubjectEntity)));
    }
    
    [Fact]
    void Attach_Should_AddSubjectToInternalCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });
        
        //Act
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);
        var mockedSubject = new TestSubjectEntity();
        mockedSubject.Value = "unique_id";

        //We need to give some time for background worker to do its job
        Thread.Sleep(2000);
        
        //Assert
        var watcher = sut as RavenTypeWatcher;
        watcher._watchedSubjects.ShouldContain(mockedSubject);
        mockedSubject.Observers.ShouldContain(watcher);
    }
    
    [Fact]
    void Exclude_Should_RemoveSubjectFromCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });
        
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);
        var mockedSubject = new TestSubjectEntity();
        mockedSubject.Value = "unique_id";
        
        //Act
        sut.Exclude(mockedSubject);
        
        //Assert
        var watcher = sut as RavenTypeWatcher;
        watcher._watchedSubjects.ShouldNotContain(mockedSubject);
    }
    
    
    [Fact]
    void Stop_Should_RemoveAllSubjectsFromInternalCollection()
    {
        //Arrange
        var watcherName = "TestSubject";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });
        
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);

        var mockedSubject1 = new TestSubjectEntity();
        var mockedSubject2 = new TestSubjectEntity();
        
        _ravenProvider.Setup(x => x.GetRaven(watcherName, null)).Returns(sut as RavenTypeWatcher);
        _ravenProvider.Setup(x => x.UpdateSubjects(It.IsAny<string>(), It.IsAny<IEnumerable<RavenSubject>>(), null))
            .Callback(
                () =>
                {
                    mockedSubject1.Observers.Clear();
                    mockedSubject2.Observers.Clear();
                });
        
        
        //Act
        sut.Stop(watcherName);

        //Assert
        var watcher = sut as RavenTypeWatcher;
        watcher._watchedSubjects.ShouldNotContain(mockedSubject1);
        watcher._watchedSubjects.ShouldNotContain(mockedSubject2);
        mockedSubject1.Observers.ShouldNotContain(watcher);
        mockedSubject2.Observers.ShouldNotContain(watcher);
        _ravenProvider.Verify(x => x.UpdateSubjects(watcherName, It.IsAny<IEnumerable<RavenSubject>>(), null));
    }

    [Fact]
    void UpdateNewestSubject_Should_ChangeSubjectToItsNewestVersion()
    {
        //Arrange
        var watcherName = "TestSubject";
        _ravenProvider.Setup(x => x.AddRaven(It.IsAny<string>(), It.IsAny<IRavenTypeWatcher>(), typeof(TestSubjectEntity))).Callback(() =>
        {
            RavenCache.RavenTypeWatcherCache.TryAdd(typeof(TestSubjectEntity), sut);
        });
        
        sut.Create<TestSubjectEntity>(watcherName, nameof(TestSubjectEntity.Value),subject => true);

        var mockedSubject = new TestSubjectEntity();
        mockedSubject.Value = "unique_id";
        mockedSubject.Changed = true;
        
        //Act
        sut.UpdateNewestSubject(mockedSubject.Value, mockedSubject);
        
        //Assert
        var watcher = sut as RavenTypeWatcher;
        watcher._watchedSubjects.ShouldContain(mockedSubject);
        var subject = watcher._watchedSubjects.FirstOrDefault(x => x.UniqueId == mockedSubject.UniqueId);
        (subject as TestSubjectEntity)?.Changed.ShouldBe(true);
    }
}